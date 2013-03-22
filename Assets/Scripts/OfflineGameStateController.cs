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
        GameStatus = GameStatus.InRoom;
        _informer.Autostart = false;
        _informer.AddMessage(new InformerMessage("3",false,false));
        _informer.AddMessage(new InformerMessage("2", false, false));
        _informer.AddMessage(new InformerMessage("1", false, false));
        var message = new InformerMessage("start", false, false);
        message.Completed += OnMessageCompleted;
        _informer.AddMessage(message);
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
