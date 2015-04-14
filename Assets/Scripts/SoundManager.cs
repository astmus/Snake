using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public enum SoundManagerClip
{
    SnakeLevelUp,
    Boom,
    CountDownTick,
    StartGame
}

public class SoundManager : MonoBehaviour {

	// Use this for initialization
    public AudioClip _snakeLevelUp;
    public AudioClip _boom;
    public AudioClip _countDownTick;
    public AudioClip _gameStart;

    private AudioSource _source;
    void Start () {

        _source = GetComponent<AudioSource>();
        _source.volume = GameSettings.Instance.SoundsVolume;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool _isPaused;
    public bool IsPaused
    {
        get { return _isPaused; }
        set
        {
            _isPaused = value;
            print(_isPaused);
            if (_isPaused) _source.audio.Pause(); else _source.audio.UnPause();
        }
    }

    public void PlaySound(SoundManagerClip clip)
    {
        switch (clip)
        {
            case SoundManagerClip.SnakeLevelUp:
                _source.PlayOneShot(_snakeLevelUp);
                break;
            case SoundManagerClip.Boom:
                _source.PlayOneShot(_boom);
                break;
            case SoundManagerClip.CountDownTick:
                _source.PlayOneShot(_countDownTick);
                break;
            case SoundManagerClip.StartGame:
                _source.PlayOneShot(_gameStart);
                break;
        }
        
    }


}
