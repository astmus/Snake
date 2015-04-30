using UnityEngine;
using System.Collections;

public class InputMenuSceneHandler : MonoBehaviour {

	// Use this for initialization
    public TextMesh _userRange;
	void Start () {
        string currentRange = PlayerPrefs.HasKey(SnakeControllerOffline.PLAYER_RANGE_KEY) ? PlayerPrefs.GetString(SnakeControllerOffline.PLAYER_RANGE_KEY) : "worm";
        _userRange.text = currentRange;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
	}
}
