using UnityEngine;
using System.Collections;

public class AboutGuiWriter : MonoBehaviour {

	// Use this for initialization
    private Rect _position;
    void Start()
    {
        _position = new Rect(Screen.width * 0.05f, Screen.height * 0.85f, Screen.width * 0.1f, Screen.height * 0.1f);
    }

    void OnGUI()
    {
        if (GUI.Button(_position, "Back"))
        {
            Application.LoadLevel((int)SnakeScene.Menu);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
