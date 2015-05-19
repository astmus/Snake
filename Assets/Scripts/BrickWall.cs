using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Assets;

public class BrickWall : MonoBehaviour {

	public Action<Vector3> DestroyCallBack;
	public GameObject _flame;
	public GameObject _brickWallPrefab;
	public BrickWallState _currentState;
	
	void FixedUpdate()
    {
		switch(_currentState)
		{
			case BrickWallState.Complex:
				if (transform.childCount != 0) return;
				Vector3 position = transform.position;
				Destroy(gameObject);
				if (DestroyCallBack != null)
				DestroyCallBack(position);
			break;
		}
		
    }

	void OnTriggerEnter2D(Collider2D colliderInfo)
	{
		//if (colliderInfo.gameObject.tag == Assets.SnakeTags.Wall) return;
		switch (colliderInfo.gameObject.tag)
		{
			case SnakeTags.SnakeHead:
				Destroy(gameObject);
				Instantiate(_brickWallPrefab, transform.position, Quaternion.identity);
				break;
		}
	}
	
	public void DestroyByLikeExplosion()
	{
		Destroy(gameObject);
		GameObject go = (GameObject)Instantiate(_brickWallPrefab, transform.position, Quaternion.identity);
		Rigidbody2D[] childBodies = go.GetComponentsInChildren<Rigidbody2D>();		
		GetComponentsInChildren<BoxCollider2D>().ToList().ForEach(fe => fe.enabled = true);
		foreach (Rigidbody2D body in childBodies)
			body.AddTorque(UnityEngine.Random.Range(1,4), ForceMode2D.Impulse);

		//go.GetComponent<ParticleSystem>().enableEmission = true;
		//(Instantiate(_flame, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<ParticleSystem>().enableEmission = true; 
		go.GetComponentsInChildren<Transform>().ToList().ForEach(fe => 
		{
			if (fe == go.transform) return; //оказывается вместе с дочерними компонентами выгребается и собственный, так что... проверяем
			go = Instantiate(_flame, fe.position, Quaternion.LookRotation(Vector3.up)) as GameObject; //делаем огонечек
			go.transform.parent = fe; // и прикрепляем к кирпичику)
		});
		//int pos = UnityEngine.Random.Range(0, childBodies.Length);
	}
}

public enum BrickWallState
{
	Idle, // светим одну суцельную картинку
	Complex // подменяем картинку на кирпичики
}
