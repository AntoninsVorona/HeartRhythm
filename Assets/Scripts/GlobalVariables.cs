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
		public bool visitedCoolDude;
		public bool bananaTaken;

		public ScavengerAssociationVariables(bool interceptionGuardHeartAttack, bool bumIsNaked, bool visitedCoolDude, bool bananaTaken)
		{
			this.interceptionGuardHeartAttack = interceptionGuardHeartAttack;
			this.bumIsNaked = bumIsNaked;
			this.visitedCoolDude = visitedCoolDude;
			this.bananaTaken = bananaTaken;
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