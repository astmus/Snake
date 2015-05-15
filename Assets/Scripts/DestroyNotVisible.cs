using UnityEngine;
using System.Collections;

public class DestroyNotVisible : MonoBehaviour {

	// Use this for initialization
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
