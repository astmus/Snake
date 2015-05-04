using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class WhoIsWhoLabel : MonoBehaviour {

	// Use this for initialization
    private Color _invisible = new Color(255, 255, 255, 0);
    private Color _default;

    public SnakeClient _client;
    //public OfflineGameStateController _gameStateController;
    void Awake()
    {
        _default = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = _invisible;
        
        OfflineGameStateController.GameStatusChanged += OnGameStatusChanged;
        DontDestroyOnLoad(gameObject);
    }

	void Start () {
        //iTween.EaseType.easeInExpo
	}

    void OnGameStatusChanged(Assets.Scripts.GameStatus status)
    {
        switch (status)
        {
            case GameStatus.InGame:
                StopAnimation();
                break;
            case GameStatus.InRoom:
                StartAnimation();
                break;
        }
    }
	
    public void StartAnimation()
    {
        iTween.ColorTo(gameObject, _default, 0.5f);
        iTween.MoveBy(gameObject, iTween.Hash("x", -0.5f, "easeType", "easeOutExpo", "loopType", "pingPong", "speed", 1.5));
    }

    public void StopAnimation()
    {
        Hashtable param = iTween.Hash("color", _invisible, "time", 0.5f, "oncomplete", "OnColorToComplete");
        iTween.ColorTo(gameObject,param);
    }

    void OnColorToComplete()
    {
        iTween.Stop(gameObject);
    }


	// Update is called once per frame
	void Update () {
	
	}
}
