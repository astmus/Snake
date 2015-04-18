using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MenuItem : MonoBehaviour
{
    public AudioClip _mouseItemClick;    
    public Texture _itemOverChangeTexture;
    public MenuItemAction _itemAction;
    public MenuItemAction ItemAction
    {
        get { return _itemAction; }
    }
	// Use this for initialization
	void Start ()
	{
        var list = GameObject.FindGameObjectsWithTag("MenuItem").ToList();
	    int delay = list.IndexOf(this.gameObject);
	    //GetComponent<AudioSource>().volume = GameSettings.Instance.SoundsVolume;
	    StartCoroutine(StartAnimation(delay));
	}
	
    IEnumerator StartAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Hashtable args = new Hashtable();
        args[itWeenParam.X] = 0.75;
        args[itWeenParam.LoopType] = iTween.LoopType.pingPong;
        args[itWeenParam.EaseType] = iTween.EaseType.linear;
        args[itWeenParam.Speed] = 0.3;
        iTween.MoveBy(gameObject, args);
    }

	// Update is called once per frame
	void Update () {
        int nbTouches = Input.touchCount;

        if (nbTouches > 0)
        {
            for (int i = 0; i < nbTouches; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    return;
                    Ray screenRay = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(screenRay, out hit))
                        if (hit.collider.gameObject.tag == "MenuItem")
                            NavigateToNextScene(hit.collider.gameObject.GetComponent<MenuItem>().ItemAction);
                }

            }
        }
	}

   void OnMouseEnter()
    {
        
        //GetComponent<AudioSource>().PlayOneShot(_mouseItemOver);
        /*_displaySprite.image = _itemOverChangeTexture;
        _displaySprite.size = new Vector2(10,10);*/
    }

    void OnMouseDown()
    {
        //GetComponent<AudioSource>().PlayOneShot(_mouseItemClick);
    }

    void OnMouseUp()
    {
        var param = iTween.Hash("amount", new Vector3(0.5f,0.5f,0.5f), "time", 0.4f, "oncomplete", "NavigateToNextScene", "oncompleteparams", _itemAction);
        iTween.ShakePosition(gameObject, param);
        //()=>{NavigateToNextScene(_itemAction);}
    }

    private void NavigateToNextScene(MenuItemAction action)
    {
        switch (action)
        {
            case MenuItemAction.PlayOffline:
                Application.LoadLevel((int)GameScene.GameOffline);
                break;            
            case MenuItemAction.Settings:
                Application.LoadLevel((int)GameScene.Settings);
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
