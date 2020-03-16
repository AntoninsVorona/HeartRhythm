using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Cool Dude Cut Scene",
	fileName = "CoolDudeCutScene")]
public class CoolDudeCutScene : CutScene
{
	public Vector2 cameraPosition;
	
	protected override IEnumerator CutSceneSequence()
	{
		yield return new WaitForSeconds(0.5f);
		GameCamera.Instance.ChangeTargetPosition(cameraPosition);
		yield return new WaitForSeconds(3);
		GameCamera.Instance.ChangeTargetPosition(Player.Instance.transform.position);
		yield return new WaitForSeconds(1);
		GameSessionManager.Instance.CutSceneFinished();
	}
}