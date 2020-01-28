using System;
using UnityEngine;

[Serializable]
public class GlobalVariables
{
	[Serializable]
	public class ScavengerAssociationVariables
	{
		
	}

	public GlobalVariables(ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public ScavengerAssociationVariables scavengerAssociationVariables;
}