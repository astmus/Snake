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
    }

	void Start()
	{
	    
	}

    public void StartAnimation(float animationTime)
    {
        _soundManager.PlaySound(SoundManagerClip.Boom);
		Hashtable param = iTween.Hash("scale", new Vector3(4, 3), "time", animationTime, "oncomplete", "OnAnimationComplete", "ignoretimescale", true);
        iTween.ScaleTo(gameObject, param);
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
