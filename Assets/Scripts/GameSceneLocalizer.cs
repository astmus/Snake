using UnityEngine;
using System.Collections;
using SmartLocalization;

public class GameSceneLocalizer : MonoBehaviour {

    LanguageManager _langManager;
    bool _isLocalized;
    public UnityEngine.UI.Text _slowBottonText;
    public UnityEngine.UI.Text _normalButton;
    public UnityEngine.UI.Text _fastBottonText;
    public UnityEngine.UI.Text _toMenuButtonText;
    public UnityEngine.UI.Text _exitBottonText;
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
        _slowBottonText.text = _langManager.GetTextValue("Game.Slow");
        _normalButton.text = _langManager.GetTextValue("Game.Normal");
        _fastBottonText.text = _langManager.GetTextValue("Game.Fast");
        _toMenuButtonText.text = _langManager.GetTextValue("Game.ToMenu");
        _exitBottonText.text = _langManager.GetTextValue("Game.Exit");
        _isLocalized = true;
	}
}
