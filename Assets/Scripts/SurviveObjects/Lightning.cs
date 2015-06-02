using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {

	public int NumberOfSegments = 20; // в перспективе стоит менять в зависимости от расстояния до целевого объекта 		
	public float RandomDistantion = 0.5f;
	public float Duration = 0.5f;	
	public float Radius = 1f;					
	public float FrameRate = 20f;			
	private GameObject _targetObject;

	public GameObject TargetObject
	{
		get { return _targetObject; }
		set { _targetObject = value; }
	}

	private Transform _endPoint;
	private Vector2 midPoint;
	private float maxZ;
	private float timer = 0f;
	private LineRenderer lineRenderer;	
	private int vertCount = 0;

	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = false;
		if (_targetObject != null)
			_endPoint = _targetObject.transform;
		transform.LookAt(_endPoint);

		StartCoroutine(AnimateLightning());
		//RenderLightning();
	}

	public void RenderLightning()
	{
		if (_targetObject == null || _endPoint == null)
		{
			Destroy(gameObject);
			return;
		}
		if (vertCount != NumberOfSegments)
		{
			lineRenderer.SetVertexCount(NumberOfSegments);
			vertCount = NumberOfSegments;
		}

		midPoint = Random.insideUnitCircle * Radius;

		maxZ = (_endPoint.position - transform.position).magnitude;

		for(int i=1; i < NumberOfSegments-1; i++)
		{
			float z =((float)i)*(maxZ)/(float)(NumberOfSegments-1);
			float x = -midPoint.x*z*z/(2* maxZ) + z*midPoint.x/2f;
			float y = -midPoint.y*z*z/(2* maxZ) + z*midPoint.y/2f;

			lineRenderer.SetPosition(i, new Vector3(x + Random.Range(RandomDistantion,-RandomDistantion) ,y + Random.Range(RandomDistantion,-RandomDistantion),z));
		}
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(NumberOfSegments-1, new Vector3(0,0,maxZ));
	}

	public IEnumerator AnimateLightning()
	{
		while (FrameRate > 0)
		{
			RenderLightning();
			yield return new WaitForSeconds(1f/FrameRate);
		}
	}

	void Update () {
		if (_targetObject == null) Destroy(gameObject);
		transform.LookAt(_endPoint);		

		if (timer < Duration)
		{
			timer += Time.deltaTime;

			if (timer >= Duration)
			{
				BrickWall wall = _targetObject.GetComponent<BrickWall>();
				wall.DestroyByLikeExplosion();
				Destroy(gameObject);
			}
		}
	}

}