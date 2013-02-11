using UnityEngine;
using System.Collections;

public class OnGuiWriter : MonoBehaviour {

    public SnakeClient _client;
    string debugString;
	// Use this for initialization
	void Start () {
	
	}

    public void DebugString(string message)
    {
        debugString = message;
    }

	// Update is called once per frame
	void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width *0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.5f));
        switch (_client.ConnetionStatus)
        {
            case ConnectionStatus.Disconnect:
                GUILayout.Label("User Disconnected");
                break;
            case ConnectionStatus.Connect:                
                GUILayout.Label("User connected");                                
                break;
            case ConnectionStatus.InRoom:
                GUILayout.Label("In room wait for apponent");
                break;
            case ConnectionStatus.InGame:
                GUILayout.Label("game started");
                break;
            default:
                break;
        }
        GUILayout.Label(debugString);
        GUILayout.EndArea();
	}
}
