using UnityEngine;
using System.Collections;

public class Number : MonoBehaviour {
    static public Vector3 amountSize;
	// Use this for initialization
    static Number()
    {
        amountSize = new Vector3(1.2f, 1.2f, 1.2f);
        //Debug.Log("static const");
    }
	void Start () {
        iTween.MoveBy(gameObject, iTween.Hash("y", 2, "easeType", iTween.EaseType.easeInBack, "loopType", iTween.LoopType.none, "time", .5));
        iTween.ScaleBy(gameObject, iTween.Hash("amount", amountSize, "easeType", iTween.EaseType.linear, "loopType", iTween.LoopType.none, "time", .5));
        iTween.ColorTo(gameObject, iTween.Hash("color", new Color(200, 0, 0, 0), "time", .5, "oncomplete", "OnAnimationComplete"));
        //iTween.FadeTo(gameObject, iTween.Hash("alpha", 0, "time", .5));
	}

    void OnAnimationComplete()
    {
        Destroy(this.gameObject);
        //Debug.Log("Animation complete");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
