using UnityEngine;
using System.Collections;

public class OTSpriteAlphaChanger : MonoBehaviour {

	// Use this for initialization
    private OTSprite _sprite;
    private float _startTime;
	void Start ()
	{
	    _sprite = gameObject.GetComponent<OTSprite>();
	    _startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float elapsedTime = Time.realtimeSinceStartup - _startTime;
	    if (elapsedTime > 1.5f) return;
	    _sprite.alpha = 1 - elapsedTime/1.5f;
	}
}
