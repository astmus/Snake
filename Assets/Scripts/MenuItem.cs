using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MenuItem : MonoBehaviour
{

    public AudioClip _mouseItemOver;
    public AudioClip _mouseItemClick;
    public OTSprite _displaySprite;
    public Texture _itemOverChangeTexture;
    public MenuItemAction _itemAction;

	// Use this for initialization
	void Start ()
	{
        var list = GameObject.FindGameObjectsWithTag("MenuItem").ToList();
	    int delay = list.IndexOf(this.gameObject);
	    audio.volume = GameSettings.Instance.SoundsVolume;
	    StartCoroutine(StartAnimation(delay));
	}
	
    IEnumerator StartAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Hashtable args = new Hashtable();
        args[itWeenParam.X] = 0.5;
        args[itWeenParam.LoopType] = iTween.LoopType.pingPong;
        args[itWeenParam.EaseType] = iTween.EaseType.easeInOutQuad;
        args[itWeenParam.Speed] = 0.25;
        iTween.MoveBy(gameObject, args);
    }

	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        iTween.ColorTo(gameObject, Color.red, 0.3f);
        audio.PlayOneShot(_mouseItemOver);
        _displaySprite.image = _itemOverChangeTexture;
        _displaySprite.size = new Vector2(10,10);
    }

    void OnMouseDown()
    {
        audio.PlayOneShot(_mouseItemClick);
    }

    void OnMouseUp()
    {
        switch (_itemAction)
        {
            case MenuItemAction.Play:
                Application.LoadLevel((int)SnakeScene.Game);
                break;
            case MenuItemAction.Settings:
                Application.LoadLevel((int)SnakeScene.Settings);
                break;
            case MenuItemAction.About:
                Application.LoadLevel((int)SnakeScene.About);
                break;
            case MenuItemAction.Exit:
                Application.Quit();
                break;
        }
        
    }

    void OnMouseExit()
    {
        iTween.ColorTo(gameObject, Color.white,0.3f);
    }
}
