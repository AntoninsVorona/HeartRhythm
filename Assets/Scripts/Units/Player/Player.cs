using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Player : Unit
{
    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        Initialize(spawnPoint);
    }

    protected override void ForceMove()
    {
        base.ForceMove();
        GameCamera.Instance.ChangeTargetPosition(transform.position, true);
    }

    public void ReceiveInput(MovementDirectionUtilities.MovementDirection movementDirection)
    {
        Move(movementDirection);
    }

    protected override IEnumerator MovementSequence(Vector3Int newPosition)
    {
        PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
        yield return base.MovementSequence(newPosition);
        PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
    }

    protected override IEnumerator CantMoveSequence(Vector3Int newPosition, bool isHorizontal)
    {
        PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
        yield return base.CantMoveSequence(newPosition, isHorizontal);
        PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
    }

    protected override void CharacterMovement(float movementT, float characterDisplaceT, Vector3 start, Vector3 end,
        bool jump, Vector3 jumpStart, bool updateCamera = true)
    {
        base.CharacterMovement(movementT, characterDisplaceT, start, end, jump, jumpStart, updateCamera);
        if (updateCamera)
        {
            GameCamera.Instance.ChangeTargetPosition(transform.position);
        }
    }

    protected override void GameStateChanged()
    {
        var newGameState = GameLogic.Instance.CurrentGameState;
        switch (newGameState)
        {
            case GameLogic.GameState.Peace:
                movementSpeed = defaultMovementSpeed;
                break;
            case GameLogic.GameState.Fight:
                movementSpeed = 3 + 0.03f * AudioManager.Instance.bpm;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        }
    }

    protected override void Die()
    {
        //TODO Lose Game
    }

    protected override void InteractWithObject(Obstacle obstacle)
    {
    }

    protected override void InteractWithObject(Unit unit)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(spawnPoint + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
    }

    public static Player Instance { get; private set; }
}