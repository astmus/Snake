using System;
using System.Globalization;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class SettingsGuiWriter : MonoBehaviour
{

    // Use this for initialization
    #region default values
    private string _serverAddress = GameSettings.Instance.ServerAddress;
    private string _port = GameSettings.Instance.Port;
    private float _musicVolume = GameSettings.Instance.MusicVolume;
    private float _soundsVolume = GameSettings.Instance.SoundsVolume;
    private KeyCode _player1Left = GameSettings.Instance.Player1Control.Left;
    private KeyCode _player1Up = GameSettings.Instance.Player1Control.Up;
    private KeyCode _player1Right = GameSettings.Instance.Player1Control.Right;
    private KeyCode _player1Down = GameSettings.Instance.Player1Control.Down;
    private KeyCode _player2Left = GameSettings.Instance.Player2Control.Left;
    private KeyCode _player2Up = GameSettings.Instance.Player2Control.Up;
    private KeyCode _player2Right = GameSettings.Instance.Player2Control.Right;
    private KeyCode _player2Down = GameSettings.Instance.Player2Control.Down;
    #endregion

    #region controls locations
    private Rect _positionBackButton;
    private Rect _positionApplyButton;
    private Rect _areaServerAddress;
    private Rect _areaPort;
    private Rect _areaMusicVolume;
    private Rect _areaSoundsVolume;
    private Rect _palyer1LeftRect;
    private Rect _palyer1UpRect;
    private Rect _palyer1RightRect;
    private Rect _palyer1DownRect;
    private Rect _palyer2LeftRect;
    private Rect _palyer2UpRect;
    private Rect _palyer2RightRect;
    private Rect _palyer2DownRect;
    #endregion

    #region links to scene controls
    public TextMesh _soundVolumeLabel;
    public TextMesh _musicVolumeLabel;
    #endregion

    void Start()
    {
        _positionBackButton = new Rect(Screen.width * 0.05f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
        _positionApplyButton = new Rect(Screen.width * 0.2f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
        _areaServerAddress = new Rect(Screen.width * 0.05f, Screen.height * 0.155f, Screen.width * 0.3f, Screen.height * 0.04f);
        _areaPort = new Rect(Screen.width * 0.05f, Screen.height * 0.25f, Screen.width * 0.3f, Screen.height * 0.04f);
        _areaMusicVolume = new Rect(Screen.width * 0.05f, Screen.height * 0.35f, Screen.width * 0.3f, Screen.height * 0.05f);
        _areaSoundsVolume = new Rect(Screen.width * 0.05f, Screen.height * 0.445f, Screen.width * 0.3f, Screen.height * 0.05f);
        _palyer1LeftRect = new Rect(Screen.width * 0.45f, Screen.height * 0.21f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer1UpRect = new Rect(Screen.width * 0.61f, Screen.height * 0.11f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer1RightRect = new Rect(Screen.width * 0.77f, Screen.height * 0.21f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer1DownRect = new Rect(Screen.width * 0.61f, Screen.height * 0.21f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer2LeftRect = new Rect(Screen.width * 0.45f, Screen.height * 0.5f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer2UpRect = new Rect(Screen.width * 0.61f, Screen.height * 0.4f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer2RightRect = new Rect(Screen.width * 0.77f, Screen.height * 0.5f, Screen.width * 0.15f, Screen.height * 0.09f);
        _palyer2DownRect = new Rect(Screen.width * 0.61f, Screen.height * 0.5f, Screen.width * 0.15f, Screen.height * 0.09f);
    }

    void OnGUI()
    {
        if (GUI.Button(_positionBackButton, "Back"))
            OnBackButtonPress();

        if (GUI.Button(_positionApplyButton, "Apply"))
            OnApplyButtonPress();

        _serverAddress = GUI.TextArea(_areaServerAddress, _serverAddress, 200);
        _port = GUI.TextArea(_areaPort, _port, 200);

        _musicVolume = GUI.HorizontalSlider(_areaMusicVolume, _musicVolume, 0f, 1f);
        _soundsVolume = GUI.HorizontalSlider(_areaSoundsVolume, _soundsVolume, 0f, 1f);
        _soundVolumeLabel.text = string.Format("{0:0} %", (100 * _soundsVolume));
        _musicVolumeLabel.text = string.Format("{0:0} %", (100 * _musicVolume));
        _player1Left = ControllerCode(_palyer1LeftRect, _player1Left, "Left", "player1left");
        _player1Up = ControllerCode(_palyer1UpRect, _player1Up, "Up", "player1right");
        _player1Down = ControllerCode(_palyer1DownRect, _player1Down, "Down", "player1down");
        _player1Right = ControllerCode(_palyer1RightRect, _player1Right, "Right", "player1right");
        _player2Left = ControllerCode(_palyer2LeftRect, _player2Left, "Left", "player2left");
        _player2Up = ControllerCode(_palyer2UpRect, _player2Up, "Up", "player2right");
        _player2Down = ControllerCode(_palyer2DownRect, _player2Down, "Down", "player2down");
        _player2Right = ControllerCode(_palyer2RightRect, _player2Right, "Right", "player2right");
    }

    /// <summary>
    /// return pressed key code for player controling
    /// </summary>
    /// <param name="screenRect">control position</param>
    /// <param name="value">display value</param>
    /// <param name="label">display name</param>
    /// <param name="controlName">control name for check focus</param>
    /// <returns></returns>
    KeyCode ControllerCode(Rect screenRect, KeyCode value, string label, string controlName = "default")
    {
        GUI.Box(screenRect, label);
        screenRect.x += screenRect.width * 0.05f;
        screenRect.width *= 0.9f;
        screenRect.y += screenRect.height * 0.4f;
        screenRect.height *= 0.5f;
        GUI.SetNextControlName(controlName);
        GUI.TextField(screenRect, value.ToString());
        if (Event.current.type == EventType.KeyUp && GUI.GetNameOfFocusedControl() == controlName)
            value = Event.current.keyCode;
        if (_player1Left == value) _player1Left = KeyCode.None;
        if (_player1Up == value) _player1Up = KeyCode.None;
        if (_player1Right == value) _player1Right = KeyCode.None;
        if (_player1Down == value) _player1Down = KeyCode.None;
        if (_player2Left == value) _player2Left = KeyCode.None;
        if (_player2Up == value) _player2Up = KeyCode.None;
        if (_player2Right == value) _player2Right = KeyCode.None;
        if (_player2Down == value) _player2Down = KeyCode.None;  
   
        return value;
    }

    void OnApplyButtonPress()
    {
        GameSettings.Instance.ServerAddress = _serverAddress;
        GameSettings.Instance.Port = _port;
        GameSettings.Instance.MusicVolume = _musicVolume;
        GameSettings.Instance.SoundsVolume = _soundsVolume;
        GameSettings.Instance.Player1Control = new KeyController(_player1Left, _player1Right, _player1Up, _player1Down);
        GameSettings.Instance.Player2Control = new KeyController(_player2Left, _player2Right, _player2Up, _player2Down);
        Application.LoadLevel((int)GameScene.Menu);
    }

    void OnBackButtonPress()
    {
        Application.LoadLevel((int)GameScene.Menu);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
