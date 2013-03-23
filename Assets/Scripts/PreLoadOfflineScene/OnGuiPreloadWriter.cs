using System;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class OnGuiPreloadWriter : MonoBehaviour
{

    // Use this for initialization
    private OfflineGameRules _rules = GameSettings.Instance.OfflineRules;
    readonly String[] toolBarItems = new String[] { "single player", "multy player" };
    readonly Rect gameTypeRect = new Rect(Screen.width * 0.07f, Screen.height * 0.2f, Screen.width * 0.3f, Screen.height * 0.08f);
    readonly Rect gameTypeBoxRect = new Rect(Screen.width * 0.06f, Screen.height * 0.13f, Screen.width * 0.32f, Screen.height * 0.16f);
    readonly Rect winRulesBoxRect = new Rect(Screen.width * 0.06f, Screen.height * 0.31f, Screen.width * 0.51f, Screen.height * 0.255f);
    readonly Rect byPointsRect = new Rect(Screen.width * 0.07f, Screen.height * 0.38f, Screen.width * 0.49f, Screen.height * 0.08f);
    readonly Rect bySnakeLengthRect = new Rect(Screen.width * 0.07f, Screen.height * 0.47f, Screen.width * 0.49f, Screen.height * 0.0825f);

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.Box(gameTypeBoxRect, "");
        _rules.GameType = (GameType)GUI.Toolbar(gameTypeRect, (int)_rules.GameType, toolBarItems);
        GUI.Box(winRulesBoxRect, "");
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        bool pointsEnabled = _rules.PointsEnabled;
        _rules.PointsCountWin = SpinControl(byPointsRect, _rules.PointsCountWin, 500, 20000,100, ref pointsEnabled, "by points");
        _rules.PointsEnabled = pointsEnabled;
        bool snakeLengthEnabled = _rules.SnakeLengthEnabled;
        _rules.SnakeLengthWin = SpinControl(bySnakeLengthRect, _rules.SnakeLengthWin, 5, 60, 1, ref snakeLengthEnabled, "by length");
        _rules.SnakeLengthEnabled = snakeLengthEnabled;

    }

    int SpinControl(Rect screenRect, int value, int minValue, int maxValue,int step, ref bool enabled, string label)
    {
        GUI.Box(screenRect, "");
        /**/
        // Wrap everything in the designated GUI Area
        GUILayout.BeginArea(screenRect);
        GUILayout.BeginHorizontal();
        enabled = GUILayout.Toggle(enabled, label, GUILayout.MinHeight(screenRect.height),GUILayout.MaxWidth(screenRect.width*0.25f));
        GUI.enabled = enabled;
        if (GUILayout.Button(" - ", GUILayout.MinHeight(screenRect.height)))
            value -= step;
        GUILayout.Label(value.ToString(), GUILayout.MinHeight(screenRect.height), GUILayout.MinWidth(screenRect.width * 0.1f));
        if (GUILayout.Button(" + ", GUILayout.MinHeight(screenRect.height)))
            value += step;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUI.enabled = true;
        return value;
    }
}
