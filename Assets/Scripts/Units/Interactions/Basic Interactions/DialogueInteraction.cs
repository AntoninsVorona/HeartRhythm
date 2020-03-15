using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Dialogue Interaction",
	fileName = "DialogueInteraction")]
public class DialogueInteraction : Interaction
{
	public string conversationTitle;
	
	public override bool ApplyInteraction()
	{
		GameSessionManager.Instance.StartConversation(conversationTitle, owner.transform);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}