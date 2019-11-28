using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SplitController : MonoBehaviour
{
	private const string SPLIT_AMOUNT_TEXT = "Split: {0}";
	
	private InventorySlot splitFrom;
	private InventorySlot splitTo;

	[SerializeField]
	private GameObject inputSliderHolder;

	[SerializeField]
	private Slider inputSlider;

	[SerializeField]
	private TextMeshProUGUI splitAmount;

	[HideInNormalInspector]
	public bool splitInProgress;

	public void Show(InventorySlot splitFrom, InventorySlot splitTo)
	{
		splitInProgress = true;
		this.splitFrom = splitFrom;
		this.splitTo = splitTo;
		var maxSplit = Player.Instance.ItemsInSlot(splitFrom) - 1;
		inputSlider.maxValue = maxSplit;
		inputSlider.value = Mathf.RoundToInt(((float) maxSplit + 1) / 2);
		inputSliderHolder.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(inputSlider.gameObject);
	}

	public void Close()
	{
		splitInProgress = false;
		inputSliderHolder.SetActive(false);
	}

	public void ApplyInput()
	{
		ApplyInput(Mathf.RoundToInt(inputSlider.value));
	}

	public void ApplyInput(int input)
	{
		GameUI.Instance.uiInventory.SplitItem(splitFrom, splitTo, input);
		Close();
	}
	
	public void OnValueChanged(float value)
	{
		splitAmount.text = string.Format(SPLIT_AMOUNT_TEXT, value);
	}

	public void CancelSplit()
	{
		Close();
	}
}