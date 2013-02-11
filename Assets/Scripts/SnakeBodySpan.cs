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

public class SnakeBodySpan : ISnakePart
{

    GameObject _snakeBody;
    List<TargetPoint> _pointList;
    OTSprite _sprite;
    ISnakePart _previousPart;
    //float dist = float.MaxValue;
    public event System.Action<TargetPoint> PartRotate;
    public SnakeBodySpan(GameObject span, ISnakePart previousPart)
    {
        _pointList = new List<TargetPoint>();
        _snakeBody = span;
        _sprite = span.GetComponent<OTSprite>();
        _sprite.rotation = previousPart.Rotation;
        //_sprite.position = new Vector2(part.Position.x - _sprite.size.x, part.Position.y);
        _sprite.position = previousPart.Position;
        _sprite.transform.Translate(-_snakeBody.transform.lossyScale.x, 0, 0);
        _previousPart = previousPart;
        previousPart.PartRotate += new System.Action<TargetPoint>(OnPartRotate);
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
        get { return _sprite.transform; }
    }

    public void Translate(float x, float y, float z,params OnGuiWriter [] wr)
    {
        //Vector2 v = Position + new Vector2(x,y);
        _sprite.transform.Translate(x, y, z);
        //Debug.Log("lossy"+_snakeBody.transform.lossyScale.x);
        //Debug.Log("local"+_snakeBody.transform.localScale.x);
        //if (_pointList.Count != 0)
        //{
        float dist = Vector2.Distance(Position, _previousPart.Position);
        if (x > 0 || y > 0)
        {            
            if (dist > _snakeBody.transform.localScale.x)
            {
                Rotation = _previousPart.Rotation;
                Position = _previousPart.Position;
                _sprite.transform.Translate(-_snakeBody.transform.localScale.x, 0, 0);
            }
        }
        else
        {
            wr[0].DebugString("subzero change");
            if (dist > _snakeBody.transform.localScale.x)
            {
                _previousPart.Rotation = Rotation;
                _previousPart.Position = Position;
                _sprite.transform.Translate(_snakeBody.transform.localScale.x, 0, 0);
            }
        }
        
        //else
        //    dist = newDist;
        //}        
        //else dis = 100;   
    }

    public float Rotation
    {
        set { _sprite.rotation = value; }
        get { return _sprite.rotation; }
    }

    public Vector2 Position
    {
        set { _sprite.position = value; }
        get { return _sprite.position; }
    }

    public GameObject AsGameObject()
    {
        return _snakeBody;
    }
}
