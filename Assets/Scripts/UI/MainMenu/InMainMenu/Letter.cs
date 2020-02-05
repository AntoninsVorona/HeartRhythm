using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
	public List<LetterPart> parts;

	public void Vibrate()
	{
		foreach (var part in parts)
		{
			part.Vibrate();
		}
	}
	
	public void Fly(Vector2 point)
	{
		StartCoroutine(FlightSequence(point));
	}

	private IEnumerator FlightSequence(Vector2 point)
	{
		foreach (var part in parts)
		{
			part.Fly(point);
			yield return new WaitForSeconds(0.05f);
		}
	}
}