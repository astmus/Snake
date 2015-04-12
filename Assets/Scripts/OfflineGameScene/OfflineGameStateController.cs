using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public enum GameDifficult : byte
{
    NotSelected = 0x0000,
    Slow = 0x0001,
    Normal = 0x0002,
    Fast = 0x0003
}

public class OfflineGameStateController : MonoBehaviour
{

    // Use this for initialization
    private GameStatus _gameStatus = GameStatus.Connect;
    public event Action<GameStatus> GameStatusChanged;
    public GameInformer _informer;
    public SoundManager _soundManager;
    public GameStatus GameStatus
    {
        get { return _gameStatus; }
        private set { _gameStatus = value; if (GameStatusChanged != null) GameStatusChanged(value); }
    }

    void Start()
    {
        Time.timeScale = 1f;
        GameStatus = GameStatus.InRoom;
        GameObject.Find("SpeedCanvas").GetComponent<Canvas>().enabled = true;
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
            //string message = "player " + snake.PlayerNumber+" winner";
            string message = "win";
            _informer.AddMessage(new InformerMessage(message,true));
            GameStatus = GameStatus.GameOver;
            Time.timeScale = 0f;
            return true;
        }
        return false;
    }

    public void DiffucltSelected(int difficult)
    {
        GameSettings.Instance.DifficultOfCurrentGame = (GameDifficult)difficult;
        GameObject.Find("SpeedCanvas").GetComponent<Canvas>().enabled = false;
        _informer.Autostart = false;
        _informer.AddMessage(new InformerMessage("3", false, PlayCountdownTickSound));
        _informer.AddMessage(new InformerMessage("2", false, PlayCountdownTickSound));
        _informer.AddMessage(new InformerMessage("1", false, PlayCountdownTickSound));
        var message = new InformerMessage("start", false);
        message.Completed += OnMessageCompleted;
        _informer.AddMessage(message);
        _informer.StartDisplayMessages();
        _informer.Autostart = true;
    }

    public void PlayCountdownTickSound()
    {
        _soundManager.PlaySound(SoundManagerClip.CountDownTick);
    }
    void PlayStartGameSound()
    {
        _soundManager.PlaySound(SoundManagerClip.StartGame);
    }

    void OnPlayAgainButtonPress()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void OnToMenuButtonPress()
    {
        Time.timeScale = 1f;
        Application.LoadLevel((int)GameScene.Menu);
    }

    public void OnMessageCompleted()
    {
        PlayStartGameSound();
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
