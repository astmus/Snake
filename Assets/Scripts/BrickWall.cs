using UnityEngine;
using System.Collections;
using System;

public class BrickWall : MonoBehaviour {

	public Action<Vector3> DestroyCallBack;
	void FixedUpdate()
    {
		if (transform.childCount != 0) return;
		Vector3 position = transform.position;
		Destroy(gameObject);
		if (DestroyCallBack != null)
			DestroyCallBack(position);
    }
	
}
