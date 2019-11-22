using UnityEngine;

public class SceneObjects : MonoBehaviour
{
	public World currentWorld;
	public MobController currentMobController;

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}