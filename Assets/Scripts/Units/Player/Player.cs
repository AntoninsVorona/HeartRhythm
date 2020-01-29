using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	[Serializable]
	public class PlayerData : UnitData
	{
		public Inventory.InventoryData inventoryData;

		public PlayerData(string identifierName, Vector2Int currentPosition,
			Inventory.InventoryData inventoryData) : base(identifierName, currentPosition)
		{
			this.inventoryData = inventoryData;
		}
	}

	[Serializable]
	public class CombatData
	{
		public int startHp = 50;
		public int maxHp = 100;

		private int currentHp;

		public int CurrentHp
		{
			get => currentHp;
			set
			{
				currentHp = value;
				GameUI.Instance.equalizerController.UpdateCurrentHp(currentHp, maxHp);
			}
		}

		public void Heal(int heal)
		{
			var canHeal = maxHp - CurrentHp;
			if (heal > canHeal)
			{
				heal = canHeal;
			}

			CurrentHp += heal;
		}

		public void TakeDamage(int damage)
		{
			var leftOverHp = CurrentHp - damage;
			if (leftOverHp < 0)
			{
				damage = CurrentHp;
			}

			CurrentHp -= damage;
		}
	}

	private static readonly int FINISH_DANCE_MOVE_TRIGGER = Animator.StringToHash("FinishDanceMove");
	private static readonly int IDLE_TRIGGER = Animator.StringToHash("Idle");

	[SerializeField]
	private CombatData combatData;

	private Inventory inventory;

	private Inventory Inventory
	{
		get
		{
			if (!inventory)
			{
				inventory = GetComponent<Inventory>();
			}

			return inventory;
		}
	}

	private Unit interactingWithUnit;

	private Animator animator;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
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
		Inventory.Initialize();
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
		//TODO Get Kicked out
	}

	protected override void InteractWithObject(Unit unit)
	{
		if (unit.talksWhenInteractedWith)
		{
			unit.Talk();
			return;
		}

		if (unit.interactions.Count > 0)
		{
			interactingWithUnit = unit;
			GameLogic.Instance.StartDanceMove(unit);
		}
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

	public void BackToIdleAnimation()
	{
		animator.SetTrigger(IDLE_TRIGGER);
	}

	public (bool pickedUpAll, int amountLeft) PickUpItem(Item item, int amount = 1)
	{
		//TODO Display cant pick up
		if (amount > 0)
		{
			return Inventory.PickUpItem(item, amount);
		}

		Debug.LogError("Can't pick up less than or equals to 0 items!");

		return (false, 0);
	}

	public (bool canPickUpAll, int amount) CanPickUp(Item item, int amount)
	{
		return Inventory.CanPickUpItem(item, amount);
	}

	public bool DropItem(InventorySlot selectedInventorySlot, int amount)
	{
		if (amount > 0)
		{
			var (canSpawn, movementDirection) =
				GameLogic.Instance.currentSceneObjects.currentObstacleManager.CanSpawnAroundLocation(currentPosition);
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

	public void LoseItem(Item item, int amount = 1)
	{
		if (amount > 0)
		{
			Inventory.LoseItem(item, amount);
		}
		else
		{
			Debug.LogError("Can't drop less than or equals to 0 items!");
		}
	}

	public bool UseItem(InventorySlot slot)
	{
		var consumable = (Consumable) slot.itemInside;
		var usedLast = Inventory.UseItem(slot);
		UseItem(consumable);
		return usedLast;
	}

	public void UseItem(Consumable item)
	{
		ApplyItemEffect(item);
	}

	private void ApplyItemEffect(Consumable item)
	{
		item.ApplyEffect();
	}

	protected override MonoBehaviour CoroutineStarter()
	{
		return this;
	}

	public void ChangeSlots(InventorySlot draggedInventorySlot, InventorySlot slotHit)
	{
		Inventory.ChangeSlots(draggedInventorySlot, slotHit);
	}

	public void SplitItem(InventorySlot draggedFrom, InventorySlot draggedTo, int amount)
	{
		Inventory.SplitItem(draggedFrom, draggedTo, amount);
	}

	public int ItemsInSlot(InventorySlot slot)
	{
		return Inventory.ItemsInSlot(slot);
	}

	public void InitializeFightWithEnemyCombatData()
	{
		combatData.CurrentHp = combatData.startHp;
	}

	public int GetMaxHp()
	{
		return combatData.maxHp;
	}

	public float GetCurrentHp()
	{
		return combatData.CurrentHp;
	}

	public void Heal(int heal)
	{
		combatData.Heal(heal);
	}

	public void TakeDamage(int damage)
	{
		combatData.TakeDamage(damage);
		if (combatData.CurrentHp == 0)
		{
			Die();
		}
	}

	public override void Talk(string text = null)
	{
		talkUI.Talk(CoroutineStarter(), text);
	}

	public override void ApplyUnitData(UnitData unitData)
	{
		base.ApplyUnitData(unitData);
		Inventory.LoadData(((PlayerData) unitData)?.inventoryData);
	}

	public override UnitData GetUnitData()
	{
		return new PlayerData(identifierName, currentPosition, Inventory.GetData());
	}

	public static Player Instance { get; private set; }
}