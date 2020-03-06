﻿using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Damaging Guard Cut Scene",
	fileName = "DamagingGuardCutScene")]
public class DamagingGuardCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("InterceptionGuardSeeThatCreature");
		yield return new WaitUntil(() => dialogueFinished);
		var battleRules = (InterceptionGuardBattleRules) BattleRules.Instance;
		yield return new WaitForSeconds((float) AudioManager.Instance.GetTimeUntilNextPulse());
		battleRules.damagingGuard.Move(new Vector2Int(13, 2));
		yield return new WaitForSeconds(0.1f);
		yield return new WaitForSeconds((float) AudioManager.Instance.GetTimeUntilNextPulse());
		battleRules.damagingGuard.Move(new Vector2Int(12, 2));
		yield return new WaitForSeconds(0.1f);
		yield return new WaitForSeconds((float) AudioManager.Instance.GetTimeUntilNextPulse());
		battleRules.damagingGuard.Move(new Vector2Int(11, 2));
		yield return new WaitForSeconds(0.1f);
		yield return new WaitForSeconds((float) AudioManager.Instance.GetTimeUntilNextPulse());
		battleRules.damagingGuard.Move(new Vector2Int(10, 2));
		yield return new WaitForSeconds(0.5f);
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("InterceptionGuardHeDealtCorruption");
		yield return new WaitUntil(() => dialogueFinished);
		GameSessionManager.Instance.CutSceneFinished();
	}
}