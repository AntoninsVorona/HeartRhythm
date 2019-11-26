using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	private static readonly int FINISH_DANCE_MOVE_TRIGGER = Animator.StringToHash("FinishDanceMove");
	private static readonly int IDLE_TRIGGER = Animator.StringToHash("Idle");
	private Inventory inventory;
	private Unit interactingWithUnit;
	private Animator animator;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		animator = GetComponent<Animator>();
	}

	protected override void Start()
	{
		base.Start();
		Initialize(spawnPoint);
		inventory = GetComponent<Inventory>();
		inventory.Initialize();
	}

	protected override void ForceMove()
	{
		base.ForceMove();
		GameCamera.Instance.ChangeTargetPosition(transform.position, true);
	}

	public void ReceiveInput(MovementDirectionUtilities.MovementDirection movementDirection)
	{
		switch (GameLogic.Instance.playState)
		{
			case GameLogic.PlayState.Basic:
				Move(movementDirection);
				break;
			case GameLogic.PlayState.DanceMove:
				ReceiveDanceMove(movementDirection);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ReceiveDanceMove(MovementDirectionUtilities.MovementDirection movementDirection)
	{
		animator.SetTrigger(movementDirection.ToString());
		PlayerInput.Instance.AddDanceMoveSymbol(movementDirection);
	}

	public void EndDanceMove(bool withAnimation)
	{
		if (withAnimation)
		{
			animator.SetTrigger(FINISH_DANCE_MOVE_TRIGGER);
		}

		GameLogic.Instance.FinishDanceMove(!withAnimation);
	}

	protected override IEnumerator MovementSequence(Vector2Int newPosition)
	{
		PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
		yield return base.MovementSequence(newPosition);
		PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
	}

	protected override IEnumerator CantMoveSequence(Vector2Int newPosition, bool isHorizontal)
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

	public override void Die()
	{
		//TODO Lose Game
	}

	protected override void InteractWithObject(Unit unit)
	{
		interactingWithUnit = unit;
		GameLogic.Instance.StartDanceMove(unit);
	}

	public void ApplyDanceMoveSet(List<MovementDirectionUtilities.MovementDirection> acceptorDanceMoveSet)
	{
		var interaction = interactingWithUnit.InteractionMatches(acceptorDanceMoveSet);
		var restoreInput = true;
		if (interaction != null)
		{
			restoreInput = interaction.ApplyInteraction();
		}

		PlayerInput.Instance.acceptor.IgnoreInput = !restoreInput;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube((Vector3Int) spawnPoint + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
	}

	public void BackToIdleAnimation()
	{
		animator.SetTrigger(IDLE_TRIGGER);
	}

	public (bool pickedUpAll, int amountLeft) PickUpItem(Item item, int amount)
	{
		//TODO Display cant pick up
		if (amount > 0)
		{
			return inventory.PickUpItem(item, amount);
		}

		Debug.LogError("Can't pick up less than or equals to 0 items!");
		
		return (false, 0);
	}

	public bool DropItem(InventorySlot selectedInventorySlot, int amount)
	{
		if (amount > 0)
		{
			var (canSpawn, movementDirection) = GameLogic.Instance.currentSceneObjects.currentObstacleManager.CanSpawnAroundLocation(currentPosition);
			if (canSpawn)
			{
				var item = selectedInventorySlot.itemInside;
				var droppedAll = inventory.DropItem(selectedInventorySlot, amount);
				var location = currentPosition + MovementDirectionUtilities.VectorFromDirection(movementDirection);
				GameLogic.Instance.currentSceneObjects.currentObstacleManager.SpawnItemOnGround(item, amount, location);
				return droppedAll;
			}

			//TODO Display cant drop
			Debug.LogError("No Drop Location!");
			return false;
		}

		Debug.LogError("Can't drop less than or equals to 0 items!");

		return false;
	}

	protected override MonoBehaviour CoroutineStarter()
	{
		return this;
	}

	public void ChangeSlots(InventorySlot draggedInventorySlot, InventorySlot slotHit)
	{
		inventory.ChangeSlots(draggedInventorySlot, slotHit);
	}

	public int ItemsInSlot(InventorySlot slot)
	{
		return inventory.ItemsInSlot(slot);
	}

	public static Player Instance { get; private set; }
}