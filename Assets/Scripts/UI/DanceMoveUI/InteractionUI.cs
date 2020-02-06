using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
	public Image interactionSymbol;
	public TextMeshProUGUI interactionName;
	public SymbolHolder symbolHolder;

	public void Initialize(Sprite symbol, string name, int symbolCount)
	{
		interactionSymbol.sprite = symbol;
		interactionName.text = name;
		symbolHolder.Initialize(symbolCount);
	}

	public void AddSymbol(Sprite symbol, int index)
	{
		symbolHolder.AddSymbol(symbol, index, false);
	}
}