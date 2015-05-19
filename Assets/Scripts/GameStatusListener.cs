using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class GameStatusListener : MonoBehaviour {

	// Use this for initialization
    //public SnakeClient _client;

	void Start ()
	{	    
        OfflineGameStateController.GameStatusChanged += OnGameStatusChanged;
	    GetComponent<AudioSource>().volume = GameSettings.Instance.MusicVolume;
		switch(GameSettings.Instance.CurrentGameType)
		{
			case GameType.SinglePlayer:
				Destroy(GameObject.Find("PolyLightning"));
				Destroy(GameObject.Find("WallMaker"));
				break;
			case GameType.Survive:
				break;
		}
	}

	void OnDestroy()
	{
		OfflineGameStateController.GameStatusChanged -= OnGameStatusChanged;
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
