using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public BeatController beatController;
    public DanceMoveUI danceMoveUI;
    public Text modeToggler;

    private void Awake()
    {
        Instance = this;
        beatController.Deactivate();
        danceMoveUI.Deactivate();
    }

    private void Start()
    {
        GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
    }

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

    public static GameUI Instance { get; private set; }
}