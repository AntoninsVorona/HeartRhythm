﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Pank Poc Visit Cut Scene",
	fileName = "PankPocVisitCutScene")]
public class PankPocVisitCutScene : CutScene
{
	[SerializeField]
	private List<Vector2Int> playerPositions;

	[SerializeField]
	private List<Vector2Int> pankPositions;

	[SerializeField]
	private List<Vector2Int> pankBackPositions;

	protected override IEnumerator CutSceneSequence()
	{
		yield return new WaitForSeconds(0.5f);
		var pankPocRoomLocationRules = (PankPocRoomLocationRules) LocationRules.Instance;
		pankPocRoomLocationRules.RotatePankPoc();
		yield return new WaitForSeconds(0.5f);
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("PankPocFirstVisit");
		yield return new WaitUntil(() => dialogueFinished);
		var pankDone = false;
		var pankId = -1;
		var playerDone = false;
		var playerId = -1;
		while (!pankDone || !playerDone)
		{
			if (!pankDone)
			{
				if (++pankId < pankPositions.Count)
				{
					pankPocRoomLocationRules.MovePankPoc(pankPositions[pankId]);
				}
				else
				{
					pankDone = true;
				}
			}

			if (!playerDone)
			{
				if (++playerId < playerPositions.Count)
				{
					Player.Instance.Move(playerPositions[playerId]);
				}
				else
				{
					playerDone = true;
				}
			}

			yield return new WaitForSeconds(0.25f);
		}

		pankPocRoomLocationRules.PlayShowHeadset();
		yield return new WaitForSeconds(3); //TODO Animation Length
		pankPocRoomLocationRules.PlayHideHeadset();
		yield return new WaitForSeconds(3); //TODO Animation Length
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("PankPocRequest"); //TODO
		pankId = -1;
		while (true)
		{
			if (++pankId < pankBackPositions.Count)
			{
				pankPocRoomLocationRules.MovePankPoc(pankBackPositions[pankId]);
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				break;
			}
		}

		yield return new WaitUntil(() => dialogueFinished);
		GameSessionManager.Instance.CutSceneFinished();
	}
}