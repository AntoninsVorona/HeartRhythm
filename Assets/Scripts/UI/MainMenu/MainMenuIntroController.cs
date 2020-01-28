using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuIntroController : MonoBehaviour
{
	[SerializeField]
	private List<Sprite> stages;

	[SerializeField]
	private Image image;

	[SerializeField]
	private GameObject shatterPieceHolder;

	[SerializeField]
	private List<ShatterPiece> firstWaveShatterPieces;

	[SerializeField]
	private List<ShatterPiece> secondWaveShatterPieces;

	[SerializeField]
	private List<ShatterPiece> thirdWaveShatterPieces;

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
		foreach (var shatterPiece in firstWaveShatterPieces)
		{
			shatterPiece.FlyUp();
		}

		float t = 0;
		while (t < 0.125f)
		{
			yield return null;
			var fixedDeltaTime = Time.fixedDeltaTime;
			t += fixedDeltaTime;
			foreach (var shatterPiece in firstWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}
		}

		foreach (var shatterPiece in secondWaveShatterPieces)
		{
			shatterPiece.FlyUp();
		}

		while (t < 0.25f)
		{
			yield return null;
			var fixedDeltaTime = Time.fixedDeltaTime;
			t += fixedDeltaTime;
			foreach (var shatterPiece in firstWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}

			foreach (var shatterPiece in secondWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}
		}

		foreach (var shatterPiece in thirdWaveShatterPieces)
		{
			shatterPiece.FlyUp();
		}

		while (t < 3f)
		{
			yield return null;
			var fixedDeltaTime = Time.fixedDeltaTime;
			t += fixedDeltaTime;
			foreach (var shatterPiece in firstWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}

			foreach (var shatterPiece in secondWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}

			foreach (var shatterPiece in thirdWaveShatterPieces)
			{
				shatterPiece.ApplyVelocity(fixedDeltaTime);
			}
		}

		shatterPieceHolder.SetActive(false);
	}
}