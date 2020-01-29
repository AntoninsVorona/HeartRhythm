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

	public GlobalVariables(ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public ScavengerAssociationVariables scavengerAssociationVariables;
}