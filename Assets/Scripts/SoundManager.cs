using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public enum SoundManagerClip
{
    SnakeLevelUp,
    Boom,
    CountDownTick,
    StartGame,
	ColideWithBrickWall,
	LightningStrike
}

public class SoundManager : MonoBehaviour {

	// Use this for initialization
    public AudioClip _snakeLevelUp;
    public AudioClip _boom;
    public AudioClip _countDownTick;
    public AudioClip _gameStart;
	public AudioClip _brickWallColided;
	public AudioClip _lightningStrike;

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
            AudioListener.pause = _isPaused;
        }
    }

	public void Stop()
	{
		_source.Stop();
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
			case SoundManagerClip.ColideWithBrickWall:
				_source.PlayOneShot(_brickWallColided);
				break;
			case SoundManagerClip.LightningStrike:
				_source.PlayOneShot(_lightningStrike);
				break;
        }
        
    }


}
