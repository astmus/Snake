using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class OnGuiWriter : MonoBehaviour {

    public SnakeClient _client;
    string debugString;
    private Rect _areaLayout;
	// Use this for initialization
	void Start () {
	   _areaLayout = new Rect(0,0,Screen.width,Screen.height);
	}

    public void DebugString(string message)
    {
        debugString = message;
    }

	// Update is called once per frame
	void OnGUI()
	{
	    GUILayout.BeginArea(_areaLayout);
        switch (_client.ConnectionStatus)
        {
            //case GameStatus.Disconnect:
            //    GUILayout.Label("User Disconnected");
            //    break;
            //case GameStatus.Connect:                
            //    GUILayout.Label("User connected");                                
            //    break;
            //case GameStatus.InRoom:
            //    GUILayout.Label("In room wait for apponent");
            //    break;
            case GameStatus.GameOver:
                GameOverMenuDraw();
                break;
            default:
                break;
        }
        //GUILayout.Label(debugString);
        GUILayout.EndArea();
	}


    void GameOverMenuDraw()
    {
        GUI.BeginGroup(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 100, 105));
        GUI.Box(new Rect(0, 0, 100, 105), "select action");
        if (GUI.Button(new Rect(10, 30, 80, 30), "play again"))
            OnPlayAgainButtonPress();
        if (GUI.Button(new Rect(10, 65, 80, 30), "to menu"))
            OnToMenuButtonPress();
        GUI.EndGroup();
    }

    void OnPlayAgainButtonPress()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void OnToMenuButtonPress()
    {
        Application.LoadLevel((int)SnakeScene.Menu);
    }
}
