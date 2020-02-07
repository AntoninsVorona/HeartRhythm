using UnityEngine;
using UnityEngine.UI;

public class SymbolPlaceholder : InputSymbol
{
	[SerializeField]
	private Image noSymbolImage;

	[SerializeField]
	private Image heartImage;

	[SerializeField]
	private Image decayedHeartImage;

	[SerializeField]
	private Animator hopAnimator;

	public override void UpdateSymbol(Sprite symbol)
	{
		if (symbol == null)
		{
			noSymbolImage.gameObject.SetActive(true);
			hopAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
			heartImage.gameObject.SetActive(false);
			decayedHeartImage.gameObject.SetActive(false);
			symbolImage.gameObject.SetActive(false);
		}
		else
		{
			noSymbolImage.gameObject.SetActive(false);
			hopAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
			heartImage.gameObject.SetActive(true);
			symbolImage.gameObject.SetActive(true);
			symbolImage.sprite = symbol;
		}

		decayedHeartImage.gameObject.SetActive(false);
	}

	public void SetDecayed()
	{
		heartImage.gameObject.SetActive(false);
		decayedHeartImage.gameObject.SetActive(true);
	}
}