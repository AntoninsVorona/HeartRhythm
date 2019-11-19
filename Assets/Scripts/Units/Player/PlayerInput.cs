using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public enum WrongInputType
    {
        BeatIsInvalid = 0,
        AlreadyReceivedAnInput = 1
    }

    public struct Acceptor
    {
        public bool PlayerReadyForInput { get; set; }
        public bool BeatIsValid { get; set; }
        public bool ReceivedInputThisTimeFrame { get; set; }
        public bool WaitingForPlayerInput { get; set; }
        public bool FirstBattleInputDone { get; set; }
        public WrongInputType lastWrongInput;

        public bool AcceptInput()
        {
            switch (GameLogic.Instance.CurrentGameState)
            {
                case GameLogic.GameState.Peace:
                    return PlayerReadyForInput;
                case GameLogic.GameState.Fight:
                    if (!BeatIsValid)
                    {
                        lastWrongInput = WrongInputType.BeatIsInvalid;
                        return false;
                    }

                    if (ReceivedInputThisTimeFrame)
                    {
                        lastWrongInput = WrongInputType.AlreadyReceivedAnInput;
                        return false;
                    }

                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [HideInNormalInspector]
    public Acceptor acceptor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        int horizontal;
        int vertical;
        switch (GameLogic.Instance.CurrentGameState)
        {
            case GameLogic.GameState.Peace:
                horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                break;
            case GameLogic.GameState.Fight:
                horizontal = Input.GetButtonDown("Left")
                    ? -1
                    : Input.GetButtonDown("Right")
                        ? 1
                        : 0;
                vertical = Input.GetButtonDown("Down")
                    ? -1
                    : Input.GetButtonDown("Up")
                        ? 1
                        : 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var movementDirection = MovementDirectionUtilities.DirectionFromInput(horizontal, vertical);
        if (movementDirection != MovementDirectionUtilities.MovementDirection.None)
        {
            if (acceptor.AcceptInput())
            {
                Debug.Log($"Time: {AudioManager.Instance.time} | Music: {AudioManager.Instance.musicAudioSource.time}");
                Player.Instance.ReceiveInput(movementDirection);
                
                switch (GameLogic.Instance.CurrentGameState)
                {
                    case GameLogic.GameState.Peace:
                        break;
                    case GameLogic.GameState.Fight:
                        acceptor.ReceivedInputThisTimeFrame = true;
                        acceptor.FirstBattleInputDone = true;
                        AudioManager.Instance.ApplyBeat();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (GameLogic.Instance.CurrentGameState)
                {
                    case GameLogic.GameState.Peace:
                        break;
                    case GameLogic.GameState.Fight:
                        switch (acceptor.lastWrongInput)
                        {
                            case WrongInputType.BeatIsInvalid:
                                Debug.LogError("Beat is invalid!");
                                break;
                            case WrongInputType.AlreadyReceivedAnInput:
                                Debug.LogError("Already received an input during this beat!");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void GameStateChanged()
    {
        var newGameState = GameLogic.Instance.CurrentGameState;
        switch (newGameState)
        {
            case GameLogic.GameState.Peace:
                acceptor.PlayerReadyForInput = true;
                break;
            case GameLogic.GameState.Fight:
                acceptor.BeatIsValid = false;
                acceptor.ReceivedInputThisTimeFrame = false;
                acceptor.WaitingForPlayerInput = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        }
    }

    public void MissedBeat()
    {
        Debug.LogError("Missed the Beat!");
    }

    public static PlayerInput Instance { get; private set; }
}