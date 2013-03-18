using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public enum SoundManagerClip
{
    SnakeLevelUp
}

public class SoundManager : MonoBehaviour {

	// Use this for initialization
    public AudioClip _snakeLevelUp;
    void Start () {
        audio.volume = GameSettings.Instance.SoundsVolume;
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
        }
        
    }


}
