﻿using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Attack Or Sync Cut Scene",
	fileName = "AttackOrSyncCutScene")]
public class AttackOrSyncCutScene : CutScene
{
	public LevelData runawayForest;

	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		if (SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.interceptionGuardHeartAttack)
		{
			//TODO Play Guitar Destroy Animation
			yield return new WaitForSeconds(3); //TODO According to destroy animation
		}
		else
		{
			Player.Instance.PlayAnimation("Headbang");
			yield return new WaitForSeconds(3);
		}

		GameCamera.Instance.Shake(2);
		AudioManager.Instance.StopBeat();
		GameUI.Instance.BackToRealWorld();
		yield return new WaitForSeconds(2);
		yield return GameSessionManager.Instance.BackToRealWorld();
		GameSessionManager.Instance.StartConversation("AfterInterceptionGuard");
		yield return new WaitUntil(() => dialogueFinished);
		yield return Player.Instance.Move(new Vector2Int(11, 0));
		yield return Player.Instance.Move(new Vector2Int(10, 0));
		yield return Player.Instance.Move(new Vector2Int(9, 0));
		yield return Player.Instance.Move(new Vector2Int(8, 0));
		yield return Player.Instance.Move(new Vector2Int(7, 0));
		yield return Player.Instance.Move(new Vector2Int(6, 0));
		yield return Player.Instance.Move(new Vector2Int(5, 0));
		yield return Player.Instance.Move(new Vector2Int(4, 0));
		yield return Player.Instance.Move(new Vector2Int(3, 0));
		yield return Player.Instance.Move(new Vector2Int(2, 0));
		yield return Player.Instance.Move(new Vector2Int(1, 0));
		yield return Player.Instance.Move(new Vector2Int(0, 0));
		yield return Player.Instance.Move(new Vector2Int(0, 1));
		yield return Player.Instance.Move(new Vector2Int(0, 2));
		Player.Instance.Move(new Vector2Int(-31, -32), true);
		var pankPocRoomLocationRules = (PankPocRoomLocationRules) LocationRules.Instance;
		pankPocRoomLocationRules.Talk();
		yield return new WaitForSeconds(3);
		GameUI.Instance.FadeAlpha(0, 1);
		yield return GameSessionManager.Instance.LoadLevel(runawayForest, 0);
		GameCamera.Instance.ChangeTargetPosition(new Vector3(3.5f, 8.5f));
		GameCamera.Instance.staticView = true;
		yield return GameUI.Instance.gameLogo.Show();
		yield return new WaitForSeconds(3);
		yield return GameUI.Instance.gameLogo.Hide();
		GameUI.Instance.ResetFadeAlpha();
		yield return GameUI.Instance.ShowBlackAnnouncer(new BlackAnnouncer.AnnouncementData("Chapter 1", "Darkwoods"));
		yield return GameUI.Instance.CloseBlackAnnouncer();
		var previousMovementSpeed = Player.Instance.SetMovementSpeed(1f);
		Player.Instance.Move(new Vector2Int(0, 1));
		yield return new WaitForSeconds(1.5f);
		Player.Instance.Move(new Vector2Int(0, 2));
		yield return new WaitForSeconds(1.75f);
		Player.Instance.Move(new Vector2Int(0, 3));
		yield return new WaitForSeconds(2f);
		Player.Instance.Move(new Vector2Int(0, 4));
		yield return new WaitForSeconds(2.25f);
		Player.Instance.PlayAnimation("RunawayForestFall");
		yield return new WaitForSeconds(6.5f);
		Player.Instance.SetMovementSpeed(previousMovementSpeed);
		//TODO Play Woodcutter
		GameSessionManager.Instance.CutSceneFinished();
		GameLogic.Instance.Save();
	}
}