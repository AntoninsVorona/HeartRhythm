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
		public bool visitedPankPoc;
		public bool visitedBigBoyGang;
		public bool bananaGiven;
		public bool shirtGiven;
		public bool pantsGiven;
		public bool funkyGiven;
		public bool alreadyVisitedMainCharacterRoom;

		public ScavengerAssociationVariables(bool interceptionGuardHeartAttack, bool bumIsNaked, bool visitedCoolDude,
			bool bananaTaken, bool visitedPankPoc, bool visitedBigBoyGang, bool bananaGiven, bool shirtGiven,
			bool pantsGiven, bool funkyGiven, bool alreadyVisitedMainCharacterRoom)
		{
			this.interceptionGuardHeartAttack = interceptionGuardHeartAttack;
			this.bumIsNaked = bumIsNaked;
			this.visitedCoolDude = visitedCoolDude;
			this.bananaTaken = bananaTaken;
			this.visitedPankPoc = visitedPankPoc;
			this.visitedBigBoyGang = visitedBigBoyGang;
			this.bananaGiven = bananaGiven;
			this.shirtGiven = shirtGiven;
			this.pantsGiven = pantsGiven;
			this.funkyGiven = funkyGiven;
			this.alreadyVisitedMainCharacterRoom = alreadyVisitedMainCharacterRoom;
		}
	}

	public GlobalVariables(bool wearsHeadset, int maxDanceMoveSymbols,
		ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.wearsHeadset = wearsHeadset;
		this.maxDanceMoveSymbols = maxDanceMoveSymbols;
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public bool wearsHeadset;
	public int maxDanceMoveSymbols;
	public ScavengerAssociationVariables scavengerAssociationVariables;
}