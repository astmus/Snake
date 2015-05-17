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
	
	public void DestroyByLikeExplosion()
	{
		Rigidbody2D[] childBodies = GetComponentsInChildren<Rigidbody2D>();
		foreach (Rigidbody2D body in childBodies)
		{
			body.gravityScale = 1;
			body.AddTorque(5, ForceMode2D.Impulse);
		}
		//int pos = UnityEngine.Random.Range(0, childBodies.Length);
		
	}
}
