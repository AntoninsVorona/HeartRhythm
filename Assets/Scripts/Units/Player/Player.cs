using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Player : Unit
{
	[Serializable]
	public class PlayerData : UnitData
	{
		public Inventory.InventoryData inventoryData;

		public PlayerData(string identifierName, Vector2Int currentPosition, Inventory.InventoryData inventoryData) :
			base(identifierName, currentPosition)
		{
			this.inventoryData = inventoryData;
		}
	}

	[Serializable]
	public class CombatData
	{
		public int MaxHp { get; }

		private int successfulBeatsInARow;

		public int SuccessfulBeatsInARow
		{
			get => successfulBeatsInARow;
			set
			{
				if (value == GameSessionManager.Instance.currentBattleSettings.hitsInARowToHeal)
				{
					successfulBeatsInARow = 0;
					Instance.Heal(1);
				}
				else
				{
					successfulBeatsInARow = value;
				}
			}
		}

		private int currentHp;

		public int CurrentHp
		{
			get => currentHp;
			set
			{
				currentHp = value;
				GameUI.Instance.equalizerController.UpdateCurrentHp(currentHp, MaxHp);
			}
		}

		public void Heal(int heal)
		{
			var canHeal = MaxHp - CurrentHp;
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

		public CombatData(int startHp, int maxHp)
		{
			MaxHp = maxHp;
			CurrentHp = startHp;
			SuccessfulBeatsInARow = 0;
		}
	}

	private static readonly int HEADSET_BOOL = Animator.StringToHash("Headset");
	private static readonly int FINISH_DANCE_MOVE_TRIGGER = Animator.StringToHash("FinishDanceMove");
	private static readonly int HANDS_OF_OUT_JACKET_TRIGGER = Animator.StringToHash("HandsOfOutJacket");
	private static readonly int TAKE_SOMETHING_TRIGGER = Animator.StringToHash("TakeSomething");

	[SerializeField]
	private SpriteRenderer gameOverSquare;

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

	public Vector3 SpriteOffset => sprite.transform.localPosition;

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
		gameOverSquare.gameObject.SetActive(false);
	}

	protected override void ForceMove()
	{
		base.ForceMove();
		GameCamera.Instance.ChangeTargetPosition(transform.position, true);
	}

	public void ReceiveInput(MovementDirectionUtilities.MovementDirection movementDirection)
	{
		switch (GameSessionManager.Instance.playState)
		{
			case GameSessionManager.PlayState.Basic:
				Move(movementDirection);
				break;
			case GameSessionManager.PlayState.DanceMove:
				ReceiveDanceMove(movementDirection);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	protected override Coroutine SuccessfulMove(Vector2Int newPosition, bool force)
	{
		if (!force && GameSessionManager.Instance.FightingAnEnemy())
		{
			combatData.SuccessfulBeatsInARow++;
		}

		var successfulMove = base.SuccessfulMove(newPosition, force);
		GameSessionManager.Instance.currentSceneObjects.ApplyPlayerMoved();
		return successfulMove;
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

		GameUI.Instance.danceMoveUI.FillLeftoverSymbols();
		GameSessionManager.Instance.FinishDanceMove(!withAnimation);
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
		bool jump, float jumpStart, bool updateCamera = true)
	{
		base.CharacterMovement(movementT, characterDisplaceT, start, end, jump, jumpStart, updateCamera);
		if (updateCamera)
		{
			GameCamera.Instance.ChangeTargetPosition(transform.position);
		}
	}

	public override void Die()
	{
		GameSessionManager.Instance.PlayerDead();
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = true;
	}

	public void ActivateGameOverSquare()
	{
		gameOverSquare.gameObject.SetActive(true);
		sprite.sortingOrder = 10001;
		animator.SetTrigger(AnimatorUtilities.DIE_TRIGGER);
	}

	protected override void InteractWithObject(Unit unit)
	{
		if (unit.talksWhenInteractedWith)
		{
			unit.Talk();
			return;
		}

		if (SaveSystem.currentGameSave.globalVariables.wearsHeadset)
		{
			if (unit.interactions.Count > 0)
			{
				interactingWithUnit = unit;
				if (unit.startsConversationUponInteraction)
				{
					GameSessionManager.Instance.StartConversation(unit.conversationToStart);
				}

				GameSessionManager.Instance.StartDanceMove(unit);
			}
		}
		else if (unit.headsetLessInteraction)
		{
			interactingWithUnit = unit;
			PlayerInput.Instance.acceptor.IgnoreInput = true;
			var restoreInput = unit.headsetLessInteraction.ApplyInteraction();
			PlayerInput.Instance.acceptor.IgnoreInput = !restoreInput;
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
		animator.SetTrigger(AnimatorUtilities.IDLE_TRIGGER);
	}

	public (bool pickedUpAll, int amountLeft) PickUpItem(Item item, int amount = 1)
	{
		if (amount > 0)
		{
			PerformTakeAnimation();
			var pickUpItem = Inventory.PickUpItem(item, amount);

			if (!pickUpItem.pickedUpAll)
			{
				GameUI.Instance.messageBox.Show("Can't pick up!");
			}

			return pickUpItem;
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
				GameSessionManager.Instance.currentSceneObjects.currentObstacleManager.CanSpawnAroundLocation(
					currentPosition);
			if (canSpawn)
			{
				var item = selectedInventorySlot.itemInside;
				var droppedAll = inventory.DropItem(selectedInventorySlot, amount);
				var location = currentPosition + MovementDirectionUtilities.VectorFromDirection(movementDirection);
				GameSessionManager.Instance.currentSceneObjects.currentObstacleManager.SpawnItemOnGround(item, amount,
					location);
				PerformTakeAnimation();
				return droppedAll;
			}

			GameUI.Instance.messageBox.Show("Can't drop!");
			Debug.LogError("No Drop Location!");
			return false;
		}

		Debug.LogError("Can't drop less than or equals to 0 items!");

		return false;
	}

	private void PerformTakeAnimation()
	{
		PlayerInput.Instance.acceptor.PerformingAnimation = true;
		animator.SetTrigger(TAKE_SOMETHING_TRIGGER);
	}

	public void TakeAnimationDone()
	{
		PlayerInput.Instance.acceptor.PerformingAnimation = false;
	}

	public void LoseItem(string itemName, int amount = 1)
	{
		LoseItem(ItemManager.Instance.GetItemByName(itemName), amount);
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
	
	public bool HasItem(string itemName)
	{
		return Inventory.HasItem(itemName);
	}

	public void InitializeFightWithEnemyCombatData(BattleArea.BattleSettings battleSettings)
	{
		combatData = new CombatData(battleSettings.startingHp, battleSettings.maxHp);
	}

	public int GetMaxHp()
	{
		return combatData.MaxHp;
	}

	public int GetCurrentHp()
	{
		return combatData.CurrentHp;
	}

	public void Heal(int heal)
	{
		if (heal > 0)
		{
			combatData.Heal(heal);
			TintGreen();
		}

		void TintGreen()
		{
			Tint(GOOD_EFFECT_COLOR);
		}
	}

	public void TakeDamage(int damage)
	{
		if (combatData.CurrentHp > 0 && damage > 0)
		{
			combatData.TakeDamage(damage);
			if (combatData.CurrentHp == 0)
			{
				Die();
			}
			else
			{
				// GameUI.Instance.equalizerController.Shake(equalizerShake);
				ShakeAndTintRed();
				combatData.SuccessfulBeatsInARow = 0;
			}
		}

		void ShakeAndTintRed()
		{
			Tint(BAD_EFFECT_COLOR);
			Shake();
		}
	}

	public void MissedBeat()
	{
		if (GameSessionManager.Instance.playState == GameSessionManager.PlayState.DanceMove)
		{
			ReceiveInput(MovementDirectionUtilities.MovementDirection.None);
		}

		if (GameSessionManager.Instance.FightingAnEnemy())
		{
			if (GameSessionManager.Instance.currentBattleSettings.missedBeatDamage.damage > 0)
			{
				TakeDamage(GameSessionManager.Instance.currentBattleSettings.missedBeatDamage.damage);
			}

			combatData.SuccessfulBeatsInARow = 0;
		}
	}

	public void InvalidInputTime()
	{
		if (GameSessionManager.Instance.FightingAnEnemy())
		{
			if (GameSessionManager.Instance.currentBattleSettings.invalidInputDamage.damage > 0)
			{
				TakeDamage(GameSessionManager.Instance.currentBattleSettings.invalidInputDamage.damage);
			}

			combatData.SuccessfulBeatsInARow = 0;
		}
	}


	public override void Talk(string text = null)
	{
		talkUI.Talk(CoroutineStarter(), text);
	}

	public override void ApplyUnitData(UnitData unitData)
	{
		base.ApplyUnitData(unitData);
		GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd
			.AddListener(t => GameSessionManager.Instance.EndConversation());
		var playerData = (PlayerData) unitData;
		Inventory.LoadData(playerData.inventoryData);
		Inventory.Initialize();
		UpdateHeadset();
		animator.SetTrigger(AnimatorUtilities.IDLE_TRIGGER);
	}

	public override UnitData GetUnitData()
	{
		return new PlayerData(identifierName, currentPosition, Inventory.GetData());
	}

	protected override bool ApplyDataOnStart()
	{
		return false;
	}

	protected override void PeaceState()
	{
		base.PeaceState();
		animator.SetBool(AnimatorUtilities.PIECE_BOOL, true);
		animator.SetTrigger(AnimatorUtilities.IDLE_TRIGGER);
	}

	protected override void FightState()
	{
		base.PeaceState();
		animator.SetBool(AnimatorUtilities.PIECE_BOOL, false);
		animator.SetTrigger(HANDS_OF_OUT_JACKET_TRIGGER);
	}

	public void PlayAnimation(string trigger)
	{
		animator.SetTrigger(trigger);
	}

	public void UpdateHeadset()
	{
		animator.SetBool(HEADSET_BOOL, SaveSystem.currentGameSave.globalVariables.wearsHeadset);
	}

	public static Player Instance { get; private set; }
}