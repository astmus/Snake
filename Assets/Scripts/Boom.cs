using System;
using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {

	// Use this for initialization
    static SoundManager _soundManager;
    public event Action BoomCompleted;
    void Awake()
    {
        if (_soundManager == null) _soundManager = (SoundManager)GameObject.FindObjectOfType(typeof(SoundManager));
        gameObject.transform.localScale = new Vector3(0.5f,0.25f);
    }

	void Start()
	{
	    
	}

    public void StartAnimation(float animationTime)
    {
        _soundManager.PlaySound(SoundManagerClip.Boom);
        iTween.ScaleTo(gameObject, new Vector3(4, 2), animationTime, "OnAnimationComplete");
        iTween.ShakePosition(gameObject,new Vector3(0.2f,0.2f),animationTime);
    }

    void OnAnimationComplete()
    {
        Destroy(gameObject);
        if (BoomCompleted != null) BoomCompleted();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
