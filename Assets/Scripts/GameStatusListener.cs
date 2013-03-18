using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class GameStatusListener : MonoBehaviour {

	// Use this for initialization
    public SnakeClient _client;
    public WhoIsWhoLabel _whoIsWholabel1;
    public WhoIsWhoLabel _whoIsWhoLabel2;

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
                _whoIsWholabel1.StopAnimation();
                _whoIsWhoLabel2.StopAnimation();
                break;
            case GameStatus.InRoom:
                Debug.Log("OnGameStatusChanged InRoom");
                _whoIsWholabel1.StartAnimation();
                _whoIsWhoLabel2.StartAnimation();
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
