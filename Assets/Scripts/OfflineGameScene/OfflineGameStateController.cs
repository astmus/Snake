using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class OfflineGameStateController : MonoBehaviour
{

    // Use this for initialization
    private GameStatus _gameStatus = GameStatus.Connect;
    public event Action<GameStatus> GameStatusChanged;
    public GameInformer _informer;
    public GameStatus GameStatus
    {
        get { return _gameStatus; }
        private set { _gameStatus = value; if (GameStatusChanged != null) GameStatusChanged(value); }
    }

    void Start()
    {
        Time.timeScale = 1f;
        GameStatus = GameStatus.InRoom;
        _informer.Autostart = false;
        _informer.AddMessage(new InformerMessage("3",false,false));
        _informer.AddMessage(new InformerMessage("2", false, false));
        _informer.AddMessage(new InformerMessage("1", false, false));
        var message = new InformerMessage("start", false, false);
        message.Completed += OnMessageCompleted;
        _informer.AddMessage(message);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnGUI()
    {
        /*GUILayout.Label(GameSettings.Instance.OfflineRules.GameType.ToString());
        GUILayout.Label(GameSettings.Instance.OfflineRules.PointsEnabled.ToString());
        GUILayout.Label(GameSettings.Instance.OfflineRules.PointsCountWin.ToString());
        GUILayout.Label(GameSettings.Instance.OfflineRules.SnakeLengthEnabled.ToString());
        GUILayout.Label(GameSettings.Instance.OfflineRules.SnakeLengthWin.ToString());*/
    }
    
    public bool CheckWinRules(SnakeControllerOffline snake)
    {
        if (GameSettings.Instance.OfflineRules.CheckGameWinSituation(snake))
        {
            string message = "player " + snake.PlayerNumber+" winner";
            _informer.AddMessage(new InformerMessage(message,true));
            GameStatus = GameStatus.GameOver;
            Time.timeScale = 0f;
            return true;
        }
        return false;
    }

    void OnMessageCompleted()
    {
        GameStatus = GameStatus.InGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameStatus != GameStatus.InRoom) return;
        _gameStatus = GameStatus.CountDown;
        _informer.StartDisplayMessages();
        _informer.Autostart = true;
    }
}
