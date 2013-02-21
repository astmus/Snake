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
    #endregion

    #region controls locations
    private Rect _positionBackButton;
    private Rect _positionApplyButton;
    private Rect _areaServerAddress;
    private Rect _areaPort;
    private Rect _areaMusicVolume;
    private Rect _areaSoundsVolume;
    #endregion

    #region links to scene controls
    public TextMesh _soundVolumeLabel;
    public TextMesh _musicVolumeLabel;
    #endregion

    void Start()
    {
        _positionBackButton = new Rect(Screen.width * 0.05f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
        _positionApplyButton = new Rect(Screen.width * 0.2f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
        _areaServerAddress = new Rect(Screen.width * 0.15f, Screen.height * 0.175f, Screen.width * 0.3f, Screen.height * 0.04f);
        _areaPort = new Rect(Screen.width * 0.15f, Screen.height * 0.3f, Screen.width * 0.3f, Screen.height * 0.04f);
        _areaMusicVolume = new Rect(Screen.width * 0.15f, Screen.height * 0.42f, Screen.width * 0.3f, Screen.height * 0.05f); ;
        _areaSoundsVolume = new Rect(Screen.width * 0.15f, Screen.height * 0.52f, Screen.width * 0.3f, Screen.height * 0.05f); ;
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
        _soundVolumeLabel.text = string.Format("{0:0} %",(100 * _soundsVolume));
        _musicVolumeLabel.text = string.Format("{0:0} %",(100 * _musicVolume));
    }

    void OnApplyButtonPress()
    {
        GameSettings.Instance.ServerAddress = _serverAddress;
        GameSettings.Instance.Port = _port;
        GameSettings.Instance.MusicVolume = _musicVolume;
        GameSettings.Instance.SoundsVolume = _soundsVolume;
        Application.LoadLevel((int)SnakeScene.Menu);
    }

    void OnBackButtonPress()
    {
        Application.LoadLevel((int)SnakeScene.Menu);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
