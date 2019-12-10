using System;
using UnityEngine;

[Serializable]
public abstract class DialogueRegistrator : ScriptableObject
{
	public abstract void RegisterDialogueFunctions();
	public abstract void UnregisterDialogueFunctions();
}