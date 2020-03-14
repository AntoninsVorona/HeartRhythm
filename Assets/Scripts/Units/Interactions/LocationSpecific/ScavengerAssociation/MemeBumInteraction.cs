using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Meme Bum Interaction",
	fileName = "MemeBumInteraction")]
public class MemeBumInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		Player.Instance.Talk("You ok?");
		owner.Talk("Ok.");
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}