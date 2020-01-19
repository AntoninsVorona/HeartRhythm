using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFadeController : MonoBehaviour
{
	[SerializeField]
	private List<Sprite> stages;

	[SerializeField]
	private Image image;

	[SerializeField]
	private GameObject shatterPieceHolder;

	[SerializeField]
	private List<ShatterPiece> shatterPieces;

	public void Initialize()
	{
		ChangeStage(0);
		image.gameObject.SetActive(true);
		shatterPieceHolder.gameObject.SetActive(false);
	}

	public void ChangeStage(int index)
	{
		if (index == stages.Count)
		{
			Crumble();
			return;
		}

		image.sprite = stages[index];
	}

	public int GetStagesAmount()
	{
		return stages.Count + 1;
	}

	private void Crumble()
	{
		image.gameObject.SetActive(false);
		shatterPieceHolder.gameObject.SetActive(true);
		StartCoroutine(CrumbleSequence());
	}

	private IEnumerator CrumbleSequence()
	{
		foreach (var shatterPiece in shatterPieces)
		{
			shatterPiece.FlyUp();
		}

		float t = 0;
		while (t < 3)
		{
			yield return null;
			var fixedDeltaTime = Time.fixedDeltaTime;
			t += fixedDeltaTime;
			foreach (var shatterPiece in shatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}
		}

		shatterPieceHolder.SetActive(false);
	}
}