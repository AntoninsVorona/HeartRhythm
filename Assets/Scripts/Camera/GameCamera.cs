using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;
	
	private void LateUpdate()
	{
		transform.position = Player.Instance.transform.position + offset;
	}
}