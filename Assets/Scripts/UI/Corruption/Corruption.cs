using UnityEngine;

public class Corruption : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup corruptionTier1;

	[SerializeField]
	private CanvasGroup corruptionTier2;

	[SerializeField]
	private CanvasGroup corruptionTier3;

	public void UpdateCorruption(int corruptionTier)
	{
		switch (corruptionTier)
		{
			case 1:
				corruptionTier1.alpha = 0.25f;
				corruptionTier2.alpha = 0;
				corruptionTier3.alpha = 0;
				break;
			case 2:
				corruptionTier1.alpha = 0.5f;
				corruptionTier2.alpha = 0.25f;
				corruptionTier3.alpha = 0;
				break;
			case 3:
				corruptionTier1.alpha = 0.75f;
				corruptionTier2.alpha = 0.5f;
				corruptionTier3.alpha = 0f;
				break;
			case 4:
				corruptionTier1.alpha = 0.75f;
				corruptionTier2.alpha = 0.75f;
				corruptionTier3.alpha = 0f;
				break;
			case 5:
				corruptionTier1.alpha = 0.75f;
				corruptionTier2.alpha = 0.75f;
				corruptionTier3.alpha = 0.25f;
				break;
			case 6:
				corruptionTier1.alpha = 0.75f;
				corruptionTier2.alpha = 0.75f;
				corruptionTier3.alpha = 0.75f;
				break;
			default:
				corruptionTier1.alpha = 0;
				corruptionTier2.alpha = 0;
				corruptionTier3.alpha = 0;
				break;
		}
	}
}