using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ResizeCamera : MonoBehaviour
{
    private float DesignOrthographicSize;
    private float DesignAspect;
    private float DesignWidth;

    public float DesignAspectHeight;
    public float DesignAspectWidth;

    public void Awake()
    {
        float xFactor = Screen.width / 800f;
        float yFactor = Screen.height / 480f;
        Camera.main.rect = new Rect(0, 0, 1, xFactor / yFactor);
        //Debug.Log(Camera.main.rect.height);
    }

    public void Resize()
    {
        float wantedSize = this.DesignWidth / this.GetComponent<Camera>().aspect;
        this.GetComponent<Camera>().orthographicSize = Mathf.Max(wantedSize,
            this.DesignOrthographicSize);
    }
}
