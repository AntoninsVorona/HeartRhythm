using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private bool followPlayer;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		StartFollowing();
	}

	private void LateUpdate()
	{
		if (followPlayer)
		{
			transform.position = Player.Instance.transform.position + offset;
		}
	}

	public void StartFollowing()
	{
		followPlayer = true;
	}

	public void StopFollowing()
	{
		followPlayer = false;
	}
	
	public static GameCamera Instance { get; private set; }
}