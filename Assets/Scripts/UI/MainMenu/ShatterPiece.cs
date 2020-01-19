using UnityEngine;

public class ShatterPiece : MonoBehaviour
{
	[SerializeField]
	private Vector2 startingVelocity;
	[SerializeField]
	public float rotation;
	private Vector3 velocity;

	public void ApplyVelocity(float deltaTime)
	{
		velocity += deltaTime * 1500 * (Vector3) startingVelocity;
		transform.position += velocity * deltaTime;
		transform.Rotate(new Vector3(0, 0, rotation));
	}
}