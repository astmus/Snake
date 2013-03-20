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
        audio.volume = GameSettings.Instance.SoundsVolume;
        Debug.Log(audio.volume);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlaySound(SoundManagerClip clip)
    {
        switch (clip)
        {
            case SoundManagerClip.SnakeLevelUp:
                audio.PlayOneShot(_snakeLevelUp);
                break;
            case SoundManagerClip.Boom:
                audio.PlayOneShot(_boom);
                break;
        }
        
    }


}
