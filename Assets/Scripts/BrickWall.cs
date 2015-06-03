using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Assets;

public class BrickWall : MonoBehaviour {

	public Action<Vector3> DestroyCallBack;
	public GameObject _flame;
	public GameObject _brickWallPrefab;
	public Number _numberPrefab;
	Number _countDownLabel;
	public float CountDownTime {get;set;} //в секундочках
	public BrickWallState _currentState;	

	void FixedUpdate()
    {
		switch(_currentState)
		{
			case BrickWallState.Complex:
				if (transform.childCount != 0) return;				
				Destroy(gameObject);				
			break;
		}
		if (CountDownTime > 0)
		{
			CountDownTime -= Time.deltaTime;
			_countDownLabel.Text = Math.Round(CountDownTime, 2).ToString();
			if (CountDownTime < 0)
			{
				//_countDownLabel.Text = "0";
				_countDownLabel.IsAnimationAllowed = true;
			}
			//else
			//	_countDownLabel.Text = Math.Round(CountDownTime, 2).ToString();
		}
    }

	void Start()
	{
		if (CountDownTime > 0)
		{
			_countDownLabel = (Number)Instantiate(_numberPrefab, new Vector3(transform.position.x - 0.65f, transform.position.y, -18), Quaternion.identity);
			_countDownLabel.IsAnimationAllowed = false;
		}
	}

	public void SwitchFromTransparent()
	{
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<SpriteRenderer>().color = Color.white;
		tag = SnakeTags.BrickWall;
	}
	void OnTriggerEnter2D(Collider2D colliderInfo)
	{
		//if (colliderInfo.gameObject.tag == Assets.SnakeTags.Wall) return;
		switch (colliderInfo.gameObject.tag)
		{
			case SnakeTags.SnakeHead:
				Destroy(gameObject);
				Instantiate(_brickWallPrefab, transform.position, Quaternion.identity);
				if (DestroyCallBack != null)
					DestroyCallBack(transform.position);
				break;
		}
	}
	
	public void DestroyByLikeExplosion()
	{
		Destroy(gameObject);
		if (DestroyCallBack != null)
			DestroyCallBack(transform.position);
		
		GameObject go = (GameObject)Instantiate(_brickWallPrefab, transform.position, Quaternion.identity);
		for (int i = 0; i < go.transform.childCount; ++i)
		{
			GameObject child = go.transform.GetChild(i).gameObject;
			//child.GetComponent<BoxCollider2D>().enabled = true;
			child.GetComponent<TrailRenderer>().enabled = true;
			child.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(1,4), ForceMode2D.Impulse);
		}

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
