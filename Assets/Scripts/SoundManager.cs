using System;
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
