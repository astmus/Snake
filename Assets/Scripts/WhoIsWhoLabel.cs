using UnityEngine;
using System.Collections;

public class WhoIsWhoLabel : MonoBehaviour {

	// Use this for initialization
    private Color _invisible = new Color(255, 255, 255, 0);
    private Color _default;
    void Awake()
    {
        Debug.Log("Awake");
        _default = renderer.material.color;
        renderer.material.color = _invisible;
    }

	void Start () {
        //iTween.EaseType.easeInExpo
	}
	
    public void StartAnimation()
    {
        iTween.ColorTo(gameObject, _default, 0.5f);
        iTween.MoveBy(gameObject, iTween.Hash("x", -0.5f, "easeType", "easeOutExpo", "loopType", "pingPong", "speed", 1.5));
    }

    public void StopAnimation()
    {
        iTween.ColorTo(gameObject,_invisible,0.5f,"OnColorToComplete");
    }

    void OnColorToComplete()
    {
        Debug.Log("WhoIsWhoLabel OnColorToComplete run");
        iTween.Stop(gameObject);
    }


	// Update is called once per frame
	void Update () {
	
	}
}
