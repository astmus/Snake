using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class GameStatusListener : MonoBehaviour {

	// Use this for initialization
    public SnakeClient _client;
    public OfflineGameStateController _gameStatusController;

	void Start ()
	{
	    if (_client != null)
	        _client.GameStatusChanged += OnGameStatusChanged;
	    else
	        _gameStatusController.GameStatusChanged += OnGameStatusChanged;
	    GetComponent<AudioSource>().volume = GameSettings.Instance.MusicVolume;
	}

    void OnGameStatusChanged(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.InGame:
                GetComponent<AudioSource>().Play();
                break;
            case GameStatus.InRoom:
                break;
            case GameStatus.GameOver:
                GetComponent<AudioSource>().Stop();
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
