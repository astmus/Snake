using System;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class OnGuiPreloadWriter : MonoBehaviour {

	// Use this for initialization
    private OfflineGameRules _rules = GameSettings.Instance.OfflineRules;
    readonly String [] toolBarItems = new String[]{"single player", "multy player"};
    readonly Rect gameTypeRect = new Rect(Screen.width * 0.07f, Screen.height * 0.2f, Screen.width * 0.3f, Screen.height * 0.08f);
    readonly Rect gameTypeBoxRect = new Rect(Screen.width * 0.06f, Screen.height * 0.13f, Screen.width * 0.32f, Screen.height * 0.16f);
    readonly Rect winRulesBoxRect = new Rect(Screen.width * 0.06f, Screen.height * 0.31f, Screen.width * 0.32f, Screen.height * 0.16f);
    readonly Rect byPointsRect = new Rect(Screen.width * 0.07f, Screen.height * 0.33f, Screen.width * 0.05f, Screen.height * 0.06f);
    private bool showBtn;
    public Texture tex;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.depth = 100;
        GUI.Box(gameTypeBoxRect, "");
        _rules.GameType = (GameType)GUI.Toolbar(gameTypeRect, (int)_rules.GameType, toolBarItems);
        GUI.Box(winRulesBoxRect, "");



        showBtn = GUILayout.Toggle(showBtn,tex);
        if (showBtn)
            GUILayout.Button("Close");
    }
}
