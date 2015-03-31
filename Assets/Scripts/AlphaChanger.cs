using UnityEngine;
using System.Collections;

public class AlphaChanger : MonoBehaviour {

    private SpriteRenderer _sprite;
    private float _startTime;
    void Start()
    {
        _sprite = gameObject.GetComponent<SpriteRenderer>();
        _startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.realtimeSinceStartup - _startTime;
        if (elapsedTime > 1.5f) return;
        Color cc = _sprite.material.color;
        _sprite.material.color = new Color(cc.r,cc.g,cc.b, 1 - elapsedTime / 1.5f);
    }
}
