using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolHolder : MonoBehaviour
{
	[SerializeField]
	private InputSymbol symbolPrefab;

	private readonly List<InputSymbol> symbols = new List<InputSymbol>();
	private int lastIndex;

	public void Initialize(int symbolCount, Sprite waitingForInput = null)
	{
		for (var i = 0; i < symbolCount; i++)
		{
			if (symbols.Count == i)
			{
				var inputSymbol = Instantiate(symbolPrefab, transform);
				symbols.Add(inputSymbol);
			}

			var symbol = symbols[i];
			symbol.Initialize(waitingForInput);
			if (symbol is SymbolPlaceholder symbolPlaceholder)
			{
				symbolPlaceholder.SetDecayed();
			}
		}

		for (var i = symbolCount; i < symbols.Count; i++)
		{
			var symbol = symbols[i];
			symbol.gameObject.SetActive(false);
		}

		lastIndex = -1;
	}

	public void AddSymbol(Sprite symbol, int index, bool setDecayed)
	{
		lastIndex = index;
		var inputSymbol = symbols[index];
		inputSymbol.UpdateSymbol(symbol);
		if (setDecayed)
		{
			((SymbolPlaceholder) inputSymbol).SetDecayed();
		}
	}

	public void FillLeftoverSymbols()
	{
		for (var i = lastIndex + 1; i < symbols.Count; i++)
		{
			AddSymbol(null, i, false);
		}
	}
}