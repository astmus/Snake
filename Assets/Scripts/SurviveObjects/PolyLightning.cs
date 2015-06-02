using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyLightning : MonoBehaviour {

	public Lightning _lightningPrefab;
	public SoundManager _soundManager;
	private List<GameObject> _targetPoints;

	public List<GameObject> TargetPoints
	{
		get { return _targetPoints; }
		set { _targetPoints = value; }
	}

	bool _shouldUpdate = false;

	void Start()
	{
		_targetPoints = new List<GameObject>();
	}

	void Update()
	{
		if (!_shouldUpdate) return;
		_shouldUpdate = false;
		foreach (GameObject targetPoint in _targetPoints)
		{
			GameObject lightInstance = Instantiate(_lightningPrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
			Lightning light = lightInstance.GetComponent<Lightning>();
			light.TargetObject = targetPoint;
		}
		_targetPoints.Clear();
	}

	public void StartLightningStroke()
	{
		_shouldUpdate = true;
		_soundManager.PlaySound(SoundManagerClip.LightningStrike);
		transform.position = new Vector3(UnityEngine.Random.Range(-15,15),transform.position.y,transform.position.z);
	}
}