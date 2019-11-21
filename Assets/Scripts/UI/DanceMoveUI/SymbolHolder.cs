using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolHolder : MonoBehaviour
{ 
    [SerializeField]
    private InputSymbol symbolPrefab;
    private readonly List<InputSymbol> symbols = new List<InputSymbol>();

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
        }

        for (var i = symbolCount; i < symbols.Count; i++)
        {
            var symbol = symbols[i];
            symbol.gameObject.SetActive(false);
        }
    }

    public void AddSymbol(Sprite symbol, int index)
    {
        symbols[index].UpdateSymbol(symbol);
    }
}