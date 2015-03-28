using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public enum SoundManagerClip
{
    SnakeLevelUp,
    Boom
}

public class SoundManager : MonoBehaviour {

	// Use this for initialization
    public AudioClip _snakeLevelUp;
    public AudioClip _boom;
    void Start () {
        GetComponent<AudioSource>().volume = GameSettings.Instance.SoundsVolume;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlaySound(SoundManagerClip clip)
    {
        switch (clip)
        {
            case SoundManagerClip.SnakeLevelUp:
                GetComponent<AudioSource>().PlayOneShot(_snakeLevelUp);
                break;
            case SoundManagerClip.Boom:
                GetComponent<AudioSource>().PlayOneShot(_boom);
                break;
        }
        
    }


}
