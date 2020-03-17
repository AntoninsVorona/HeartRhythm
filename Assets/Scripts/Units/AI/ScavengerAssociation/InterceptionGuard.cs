using UnityEngine;

public class InterceptionGuard : Mob
{
	[SerializeField]
	private Transform shadow;

	public void Rotate()
	{
		sprite.transform.localPosition = new Vector3(-0.246f, 0.258f, 0);
		sprite.transform.rotation = Quaternion.Euler(0, 0, 90);
		shadow.localPosition = new Vector3(-0.238f, -0.081f, 0);
		shadow.localScale = new Vector3(2.1f, 1, 1);
	}
}