using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Event", fileName = "Give Item to Player")]
public class GiveItemToPlayerEvent : DialogueEvent
{
	public Item itemToGive;
	public int count = 1;
	
	public override void ApplyEvent()
	{
		Player.Instance.PickUpItem(itemToGive, count);
	}
}