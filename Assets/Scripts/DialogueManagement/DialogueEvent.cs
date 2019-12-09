using System;
using UnityEngine;

[Serializable]
public abstract class DialogueEvent : ScriptableObject
{
	public abstract void ApplyEvent();
}