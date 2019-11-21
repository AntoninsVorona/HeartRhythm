using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
	public TextMeshProUGUI interactionName;
	public SymbolHolder symbolHolder;

	public void Initialize(string name, int symbolCount)
	{
		interactionName.text = $"{name}:";
		symbolHolder.Initialize(symbolCount);
	}

	public void AddSymbol(Sprite symbol, int index)
	{
		symbolHolder.AddSymbol(symbol, index);
	}
}