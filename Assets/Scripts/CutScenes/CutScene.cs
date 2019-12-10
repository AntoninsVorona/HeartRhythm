using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class CutScene : ScriptableObject
{
	[HideInInspector]
	public bool dialogueFinished;

	public void StartCutScene()
	{
		GameLogic.Instance.StartCoroutine(CutSceneSequence());
	}

	protected abstract IEnumerator CutSceneSequence();
}