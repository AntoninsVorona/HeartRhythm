using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Get Key Cut Scene",
	fileName = "GetKeyCutScene")]
public class GetKeyCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		Player.Instance.LoseItem("Microscheme");
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("HeadsetChestKey"), 1, false);
		var pankPocRoomLocationRules = (PankPocRoomLocationRules) LocationRules.Instance;
		yield return new WaitForSeconds(0.5f);
		yield return Player.Instance.Move(new Vector2Int(0, 0));
		yield return Player.Instance.Move(new Vector2Int(1, 0));
		yield return Player.Instance.Move(new Vector2Int(2, 0));
		yield return Player.Instance.Move(new Vector2Int(3, 0));
		yield return Player.Instance.Move(new Vector2Int(4, 0));
		yield return Player.Instance.Move(new Vector2Int(5, 0));
		yield return Player.Instance.Move(new Vector2Int(5, 1));
		pankPocRoomLocationRules.RotatePankPoc();
		GameSessionManager.Instance.StartConversation("HereIsYourMicroscheme");
		yield return new WaitUntil(() => dialogueFinished);
		GameSessionManager.Instance.CutSceneFinished();
	}
}