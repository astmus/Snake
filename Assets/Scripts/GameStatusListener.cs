using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class GameStatusListener : MonoBehaviour {

	// Use this for initialization
    public SnakeClient _client;
	void Start () {
        _client.GameStatusChanged += OnGameStatusChanged;
	    audio.volume = GameSettings.Instance.MusicVolume;
	}

    void OnGameStatusChanged(GameStatus status)
    {
        
        switch (status)
        {
            case GameStatus.InGame:
                audio.Play();
                break;
            case GameStatus.GameOver:
                audio.Stop();
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
