using UnityEngine;

public class SceneObjects : MonoBehaviour
{
	public World currentWorld;
	public MobManager currentMobManager;
	public ObstacleManager currentObstacleManager;

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}