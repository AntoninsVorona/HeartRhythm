using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public BeatController beatController;
    public Text modeToggler;

    private void Awake()
    {
        Instance = this;
        beatController.Deactivate();
    }

    public static GameUI Instance { get; private set; }

    public void GameStateChanged(GameLogic.GameState currentGameState)
    {
        modeToggler.text = $"{currentGameState} Mode";
        switch (currentGameState)
        {
            case GameLogic.GameState.Peace:
                break;
            case GameLogic.GameState.Fight:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentGameState), currentGameState, null);
        }
    }
}