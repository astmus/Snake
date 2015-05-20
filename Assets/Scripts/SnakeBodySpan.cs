using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetPoint
{
    public TargetPoint()
    {
        Position = new Vector2();
        Rotation = 0;
    }
    public TargetPoint(float rotation, Vector2 position)
    {
        Position = position;
        Rotation = rotation;
    }
    public Vector2 Position { set; get; }
    public float Rotation { set; get; }

}

public class SnakeBodySpan : MonoBehaviour, ISnakePart
{

    GameObject _snakeBody;
    List<TargetPoint> _pointList;
    //OTSprite _sprite;
    ISnakePart _previousPart;
    public Vector2? LastRotatePoint { get; set; }
    static Vector3 _amountDestroy = new Vector3(.5f, .5f); //размер который будте добавлен части червяка при уничтожении
    //float dist = float.MaxValue;
    //public event System.Action<TargetPoint> PartRotate;
    float _rotation;
    public SnakeBodySpan()
    {
        _pointList = new List<TargetPoint>();
       // _snakeBody = span;
        //_sprite = span.GetComponent<OTSprite>();        
    }

    //[Obsolete("old method for compability")]
    public SnakeBodySpan(GameObject span, ISnakePart previousPart)
    {
        _pointList = new List<TargetPoint>();
        _snakeBody = span;
        //_sprite = span.GetComponent<OTSprite>();
        //_sprite.rotation = previousPart.Rotation;
        //_sprite.position = new Vector2(part.Position.x - _sprite.size.x, part.Position.y);
        //_sprite.position = previousPart.Position;
        //_sprite.transform.Translate(-_snakeBody.transform.lossyScale.x, 0, 0);
        _previousPart = previousPart;
       // previousPart.PartRotate += new System.Action<TargetPoint>(OnPartRotate);
    }

    public ISnakePart PreviousPart
    {
        set 
        {
            if (_previousPart == null)
            {
                _previousPart = value;
                Rotation = _previousPart.Rotation;
                //_sprite.position = new Vector2(part.Position.x - _sprite.size.x, part.Position.y);
                transform.position = _previousPart.Position;
                transform.Translate(-/*_snakeBody.*/transform.lossyScale.x, 0, 0);
                //_previousPart.PartRotate += new System.Action<TargetPoint>(OnPartRotate);
            }
        }
    }

    void OnPartRotate(TargetPoint target)
    {
        AddTargetPoint(target);
    }

    public void AddTargetPoint(TargetPoint point)
    {
        _pointList.Add(point);
    }

    public Transform Transform
    {
        get { return transform; }
    }

    float _lastDist = float.MaxValue;
    public void Translate(float x, float y, float z)
    {
        //Vector2 v = Position + new Vector2(x,y);
        transform.Translate(x, y, z);
        //Debug.Log("lossy"+_snakeBody.transform.lossyScale.x);
        //Debug.Log("local"+_snakeBody.transform.localScale.x);
        //if (_pointList.Count != 0)
        //{
        if (!_previousPart.LastRotatePoint.HasValue) return;
        float dist = Vector2.Distance(Position, _previousPart.LastRotatePoint.Value);
        if (dist > _lastDist)
        {
                Rotation = _previousPart.Rotation;
                Position = _previousPart.Position;
                transform.Translate(-/*_snakeBody.*/transform.localScale.x, 0, 0);
                LastRotatePoint = Position;
                _lastDist = float.MaxValue;
                _previousPart.LastRotatePoint = null;
        }
        else
            _lastDist = dist;
        /*else
        {
            if (dist > transform.localScale.x)
            {
                _previousPart.Rotation = Rotation;
                _previousPart.Position = Position;
                transform.Translate(transform.localScale.x, 0, 0);
            }
        }*/
        //else
        //    dist = newDist;
        //}        
        //else dis = 100;   
    }

    public void AnimationDestroy(float time,float delay)
    {
        //itWeenParam.
        //OTSprite
        Hashtable args = new Hashtable();
        args[itWeenParam.Amount] = _amountDestroy;
        args[itWeenParam.Time] = time;
        args[itWeenParam.OnComplete] = "OnDestroyCompleted";
        args[itWeenParam.Delay] = delay;
        iTween.ScaleAdd(this.gameObject, args);
        this.gameObject.AddComponent(typeof(AlphaChanger));
    }

	public void RemoveColiderAndRigibody()
	{
		Destroy(this.gameObject.GetComponent<Rigidbody2D>());
		Destroy(this.gameObject.GetComponent<BoxCollider2D>());
	}

    void OnDestroyCompleted()
    {
        Destroy(gameObject);
    }

    /*public void OnComplete()
    {
        Debug.Log("OnAmountScaleAddDestroy");
        Object.Destroy(_snakeBody);
    }*/

    public float Rotation
    {
        get
        {
            return transform.rotation.eulerAngles.z;
        }
        set
        {
            float val = value;
            // keep this rotation within 0-360
            if (val < 0) val += 360.0f;
            else
                if (val >= 360) val -= 360.0f;

            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, val);
            _rotation = val;
        }
    }

    public Vector2 Position
    {
        set { transform.position = value; }
        get { return transform.position; }
    }

    public GameObject AsGameObject()
    {        
        return this.gameObject;
    }
}
