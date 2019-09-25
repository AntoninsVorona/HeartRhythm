using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
	public struct Acceptor
	{
		public bool PlayerReadyForInput { private get; set; }

		public bool AcceptInput()
		{
			return PlayerReadyForInput;
		}
	}

	[HideInNormalInspector]
	public Acceptor acceptor;

	private void Awake()
	{
		Instance = this;
		acceptor = new Acceptor
		{
			PlayerReadyForInput = true
		};
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		
		var horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
		var vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
		if (acceptor.AcceptInput())
		{
			Player.Instance.ReceiveInput(horizontal, vertical);
		}
		else
		{
			if (horizontal != 0 || vertical != 0)
			{
				switch (GameLogic.Instance.CurrentGameState)
				{
					case GameLogic.GameState.Peace:
						break;
					case GameLogic.GameState.Fight:
						Debug.LogError("Can't input right now!");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
	
	public static PlayerInput Instance { get; private set; }
}