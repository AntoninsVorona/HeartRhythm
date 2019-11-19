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

    private void Start()
    {
        GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
    }

    public static GameUI Instance { get; private set; }

    private void GameStateChanged()
    {
        var currentGameState = GameLogic.Instance.CurrentGameState;
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