using UnityEngine;
using System.Collections;
using SmartLocalization;

public class MainMenuLocalizer : MonoBehaviour {

    LanguageManager _langManager;
    bool _isLocalized;
    public TextMesh _classicalGame;
    public TextMesh _settings;
    public TextMesh _exit;
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
	}
	
	// Update is called once per frame
	void Update () {
        if (_isLocalized) return;
        
        _classicalGame.text = _langManager.GetTextValue("MainMenu.ClassicalGame");
        _settings.text = _langManager.GetTextValue("MainMenu.Settings");
        _exit.text = _langManager.GetTextValue("MainMenu.Exit");
        _isLocalized = true;
	}
}
