using UnityEngine;
using System.Collections;
using System;

public class RateStarHandler : MonoBehaviour {

	// Use this for initialization

    public static event Action RateStarPressed;
    const string RATE_COMPLETED = "RATE_KEY";

	void Start () {
        if (!PlayerPrefs.HasKey(RATE_COMPLETED))
            iTween.RotateAdd(gameObject, iTween.Hash("y", 180, "time", 1, "looptype", iTween.LoopType.loop, "easetype",iTween.EaseType.linear,"name","rotor"));
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnMouseUp()
    {        
        iTween.StopByName("rotor");
        gameObject.GetComponent<Transform>().rotation = Quaternion.identity;
        if (RateStarPressed != null)
            RateStarPressed();
        PlayerPrefs.SetInt(RATE_COMPLETED, 1);
        //()=>{NavigateToNextScene(_itemAction);}
    }
}
