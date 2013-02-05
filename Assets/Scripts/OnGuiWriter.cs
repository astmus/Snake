using UnityEngine;
using System.Collections;

public class OnGuiWriter : MonoBehaviour {

    public SnakeClient _client;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width *0.5f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f));
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
        GUILayout.EndArea();
	}
}
