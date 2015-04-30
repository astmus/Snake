using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class InformerMessage
{
    public string Message { get; set; }
    public bool LeaveOnScreen { get; set; }
    public bool IsReverse { get; set; }
    public event Action Completed;
    public event Action Start;
    public InformerMessage(string message, bool leaveOnScreen, bool isReverse = false)
    {
        Message = message;
        LeaveOnScreen = leaveOnScreen;
        IsReverse = isReverse;
    }

    public InformerMessage(string message, bool leaveOnScreen, Action StartCallback, bool isReverse = false)
    {
        Message = message;
        LeaveOnScreen = leaveOnScreen;
        IsReverse = isReverse;
        Start += StartCallback;
    }

    public void RaiseStartEvent()
    {
        if (Start != null)
        {
            Start();
            Start = null;
        }
    }

    public void RaiseCompletedEvent()
    {
        if (Completed != null)
        {
            Completed();
            Completed = null;
        }
    }
}

public partial class GameInformer : MonoBehaviour
{

    // Use this for initialization
    TextMesh _label;
    Queue<InformerMessage> _messages;
    bool _isRunning;
    Color _invisible;
    Vector3 _minSize;
    Vector3 _maxSize;
    InformerMessage _currentHandleMessage;
    bool _countDownPriority;
    public SnakeClient _snakeClient; // used only in online game
    public bool Autostart { get; set; }
    void Awake()
    {
        Autostart = true;
        _messages = new Queue<InformerMessage>();
        _label = transform.gameObject.GetComponent<TextMesh>();
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _isRunning = false;
        _minSize = new Vector3(.2f, .2f, .0f);
        _maxSize = new Vector3(.3f, .3f, .3f);
        _countDownPriority = true;
        _invisible = new Color(255, 255, 255, 0);
        GetComponent<Renderer>().material.color = _invisible;
        transform.localScale = _maxSize;
        if (_snakeClient == null) return;
        _snakeClient.GameStatusChanged += OnGameStatusChanged; //// used only in online game
        _snakeClient.CountDownTick += OnCountDownTick; // used only in online game
        _snakeClient.GameOver += OnGameOver;// used only in online game
    }

    public void AddMessage(InformerMessage message)
    {
        _messages.Enqueue(message);
        if (Autostart) Run();
    }

    public void StartDisplayMessages()
    {
        if (Autostart) return;
        Run();
    }

    void OnGameOver(bool winResult)
    {
        //Debug.Log("------------------ win result = "+winResult);
        string res = winResult == true ? "YOU WIN!!!" : "YOU LOSE";
        _messages.Enqueue(new InformerMessage(res, true));
        Run();
    }

    void OnCountDownTick(int seconds)
    {
        if (_countDownPriority)
        {
            _messages.Clear();
            _isRunning = false;
            //iTween.Stop();
            //transform.localScale = _maxSize;
            //renderer.material.color = _invisible;
            //_countDownPriority = false;
        }
        if (seconds == 0)
        {
            _messages.Enqueue(new InformerMessage("START", false));
            _countDownPriority = true;
        }
        else
            _messages.Enqueue(new InformerMessage(seconds.ToString(), true));
        Run();
    }

    void OnGameStatusChanged(GameStatus statusCode)
    {
        //Debug.Log("GameSatus in GameStatInformr == " + statusCode);
        switch (statusCode)
        {
            case GameStatus.Connect:
                _messages.Enqueue(new InformerMessage("connected", true));
                break;
            case GameStatus.Disconnect:
                _messages.Enqueue(new InformerMessage("disconnected", true));
                break;
            case GameStatus.InRoom:
                _messages.Enqueue(new InformerMessage("waiting for opponent", true));
                break;
            case GameStatus.InGame:
                //_messages.Enqueue(new InformerMessage("", false));
                break;
        }
        Run();
    }

    public void Run()
    {
        if (_isRunning) return;
        if (_messages.Count == 0) return;
        _isRunning = true;
        ExecuteAnimation();
    }

    void ExecuteAnimation()
    {        
        _currentHandleMessage = _messages.Dequeue();
        _currentHandleMessage.RaiseStartEvent();
        Vector3 toSize = _currentHandleMessage.IsReverse ? _maxSize : _minSize;
        Vector3 fromSize = _currentHandleMessage.IsReverse ? _minSize : _maxSize;
        Color toColor = _currentHandleMessage.IsReverse ? _invisible : Color.white;
        Color fromColor = _currentHandleMessage.IsReverse ? Color.white : _invisible;
        transform.localScale = fromSize;
        GetComponent<Renderer>().material.color = fromColor;
        _label.text = _currentHandleMessage.Message;
        Hashtable param = iTween.Hash("color", toColor, "time", 1.0f);
        iTween.ColorTo(gameObject, param);
        param = iTween.Hash("scale", toSize, "time", 1.0f, "oncomplete", "OnAnimationComplete");
        iTween.ScaleTo(gameObject, param);        
    }

    void OnAnimationComplete()
    {
        if (_messages.Count > 0)
            ExecuteAnimation();
        else
        {
            if (_currentHandleMessage.LeaveOnScreen == false)
                GetComponent<Renderer>().material.color = _invisible;
            
            _isRunning = false;
        }
        _currentHandleMessage.RaiseCompletedEvent();
    }

    // Update is called once per frame
    void Update()
    {

    }


}

