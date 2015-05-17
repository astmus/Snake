using UnityEngine;
using System.Collections;
using System.Linq;
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
		GetComponentsInChildren<BoxCollider2D>().ToList().ForEach(fe => fe.enabled = true);
		foreach (Rigidbody2D body in childBodies)
		{
			body.gravityScale = 1;
			body.sleepMode = RigidbodySleepMode2D.NeverSleep;
			body.AddTorque(UnityEngine.Random.Range(3,6), ForceMode2D.Impulse);
		}
		//int pos = UnityEngine.Random.Range(0, childBodies.Length);
	}
}
