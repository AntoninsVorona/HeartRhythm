using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Kill Interaction", fileName = "KillInteraction")]
public class KillInteraction : Interaction
{
    public override bool ApplyInteraction()
    {
        owner.Die();
        return true;
    }
}