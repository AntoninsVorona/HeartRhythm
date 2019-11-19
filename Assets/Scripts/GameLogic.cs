using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public enum GameState
    {
        Peace = 0,
        Fight = 1
    }

    public GameState CurrentGameState
    {
        get => gameState;
        private set
        {
            if (value != gameState)
            {
                gameState = value;
                GameStateChanged();
            }
        }
    }

    private GameState gameState;
    private Music fightMusic;
    
    [HideInInspector]
    public List<Observer> gameStateObservers = new List<Observer>();

    [Header("Debug")]
    public Music testMusic;
    public bool inputDebugEnabled;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fightMusic = testMusic;
        CurrentGameState = GameState.Peace;
        GameStateChanged();
    }

    private void GameStateChanged()
    {
        switch (CurrentGameState)
        {
            case GameState.Peace:
                AudioManager.Instance.StopBeat();
                break;
            case GameState.Fight:
                AudioManager.Instance.InitializeBattle(fightMusic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        gameStateObservers.ForEach(o => o.Notify());
    }

    public void BeginFight(Enemy enemy)
    {
        fightMusic = enemy.fightMusic;
        CurrentGameState = GameState.Fight;
    }

    public void ToggleMode()
    {
        switch (CurrentGameState)
        {
            case GameState.Peace:
                CurrentGameState = GameState.Fight;
                break;
            case GameState.Fight:
                CurrentGameState = GameState.Peace;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static GameLogic Instance { get; private set; }
}