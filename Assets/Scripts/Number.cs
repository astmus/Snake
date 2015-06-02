using UnityEngine;
using System.Collections;

public class Number : MonoBehaviour {
    static public Vector3 amountSize;
    public Vector3 PointLabelPosition { get; set; }
	private bool _isAnimationAllowed = true;
	public bool IsAnimationAllowed
	{
		get { return _isAnimationAllowed; }
		set 
		{ 
			_isAnimationAllowed = value;
			AnimateNumber();
		}
	}

	public string Text 
	{
		get { return GetComponent<TextMesh>().text;}
		set { GetComponent<TextMesh>().text = value; }
	}

    // Use this for initialization
    static Number()
    {
        amountSize = new Vector3(2.2f, 2.2f, 0);
        //Debug.Log("static const");
    }
	void Start () {   
        //iTween.FadeTo(gameObject, iTween.Hash("alpha", 0, "time", .5));
		AnimateNumber();
	}

	void AnimateNumber()
	{
		if (!_isAnimationAllowed) return;
		iTween.MoveBy(gameObject, iTween.Hash("y", 2, "easeType", iTween.EaseType.easeInBack, "loopType", iTween.LoopType.none, "time", .5));
		iTween.ScaleBy(gameObject, iTween.Hash("amount", amountSize, "easeType", iTween.EaseType.linear, "loopType", iTween.LoopType.none, "time", .5));
		iTween.ColorTo(gameObject, iTween.Hash("color", new Color(200, 0, 0, 0), "time", 0.8f, "oncomplete", "OnAnimationComplete"));
	}

    /*public void StartAnimation()
    {
        iTween.MoveBy(gameObject, iTween.Hash("y", 2, "easeType", iTween.EaseType.easeInBack, "loopType", iTween.LoopType.none, "time", .5));
        iTween.ScaleBy(gameObject, iTween.Hash("amount", amountSize, "easeType", iTween.EaseType.linear, "loopType", iTween.LoopType.none, "time", .5));
        iTween.ColorTo(gameObject, iTween.Hash("color", new Color(200, 0, 0, 0), "time", 1f, "oncomplete", "OnAnimationComplete"));
    }*/

    void OnAnimationComplete()
    {
        Destroy(this.gameObject);
        //Debug.Log("Animation complete");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
