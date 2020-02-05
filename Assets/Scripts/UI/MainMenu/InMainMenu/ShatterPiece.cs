using UnityEngine;

public class ShatterPiece : MonoBehaviour
{
	[SerializeField]
	private Vector2 startingVelocity;
	[SerializeField]
	public float rotation;
	private Vector3 velocity;

	public void FlyUp()
	{
		velocity = new Vector3(0, 500, 0);
	}

	public void ApplyVelocity(float deltaTime)
	{
		const float damping = 3f;
		velocity += deltaTime * 1500 * (Vector3) startingVelocity;
		velocity.x -= velocity.x * damping * deltaTime;
		transform.position += velocity * deltaTime;
		transform.Rotate(new Vector3(0, 0, rotation));
	}
}