using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Attack Or Sync Cut Scene",
	fileName = "AttackOrSyncCutScene")]
public class AttackOrSyncCutScene : CutScene
{
	public LevelData runawayForest;

	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		GameCamera.Instance.Shake(2);
		AudioManager.Instance.StopBeat();
		GameUI.Instance.BackToRealWorld();
		yield return new WaitForSeconds(2);
		yield return GameSessionManager.Instance.BackToRealWorld();
		GameSessionManager.Instance.StartConversation("AfterInterceptionGuard");
		yield return new WaitUntil(() => dialogueFinished);
		GameUI.Instance.FadeAlpha(0, 1);
		GameCamera.Instance.staticView = true;
		Player.Instance.Move(new Vector2Int(21, -3));
		yield return new WaitForSeconds(0.25f);
		Player.Instance.Move(new Vector2Int(21, -2));
		yield return new WaitForSeconds(0.25f);
		Player.Instance.Move(new Vector2Int(22, -2));
		yield return new WaitForSeconds(0.25f);
		Player.Instance.Move(new Vector2Int(22, -1));
		yield return new WaitForSeconds(0.25f);
		yield return GameSessionManager.Instance.LoadLevel(runawayForest, 0);
		GameCamera.Instance.ChangeTargetPosition(new Vector3(3.5f, 8.5f));
		GameCamera.Instance.staticView = true;
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