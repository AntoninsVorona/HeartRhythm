using UnityEngine;

public class NeedYourClothesBum : Mob
{
	[SerializeField]
	private Sprite naked;

	[SerializeField]
	private Sprite clothed;

	private void Awake()
	{
		ClothesChanged();
		Instance = this;
	}

	public void ClothesChanged()
	{
		if (SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bumIsNaked)
		{
			sprite.sprite = naked;
			talksWhenInteractedWith = true;
		}
		else
		{
			sprite.sprite = clothed;
			talksWhenInteractedWith = false;
		}
	}
	
	public static NeedYourClothesBum Instance { get; private set; }
}