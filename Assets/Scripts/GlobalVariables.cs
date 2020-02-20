using System;
using UnityEngine;

[Serializable]
public class GlobalVariables
{
	[Serializable]
	public class ScavengerAssociationVariables
	{
		public bool playerHasToSleepFirst; //TODO Set to false when slept
		public HeadSetHideAndSeekController.HeadSetState headSetState;

		public ScavengerAssociationVariables(bool playerHasToSleepFirst, HeadSetHideAndSeekController.HeadSetState headSetState)
		{
			this.playerHasToSleepFirst = playerHasToSleepFirst;
			this.headSetState = headSetState;
		}
	}

	public GlobalVariables(bool wearsHeadset, ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.wearsHeadset = wearsHeadset;
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public bool wearsHeadset;
	public ScavengerAssociationVariables scavengerAssociationVariables;
}