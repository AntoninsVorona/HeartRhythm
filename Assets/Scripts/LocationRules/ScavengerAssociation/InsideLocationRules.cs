using UnityEngine;

public class InsideLocationRules : LocationRules
{
	[SerializeField]
	private AllItemsDoneCutScene allItemsDoneCutScene;

	[SerializeField]
	private Mob bigBoy;

	[SerializeField]
	private Mob shirtLess;

	[SerializeField]
	private Mob conknap;

	protected override void Awake()
	{
		base.Awake();
		UpdatePants();
		UpdateShirt();
		CheckAllDone(false);
	}

	private void UpdatePants()
	{
		conknap.animator.SetTrigger(
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.pantsGiven
				? AnimatorUtilities.START_TRIGGER
				: AnimatorUtilities.STOP_TRIGGER
		);
	}

	private void UpdateShirt()
	{
		shirtLess.animator.SetTrigger(
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.shirtGiven
				? AnimatorUtilities.START_TRIGGER
				: AnimatorUtilities.STOP_TRIGGER
		);
	}

	public void PlayBanana()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaGiven = true;
		Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("Banana"));
		GameSessionManager.Instance.StartConversation("PlayBanana");
		CheckAllDone(true);
	}

	public void PlayFunky()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.funkyGiven = true;
		Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("Funky"));
		GameSessionManager.Instance.StartConversation("PlayFunky");
		CheckAllDone(true);
	}

	public void PlayPants()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.pantsGiven = true;
		Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("NeedYourClothesBumPants"));
		GameSessionManager.Instance.StartConversation("PlayPants");
		UpdatePants();
		CheckAllDone(true);
	}

	public void PlayShirt()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.shirtGiven = true;
		Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("NeedYourClothesBumShirt"));
		GameSessionManager.Instance.StartConversation("PlayShirt");
		UpdateShirt();
		CheckAllDone(true);
	}

	public void PlayReminder()
	{
		GameSessionManager.Instance.StartConversation("PlayReminder");
	}

	public void PlayIntro()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedBigBoyGang = true;
		GameSessionManager.Instance.StartConversation("PlayIntro");
	}

	private void CheckAllDone(bool andPlayCutScene)
	{
		if (SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.shirtGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.pantsGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.funkyGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaGiven)
		{
			bigBoy.interactions.Clear();
			bigBoy.headsetLessInteraction = null;
			bigBoy.talksWhenInteractedWith = true;
			if (andPlayCutScene)
			{
				GameSessionManager.Instance.PlayCutScene(allItemsDoneCutScene);
			}
		}
		else
		{
			bigBoy.talksWhenInteractedWith = false;
		}
	}

	protected override void OnBeatDone()
	{
	}

	protected override void OnMoveDone()
	{
	}
}