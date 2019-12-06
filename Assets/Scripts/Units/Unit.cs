using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	protected Vector2Int spawnPoint;

	protected Vector2Int currentPosition = new Vector2Int(int.MinValue, int.MaxValue);
	public Vector2Int CurrentPosition => currentPosition;

	[Header("Interaction")]
	public List<Interaction> interactions;

	protected virtual void Start()
	{
		GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
	}

	public virtual void Initialize(Vector2Int location)
	{
		if (GameLogic.Instance.currentSceneObjects.currentWorld.tileMapInitialized)
		{
			Move(location, true);
		}
		else
		{
			spawnPoint = location;
			GameLogic.Instance.currentSceneObjects.currentWorld.tileMapObservers.Add(
				new Observer(ForceInitializePosition));
		}

		defaultMovementSpeed = movementSpeed;
		InitializeInteractions();
	}

	protected void InitializeInteractions()
	{
		var inter = new List<Interaction>();
		foreach (var initializedInteraction in interactions.Where(i => i).Select(Instantiate))
		{
			initializedInteraction.Initialize(this);
			inter.Add(initializedInteraction);
		}

		interactions = inter;
	}

	protected void ForceInitializePosition()
	{
		Move(spawnPoint, true);
	}

	protected void Move(MovementDirectionUtilities.MovementDirection movementDirection, bool force = false)
	{
		if (movementDirection != MovementDirectionUtilities.MovementDirection.None)
		{
			Move(currentPosition + MovementDirectionUtilities.VectorFromDirection(movementDirection), force);
		}
	}

	public virtual void Move(Vector2Int newPosition, bool force = false)
	{
		if (newPosition == currentPosition)
		{
			return;
		}

		var (cantMoveReason, unit) = GameLogic.Instance.currentSceneObjects.currentWorld.CanWalk(newPosition);
		if (cantMoveReason == GameTile.CantMoveReason.None || force)
		{
			UnoccupyTile();
			currentPosition = newPosition;
			if (force)
			{
				ForceMove();
			}
			else
			{
				CoroutineStarter().StartCoroutine(MovementSequence(newPosition));
			}

			OccupyTile();
		}
		else
		{
			CoroutineStarter().StartCoroutine(CantMoveSequence(newPosition, currentPosition.x != newPosition.x));
			switch (cantMoveReason)
			{
				case GameTile.CantMoveReason.NonWalkable:
					break;
				case GameTile.CantMoveReason.Obstacles:
				case GameTile.CantMoveReason.Unit:
					InteractWithObject(unit);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	protected virtual void OccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.OccupyTargetTile(currentPosition, this);
	}

	protected virtual void UnoccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.UnoccupyTargetTile(currentPosition);
	}

	protected abstract void InteractWithObject(Unit unit);

	protected virtual void ForceMove()
	{
		transform.position = GetCurrentPosition();
	}

	protected virtual IEnumerator MovementSequence(Vector2Int newPosition)
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

	protected virtual IEnumerator CantMoveSequence(Vector2Int newPosition, bool isHorizontal)
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
		return GameLogic.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(currentPosition);
	}

	protected Vector3 GetPosition(Vector2Int position)
	{
		return GameLogic.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(position);
	}

	protected void GameStateChanged()
	{
		var newGameState = GameLogic.Instance.CurrentGameState;
		switch (newGameState)
		{
			case GameLogic.GameState.Peace:
				PeaceState();
				break;
			case GameLogic.GameState.Fight:
				FightState();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
		}
	}

	protected virtual void PeaceState()
	{
		movementSpeed = defaultMovementSpeed;
	}

	protected virtual void FightState()
	{
		movementSpeed = defaultMovementSpeed * 0.6f + 0.03f * AudioManager.Instance.bpm;
	}

	public Interaction InteractionMatches(List<MovementDirectionUtilities.MovementDirection> acceptorDanceMoveSet)
	{
		return interactions.FirstOrDefault(interaction => interaction.DanceMoveCanBeApplied(acceptorDanceMoveSet));
	}

	protected virtual MonoBehaviour CoroutineStarter()
	{
		return GameLogic.Instance.currentSceneObjects.currentMobManager;
	}

	public abstract void Die();
}