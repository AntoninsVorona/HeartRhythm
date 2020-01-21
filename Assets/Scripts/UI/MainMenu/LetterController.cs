using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterController : MonoBehaviour
{
	[Serializable]
	public class LetterWaves : SerializableDictionary<int, LetterList>
	{
	}

	[Serializable]
	public class LetterList
	{
		public List<Letter> letters;
	}

	[SerializeField]
	private LetterWaves letterWaves;

	public Coroutine InitiateFlightSequence(Vector2 point)
	{
		return StartCoroutine(FlightSequence(point));
	}

	private IEnumerator FlightSequence(Vector2 point)
	{
		var letterLists = letterWaves.OrderBy(lw => lw.Key).Select(lw => lw.Value.letters).ToList();
		foreach (var letters in letterLists)
		{
			letters.ForEach(l => l.Vibrate());
		}
		yield return new WaitForSeconds(1f);

		foreach (var letters in letterLists)
		{
			letters.ForEach(l => l.Fly(point));
			yield return new WaitForSeconds(0.25f);
		}

		var lastPart = letterLists.Last().First().parts.First();
		yield return new WaitUntil(() => lastPart.done);
	}
}