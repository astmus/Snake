using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;
using UnityEngine.UI;

public class SettingsUIHandler : MonoBehaviour {

    public TextMesh _soundVolumeValueText;
    public TextMesh _musicVolumeValueText;
	// Use this for initialization
	void Start () {
        _soundVolumeValueText.text = string.Format("{0}%", GameSettings.Instance.SoundsVolume * 100);
        _musicVolumeValueText.text = string.Format("{0}%", GameSettings.Instance.MusicVolume * 100);
        GameObject.Find("SoundVolumeSlider").GetComponent<Slider>().value = GameSettings.Instance.SoundsVolume * 100;
        GameObject.Find("MusicVolumeSlider").GetComponent<Slider>().value = GameSettings.Instance.MusicVolume * 100;
        switch(GameSettings.Instance.ControllerType)
        {
            case ControllerType.OneTouch:
                GameObject.Find("OneFinger").GetComponent<Toggle>().isOn = true;
                break;
            case ControllerType.TwoTouch:
                GameObject.Find("LikeGamePad").GetComponent<Toggle>().isOn = true;
                break;
            case ControllerType.TwoTouchReverse:
                GameObject.Find("LikeGamePadInverse").GetComponent<Toggle>().isOn = true;
                break;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
            Application.LoadLevel((int)GameScene.Menu);
	}

    public void soundVolumeChanged(float newValue)
    {
        _soundVolumeValueText.text = string.Format("{0}%", newValue);
        GameSettings.Instance.SoundsVolume = newValue / 100f;
    }

    public void musicVolumeChanged(float newValue)
    {
        _musicVolumeValueText.text = string.Format("{0}%", newValue);
        GameSettings.Instance.MusicVolume = newValue / 100f;
    }

    public void ControlTypeOneTouchChanged(bool toggleOn)
    {
        if (toggleOn)
            GameSettings.Instance.ControllerType = ControllerType.OneTouch;
    }

    public void ControlTypeTwoTouchChanged(bool toggleOn)
    {
        if (toggleOn)
            GameSettings.Instance.ControllerType = ControllerType.TwoTouch;
    }

    public void ControlTypeTwoTouchReverseChanged(bool toggleOn)
    {
        if (toggleOn)
            GameSettings.Instance.ControllerType = ControllerType.TwoTouchReverse;
    }
}
