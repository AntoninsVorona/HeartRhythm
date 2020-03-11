using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Start Conversation Interaction",
	fileName = "StartConversationInteraction")]
public class StartConversationInteraction : Interaction
{
	public string conversation;

	public override bool ApplyInteraction()
	{
		GameSessionManager.Instance.StartConversation(conversation);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}