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
	protected static readonly Color GOOD_EFFECT_COLOR = new Color32(76, 221, 32, 255);
	protected static readonly Color BAD_EFFECT_COLOR = new Color32(222, 32, 32, 255);

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

		private bool canUpdateTalkTimer;

		public void Talk(MonoBehaviour coroutineStarter, string text = null)
		{
			if (text == null)
			{
				text = GetRandomText();
			}

			if (talkCoroutine != null)
			{
				if (canUpdateTalkTimer)
				{
					UpdateTalkTimer(text);
				}
				else
				{
					coroutineStarter.StopCoroutine(talkCoroutine);
					talkCoroutine = coroutineStarter.StartCoroutine(TalkCoroutine(text));
				}
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
			canUpdateTalkTimer = true;
			displayText.text = text;
			canvas.gameObject.SetActive(true);
			drawingBoardAnimator.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
			yield return new WaitForSeconds(0.6f);
			displayText.gameObject.SetActive(true);
			UpdateTalkTimer();
			yield return new WaitUntil(() => Time.time > talkTimer);
			canUpdateTalkTimer = false;
			displayText.gameObject.SetActive(false);
			drawingBoardAnimator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
			yield return new WaitForSeconds(0.6f);
			canvas.gameObject.SetActive(false);
			talkCoroutine = null;
		}
	}

	public Animator animator;

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

	[DrawIf("initializeSpawnPointForMe", false)]
	[SerializeField]
	protected Vector2Int spawnPoint;

	[SerializeField]
	protected List<Vector2Int> otherPoints;

	protected Vector2Int currentPosition = new Vector2Int(int.MinValue, int.MaxValue);
	public Vector2Int CurrentPosition => currentPosition;

	[Header("Initialization")]
	public bool initializeSelf = true;

	public bool initializeSpawnPointForMe;

	[Header("Interaction")]
	public Interaction headsetLessInteraction;

	public List<Interaction> interactions;

	public bool startsConversationUponInteraction;

	[DrawIf("startsConversationUponInteraction", true)]
	public string conversationToStart;

	public bool talksWhenInteractedWith;

	[SerializeField]
	[DrawIf("talksWhenInteractedWith", true)]
	protected TalkUI talkUI;

	private Observer gameStateChangedObserver;
	protected Coroutine colorTintCoroutine;
	protected Coroutine shakeCoroutine;

	protected virtual void Start()
	{
		if (ApplyDataOnStart())
		{
			ApplyUnitData(GameSessionManager.Instance.currentLevelState.GetDataByName(identifierName));
		}
		else if (initializeSpawnPointForMe)
		{
			spawnPoint = Vector2Int.FloorToInt(transform.position);
		}

		if (talkUI.canvas)
		{
			talkUI.canvas.worldCamera = GameCamera.Instance.camera;
			talkUI.canvas.gameObject.SetActive(false);
		}
	}

	public virtual void Initialize(Vector2Int location)
	{
		if (GameSessionManager.Instance.currentSceneObjects.currentWorld.tileMapInitialized)
		{
			Move(location, true);
		}
		else
		{
			spawnPoint = location;
			GameSessionManager.Instance.currentSceneObjects.currentWorld.tileMapObservers.Add(
				new Observer(this, ForceInitializePosition));
		}

		defaultMovementSpeed = movementSpeed;
		InitializeInteractions();
		gameStateChangedObserver = new Observer(this, GameStateChanged);
		GameSessionManager.Instance.gameStateObservers.Add(gameStateChangedObserver);
	}

	public void InitializeInteractions()
	{
		var inter = new List<Interaction>();
		foreach (var initializedInteraction in interactions.Where(i => i).Select(Instantiate))
		{
			initializedInteraction.Initialize(this);
			inter.Add(initializedInteraction);
		}

		interactions = inter;
		if (headsetLessInteraction)
		{
			headsetLessInteraction = Instantiate(headsetLessInteraction);
			headsetLessInteraction.Initialize(this);
		}
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

		Coroutine coroutine;
		var (cantMoveReason, unit) = GameSessionManager.Instance.currentSceneObjects.currentWorld.CanWalk(newPosition);
		if (cantMoveReason == GameTile.CantMoveReason.None || force)
		{
			coroutine = SuccessfulMove(newPosition, force);
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

	protected virtual Coroutine SuccessfulMove(Vector2Int newPosition, bool force)
	{
		UnoccupyTile();
		currentPosition = newPosition;
		Coroutine coroutine = null;
		if (force)
		{
			ForceMove();
		}
		else
		{
			coroutine = CoroutineStarter().StartCoroutine(MovementSequence(newPosition));
		}

		OccupyTile();
		return coroutine;
	}

	protected virtual void OccupyTile()
	{
		GameSessionManager.Instance.currentSceneObjects.currentWorld.AddUnitToTile(currentPosition, this);
		foreach (var point in otherPoints)
		{
			GameSessionManager.Instance.currentSceneObjects.currentWorld.AddUnitToTile(point, this);
		}
	}

	protected virtual void UnoccupyTile()
	{
		GameSessionManager.Instance.currentSceneObjects.currentWorld.RemoveUnitFromTile(currentPosition, this);

		foreach (var otherPoint in otherPoints)
		{
			GameSessionManager.Instance.currentSceneObjects.currentWorld.RemoveUnitFromTile(otherPoint, this);
		}
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
		if (!Mathf.Approximately(start.x, end.x))
		{
			sprite.flipX = start.x > end.x;
		}

		var jumpStart = sprite.transform.localPosition.y;
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
		if (!Mathf.Approximately(start.x, end.x))
		{
			sprite.flipX = start.x > end.x;
		}

		var jumpStart = sprite.transform.localPosition.y;
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
		bool jump, float jumpStart, bool updateCamera = true)
	{
		var speed = movementSpeedCurve.Evaluate(movementT);
		var x = Mathf.Lerp(start.x, end.x, speed);
		var y = Mathf.Lerp(start.y, end.y, speed);
		var spriteTransform = sprite.transform;
		transform.position = new Vector3(x, y, y);
		if (jump)
		{
			var spriteJump = jumpStart + movementDisplaceCurve.Evaluate(characterDisplaceT);
			spriteTransform.localPosition = new Vector3(spriteTransform.localPosition.x, spriteJump, 0);
		}
	}

	protected Vector3 GetCurrentPosition()
	{
		var cellCenterWorld =
			GameSessionManager.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(currentPosition);
		cellCenterWorld.z = cellCenterWorld.y;
		return cellCenterWorld;
	}

	protected Vector3 GetPosition(Vector2Int position)
	{
		var cellCenterWorld = GameSessionManager.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(position);
		cellCenterWorld.z = cellCenterWorld.y;
		return cellCenterWorld;
	}

	protected void GameStateChanged()
	{
		var newGameState = GameSessionManager.Instance.CurrentGameState;
		switch (newGameState)
		{
			case GameSessionManager.GameState.Peace:
				PeaceState();
				break;
			case GameSessionManager.GameState.Fight:
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
		return GameSessionManager.Instance.currentSceneObjects.currentMobManager;
	}

	public abstract void Talk(string text = null);

	public void StopTalk(bool force)
	{
		talkUI.StopTalk(CoroutineStarter(), force);
	}

	public abstract void Die();

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public virtual void ApplyUnitData(UnitData unitData)
	{
		if (unitData != null)
		{
			spawnPoint = unitData.currentPosition;
		}
		else if (initializeSpawnPointForMe)
		{
			spawnPoint = Vector2Int.FloorToInt(transform.position);
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
		GameSessionManager.Instance.gameStateObservers.Remove(gameStateChangedObserver);
	}

	public float SetMovementSpeed(float movementSpeed)
	{
		var previous = defaultMovementSpeed;
		defaultMovementSpeed = movementSpeed;
		this.movementSpeed = movementSpeed;
		return previous;
	}

	public virtual void Tint(Color color, float duration = 0.15f)
	{
		if (sprite)
		{
			if (colorTintCoroutine != null)
			{
				StopCoroutine(colorTintCoroutine);
			}

			colorTintCoroutine = StartCoroutine(TintSequence(color, duration));
		}
	}

	private IEnumerator TintSequence(Color color, float duration)
	{
		var defaultColor = Color.white;
		sprite.color = color;
		float t = 1;
		while (t > 0)
		{
			yield return null;
			t -= Time.deltaTime / duration;
			sprite.color = Color.Lerp(defaultColor, color, t);
		}

		sprite.color = Color.white;
		colorTintCoroutine = null;
	}

	public void StopTint()
	{
		if (colorTintCoroutine != null)
		{
			StopCoroutine(colorTintCoroutine);
			colorTintCoroutine = null;
			sprite.color = Color.white;
		}
	}

	public virtual void Shake(float duration = 0.15f)
	{
		if (sprite)
		{
			if (shakeCoroutine != null)
			{
				StopCoroutine(shakeCoroutine);
			}

			shakeCoroutine = StartCoroutine(ShakeSequence(duration));
		}
	}

	private IEnumerator ShakeSequence(float duration)
	{
		const float displace = 0.05f;
		var spriteTransform = sprite.transform;
		float t = 0;
		var count = 0;
		var spritePosition = spriteTransform.localPosition;
		var newPositionX = displace;
		spriteTransform.localPosition = new Vector3(newPositionX, spritePosition.y, spritePosition.z);
		while (t < 1)
		{
			yield return new WaitForSeconds(0.025f);
			t += 0.025f / duration;
			spritePosition = spriteTransform.localPosition;
			switch (++count % 4)
			{
				case 0:
					newPositionX = displace;
					break;
				case 2:
					newPositionX = -displace;
					break;
				default:
					newPositionX = 0;
					break;
			}

			spriteTransform.localPosition = new Vector3(newPositionX, spritePosition.y, spritePosition.z);
		}

		spriteTransform.localPosition =
			new Vector3(0, spriteTransform.localPosition.y, spriteTransform.localPosition.z);
		shakeCoroutine = null;
	}

	public void StopShake()
	{
		if (shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
			shakeCoroutine = null;

			var spriteTransform = sprite.transform;
			spriteTransform.localPosition =
				new Vector3(0, spriteTransform.localPosition.y, spriteTransform.localPosition.z);
		}
	}

	public void TurnAround()
	{
		if (sprite)
		{
			sprite.flipX = !sprite.flipX;
		}
	}
	
	public void TurnAround(bool flipX)
	{
		if (sprite)
		{
			sprite.flipX = flipX;
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		Gizmos.DrawCube(CubeLocation(spawnPoint), size);
		if (otherPoints != null && otherPoints.Count > 0)
		{
			foreach (var otherPoint in otherPoints)
			{
				Gizmos.DrawCube(CubeLocation(otherPoint), size);
			}
		}

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
				EditorUtility.SetDirty(unit);
			}

			if (GUILayout.Button("Recalculate Spawn Point Based on Position"))
			{
				var position = Vector2Int.FloorToInt(unit.transform.position);
				unit.spawnPoint = position;
				EditorUtility.SetDirty(unit);
			}
		}
	}
#endif
}