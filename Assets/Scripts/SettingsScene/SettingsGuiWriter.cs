using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class SettingsGuiWriter : MonoBehaviour {

	// Use this for initialization
    private string _serverAddress = GameSettings.Instance.ServerAddress;
    private string _port = GameSettings.Instance.Port;
    private Rect _positionBackButton;
    private Rect _positionApplyButton;
	void Start () {
        _positionBackButton = new Rect(Screen.width * 0.05f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
        _positionApplyButton = new Rect(Screen.width * 0.2f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
	}
	
    void OnGUI()
    {
        if (GUI.Button(_positionBackButton, "Back"))
            OnBackButtonPress();

        if (GUI.Button(_positionApplyButton, "Apply"))
            OnApplyButtonPress();

        _serverAddress = GUI.TextArea(new Rect(100, 80, 200, 25), _serverAddress, 200);
        _port = GUI.TextArea(new Rect(100, 160, 200, 25), _port, 200);
    }

    void OnApplyButtonPress()
    {
        GameSettings.Instance.ServerAddress = _serverAddress;
        GameSettings.Instance.Port = _port;
        Application.LoadLevel((int)SnakeScene.Menu);
    }

    void OnBackButtonPress()
    {
        Application.LoadLevel((int)SnakeScene.Menu);
    }

    // Update is called once per frame
	void Update () {
	
	}
}
