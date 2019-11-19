using System;
using System.Collections;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [Tooltip("A value of 5 means traveling will take 0.2 seconds, 1 = 1 second.")]
    [SerializeField]
    protected float movementSpeed = 5;

    protected float defaultMovementSpeed;

    [SerializeField]
    protected SpriteRenderer sprite;

    [SerializeField]
    protected AnimationCurve movementSpeedCurve;

    [SerializeField]
    protected AnimationCurve movementDisplaceCurve;

    [SerializeField]
    protected Vector3Int spawnPoint;

    protected Vector3Int currentPosition;

    protected virtual void Start()
    {
        GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
    }

    public virtual void Initialize(Vector3Int location)
    {
        if (World.Instance.tileMapInitialized)
        {
            Move(location, true);
        }
        else
        {
            spawnPoint = location;
            World.Instance.tileMapObservers.Add(new Observer(ForceInitializePosition));
        }

        defaultMovementSpeed = movementSpeed;
    }

    protected void ForceInitializePosition()
    {
        Move(spawnPoint, true);
    }

    protected void Move(MovementDirectionUtilities.MovementDirection movementDirection, bool force = false)
    {
        Move(currentPosition + MovementDirectionUtilities.VectorFromDirection(movementDirection), force);
    }

    public virtual void Move(Vector3Int newPosition, bool force = false)
    {
        var (cantMoveReason, obstacle, unit) = World.Instance.CanWalk(newPosition);
        if (cantMoveReason == GameTile.CantMoveReason.None)
        {
            World.Instance.UnoccupyTargetTile(currentPosition);
            currentPosition = newPosition;
            if (force)
            {
                ForceMove();
            }
            else
            {
                StartCoroutine(MovementSequence(newPosition));
            }

            World.Instance.OccupyTargetTile(currentPosition, this);
        }
        else
        {
            StartCoroutine(CantMoveSequence(newPosition, currentPosition.x != newPosition.x));
            switch (cantMoveReason)
            {
                case GameTile.CantMoveReason.NonWalkable:
                    break;
                case GameTile.CantMoveReason.Obstacles:
                    InteractWithObject(obstacle);
                    break;
                case GameTile.CantMoveReason.Unit:
                    InteractWithObject(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    protected abstract void InteractWithObject(Obstacle obstacle);

    protected abstract void InteractWithObject(Unit unit);

    protected virtual void ForceMove()
    {
        transform.position = GetCurrentPosition();
    }

    protected virtual IEnumerator MovementSequence(Vector3Int newPosition)
    {
        var start = transform.position;
        var end = GetPosition(newPosition);
        sprite.flipX = start.x > end.x;
        var jumpStart = sprite.transform.localPosition;
        float t = 0;
        while (t < 1 - Time.deltaTime * movementSpeed / 2)
        {
            yield return null;
            t += Time.deltaTime * movementSpeed;
            if (t > 1)
            {
                t = 1;
            }

            CharacterMovement(t, t, start, end, true, jumpStart);
        }

        CharacterMovement(1, 1, start, end, true, jumpStart, false);
    }

    protected virtual IEnumerator CantMoveSequence(Vector3Int newPosition, bool isHorizontal)
    {
        var start = transform.position;
        var end = GetPosition(newPosition);
        var jumpStart = sprite.transform.localPosition;
        float t = 0;
        const float moveUntil = 0.3f;
        while (t < moveUntil - Time.deltaTime * movementSpeed / 2)
        {
            yield return null;
            t += Time.deltaTime * movementSpeed;
            CharacterMovement(t, t, start, end, isHorizontal, jumpStart, false);
        }

        end = start;
        start = transform.position;
        const float modifier = 1 / moveUntil;
        float movementT = 0;
        while (t < 1 - Time.deltaTime * movementSpeed / 2)
        {
            yield return null;
            var timeTick = Time.deltaTime * movementSpeed;
            t += timeTick;
            if (t > 1)
            {
                t = 1;
            }

            movementT += timeTick * modifier;

            if (movementT > 1)
            {
                movementT = 1;
            }

            CharacterMovement(movementT, t, start, end, isHorizontal, jumpStart, false);
        }

        CharacterMovement(1, 1, start, end, isHorizontal, jumpStart, false);
    }

    protected virtual void CharacterMovement(float movementT, float characterDisplaceT, Vector3 start, Vector3 end,
        bool jump, Vector3 jumpStart, bool updateCamera = true)
    {
        var speed = movementSpeedCurve.Evaluate(movementT);
        var x = Mathf.Lerp(start.x, end.x, speed);
        var y = Mathf.Lerp(start.y, end.y, speed);
        transform.position = new Vector3(x, y, end.z);
        if (jump)
        {
            var spriteJump = jumpStart.y + movementDisplaceCurve.Evaluate(characterDisplaceT);
            sprite.transform.localPosition = new Vector3(jumpStart.x, spriteJump, jumpStart.z);
        }
    }

    protected Vector3 GetCurrentPosition()
    {
        return World.Instance.GetCellCenterWorld(currentPosition);
    }

    protected Vector3 GetPosition(Vector3Int position)
    {
        return World.Instance.GetCellCenterWorld(position);
    }

    protected abstract void GameStateChanged();
    
    protected abstract void Die();
}