using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public abstract class Unit : MonoBehaviour
{
	[Serializable]
	public class UnitData
	{
		public string identifierName;
		public Vector2Int currentPosition;

		public UnitData(string identifierName, Vector2Int currentPosition)
		{
			this.identifierName = identifierName;
			this.currentPosition = currentPosition;
		}
	}

	[Serializable]
	public class TalkUI
	{
		public Canvas canvas;
		public Animator drawingBoardAnimator;

		[Multiline]
		public List<string> texts = new List<string> {""};

		public TextMeshProUGUI displayText;

		[HideInInspector]
		public float talkTimer;

		[HideInInspector]
		public Coroutine talkCoroutine;

		public void Talk(MonoBehaviour coroutineStarter, string text = null)
		{
			if (text == null)
			{
				text = GetRandomText();
			}

			if (talkCoroutine != null)
			{
				UpdateTalkTimer(text);
			}
			else
			{
				talkCoroutine = coroutineStarter.StartCoroutine(TalkCoroutine(text));
			}
		}

		public void StopTalk(MonoBehaviour coroutineStarter, bool force)
		{
			if (force)
			{
				if (canvas)
				{
					canvas.gameObject.SetActive(false);
				}

				if (talkCoroutine != null)
				{
					coroutineStarter.StopCoroutine(talkCoroutine);
				}
			}
			else
			{
				if (talkCoroutine != null)
				{
					talkTimer = 0;
				}
			}
		}

		public string GetRandomText()
		{
			return texts.OrderBy(t => Guid.NewGuid()).First();
		}

		private void UpdateTalkTimer(string text = null)
		{
			if (text != null)
			{
				displayText.text = text;
			}

			talkTimer = Time.time + 3;
		}

		private IEnumerator TalkCoroutine(string text)
		{
			displayText.text = text;
			canvas.gameObject.SetActive(true);
			drawingBoardAnimator.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
			yield return new WaitForSeconds(0.6f);
			displayText.gameObject.SetActive(true);
			UpdateTalkTimer();
			yield return new WaitUntil(() => Time.time > talkTimer);
			displayText.gameObject.SetActive(false);
			drawingBoardAnimator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
			yield return new WaitForSeconds(0.6f);
			canvas.gameObject.SetActive(false);
			talkCoroutine = null;
		}
	}

	[Header("Data")]
	[SerializeField]
	protected string identifierName;
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

	[Header("Initialization")]
	public bool initializeSelf = true;

	[Header("Interaction")]
	public List<Interaction> interactions;

	public bool talksWhenInteractedWith;

	[SerializeField]
	[DrawIf("talksWhenInteractedWith", true)]
	protected TalkUI talkUI;

	private Observer gameStateChangedObserver;

	protected virtual void Start()
	{
		gameStateChangedObserver = new Observer(GameStateChanged);
		GameLogic.Instance.gameStateObservers.Add(gameStateChangedObserver);
		if (ApplyDataOnStart())
		{
			ApplyUnitData(GameLogic.Instance.currentLevelState.GetDataByName(identifierName));
		}

		if (talkUI.canvas)
		{
			talkUI.canvas.worldCamera = GameCamera.Instance.camera;
			talkUI.canvas.gameObject.SetActive(false);
		}
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

	protected Coroutine Move(MovementDirectionUtilities.MovementDirection movementDirection, bool force = false)
	{
		if (movementDirection != MovementDirectionUtilities.MovementDirection.None)
		{
			return Move(currentPosition + MovementDirectionUtilities.VectorFromDirection(movementDirection), force);
		}

		return null;
	}

	public Coroutine Move(Vector2Int newPosition, bool force = false)
	{
		if (newPosition == currentPosition)
		{
			return null;
		}

		Coroutine coroutine = null;
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
				coroutine = CoroutineStarter().StartCoroutine(MovementSequence(newPosition));
			}

			OccupyTile();
		}
		else
		{
			coroutine = CoroutineStarter()
				.StartCoroutine(CantMoveSequence(newPosition, currentPosition.x != newPosition.x));
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

		return coroutine;
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
		transform.position = new Vector3(x, y, y);
		if (jump)
		{
			var spriteJump = jumpStart.y + movementDisplaceCurve.Evaluate(characterDisplaceT);
			sprite.transform.localPosition = new Vector3(jumpStart.x, spriteJump, 0);
		}
	}

	protected Vector3 GetCurrentPosition()
	{
		var cellCenterWorld = GameLogic.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(currentPosition);
		cellCenterWorld.z = cellCenterWorld.y;
		return cellCenterWorld;
	}

	protected Vector3 GetPosition(Vector2Int position)
	{
		var cellCenterWorld = GameLogic.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(position);
		cellCenterWorld.z = cellCenterWorld.y;
		return cellCenterWorld;
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

	public abstract void Talk(string text = null);

	public void StopTalk(bool force)
	{
		talkUI.StopTalk(CoroutineStarter(), force);
	}

	public abstract void Die();

	public virtual void ApplyUnitData(UnitData unitData)
	{
		if (unitData != null)
		{
			spawnPoint = unitData.currentPosition;
		}
	}
	
	public virtual UnitData GetUnitData()
	{
		return new UnitData(identifierName, currentPosition);
	}

	protected virtual bool ApplyDataOnStart()
	{
		return true;
	}

	public void RemoveGameStateObserver()
	{
		Debug.Log("Remove" + name);
		GameLogic.Instance.gameStateObservers.Remove(gameStateChangedObserver);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		Gizmos.DrawCube(CubeLocation(spawnPoint), size);

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(Unit), true)]
	public class UnitCustomInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var unit = (Unit) target;
			if (GUILayout.Button("Recalculate Position"))
			{
				unit.transform.position = (Vector3Int) unit.spawnPoint + new Vector3(0.5f, 0.5f, 0);
			}

			if (GUILayout.Button("Recalculate Spawn Point Based on Position"))
			{
				var position = Vector2Int.FloorToInt(unit.transform.position);
				unit.spawnPoint = position;
			}
		}
	}
#endif
}