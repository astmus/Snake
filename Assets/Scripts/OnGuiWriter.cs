using Assets.Scripts;
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
        /*switch (_client.ConnetionStatus)
        {**************
            case GameStatus.Disconnect:
                GUILayout.Label("User Disconnected");
                break;
            case GameStatus.Connect:                
                GUILayout.Label("User connected");                                
                break;
            case GameStatus.InRoom:
                GUILayout.Label("In room wait for apponent");
                break;
            case GameStatus.InGame:
                GUILayout.Label("game started");
                break;
            default:
                break;
        }*/
        GUILayout.Label(debugString);
        GUILayout.EndArea();
	}
}
