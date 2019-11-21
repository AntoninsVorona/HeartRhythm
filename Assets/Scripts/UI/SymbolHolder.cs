using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolHolder : MonoBehaviour
{ 
    [Serializable]
    public class DirectionalArrowDictionary: SerializableDictionary<MovementDirectionUtilities.MovementDirection, Sprite>{}

    [SerializeField]
    private InputSymbol symbolPrefab;
    private readonly List<InputSymbol> symbols = new List<InputSymbol>();
    [SerializeField]
    private DirectionalArrowDictionary arrows;
    [SerializeField]
    private Sprite waitingForInput;

    public void Initialize()
    {
        for (var i = 0; i < PlayerInput.Instance.maxDanceMoveSymbols; i++)
        {
            if (symbols.Count == i)
            {
                var inputSymbol = Instantiate(symbolPrefab, transform);
                symbols.Add(inputSymbol);
            }
            var symbol = symbols[i];
            symbol.Initialize(waitingForInput);
        }

        for (var i = PlayerInput.Instance.maxDanceMoveSymbols; i < symbols.Count; i++)
        {
            var symbol = symbols[i];
            symbol.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public void AddSymbol(MovementDirectionUtilities.MovementDirection movementDirection, int index)
    {
        symbols[index].UpdateSymbol(arrows[movementDirection]);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}