using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;
using UnityEngine.UI;
using SmartLocalization;

public class SettingsUIHandler : MonoBehaviour {
    string _soundVolumeText;
    string _musicVolumeText;
    LanguageManager _langManager;
    bool _isLocalized;
    public TextMesh _settingsPageCaption;
    public TextMesh _soundVolumeLabel;
    public TextMesh _musicVolumeLabel;
    public TextMesh _controllerTypeLabel;
    public TextMesh _oneTouchLabel;
    public TextMesh _likeGamePadLabel;
    public TextMesh _likeGamePadReverseLabel;
    SmartCultureInfo sysLang;
	// Use this for initialization
    void Awake()
    {
        _langManager = LanguageManager.Instance;
    }
	void Start () {
        sysLang = _langManager.GetSupportedSystemLanguage();
        if (_langManager.IsLanguageSupported(sysLang))
            _langManager.ChangeLanguage(sysLang);
        else
            _langManager.ChangeLanguage("en");
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
        if (_isLocalized) return;        
        _soundVolumeText = _langManager.GetTextValue("Settings.SoundVolume");
        _musicVolumeText = _langManager.GetTextValue("Settings.MusicVolume");
        _musicVolumeLabel.text = _musicVolumeText + string.Format(" {0}%", GameSettings.Instance.MusicVolume * 100);
        _soundVolumeLabel.text = _soundVolumeText + string.Format(" {0}%", GameSettings.Instance.SoundsVolume * 100);
        _settingsPageCaption.text = _langManager.GetTextValue("Settings.Title");        
        _controllerTypeLabel.text = _langManager.GetTextValue("Settings.ControlType");
        _oneTouchLabel.text = _langManager.GetTextValue("Settings.ControlOneFinger");
        _likeGamePadLabel.text = _langManager.GetTextValue("Settings.ControlGamePad");
        _likeGamePadReverseLabel.text = _langManager.GetTextValue("Settings.ControlGamePadReverse");
        _isLocalized = true;
	}

    public void soundVolumeChanged(float newValue)
    {
        _musicVolumeLabel.text = _musicVolumeText + string.Format(" {0}%", newValue);
        GameSettings.Instance.MusicVolume = newValue / 100f;
    }

    public void musicVolumeChanged(float newValue)
    {
        _soundVolumeLabel.text = _soundVolumeText + string.Format(" {0}%", newValue);
        GameSettings.Instance.SoundsVolume = newValue / 100f;
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
