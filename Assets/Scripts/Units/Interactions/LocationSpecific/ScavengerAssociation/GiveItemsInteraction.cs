using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Give Items Interaction",
	fileName = "GiveItemsInteraction")]
public class GiveItemsInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedPankPoc)
		{
			((InsideLocationRules) LocationRules.Instance).PlayNotVisitedPankPoc();
		}
		else if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedBigBoyGang)
		{
			((InsideLocationRules) LocationRules.Instance).PlayIntro();
		}
		else if (Player.Instance.HasItem("Banana"))
		{
			((InsideLocationRules) LocationRules.Instance).PlayBanana();
		}
		else if (Player.Instance.HasItem("NeedYourClothesBumPants"))
		{
			((InsideLocationRules) LocationRules.Instance).PlayPants();
		}
		else if (Player.Instance.HasItem("NeedYourClothesBumShirt"))
		{
			((InsideLocationRules) LocationRules.Instance).PlayShirt();
		}
		else if (Player.Instance.HasItem("Funky"))
		{
			((InsideLocationRules) LocationRules.Instance).PlayFunky();
		}
		else
		{
			((InsideLocationRules) LocationRules.Instance).PlayReminder();
		}

		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}