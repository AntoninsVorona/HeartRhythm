using System;
using UnityEngine;

[Serializable]
public class GlobalVariables
{
	[Serializable]
	public class ScavengerAssociationVariables
	{
		public bool interceptionGuardHeartAttack;
		public bool bumIsNaked;

		public ScavengerAssociationVariables(bool interceptionGuardHeartAttack, bool bumIsNaked)
		{
			this.interceptionGuardHeartAttack = interceptionGuardHeartAttack;
			this.bumIsNaked = bumIsNaked;
		}
	}

	public GlobalVariables(bool wearsHeadset, int maxDanceMoveSymbols, ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.wearsHeadset = wearsHeadset;
		this.maxDanceMoveSymbols = maxDanceMoveSymbols;
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public bool wearsHeadset;
	public int maxDanceMoveSymbols;
	public ScavengerAssociationVariables scavengerAssociationVariables;
}