using UnityEngine;

public class PositionChecker : MonoBehaviour
{
	public Vector2Int position;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		Gizmos.DrawCube(CubeLocation(position), size);

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}
}