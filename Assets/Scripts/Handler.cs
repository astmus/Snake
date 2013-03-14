using UnityEngine;
using System.Collections;

public class Handler : MonoBehaviour
{
    private string _externalMessage = "External message";
    private readonly Rect _areaLayout = new Rect(0,0,Screen.width,Screen.height);
    void OnGUI()
    {
        GUILayout.BeginArea(_areaLayout);
        GUILayout.Label(_externalMessage);
        GUILayout.EndArea();
    }

    void Start()
    {
        
    }

    void HandleExternalMessage(string message)
    {
        _externalMessage = message;
    }
}
