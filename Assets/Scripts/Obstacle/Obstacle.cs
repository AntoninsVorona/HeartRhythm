using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public virtual void GetDestroyed()
	{
		Destroy(gameObject);
	}
}