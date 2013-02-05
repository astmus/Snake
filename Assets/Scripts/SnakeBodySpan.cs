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
    ISnakePart previousPart;
    //float dist = float.MaxValue;
    public event System.Action<TargetPoint> PartRotate;
    public SnakeBodySpan(GameObject span, ISnakePart part)
    {
        _pointList = new List<TargetPoint>();
        _snakeBody = span;
        _sprite = span.GetComponent<OTSprite>();
        _sprite.rotation = part.Rotation;
        //_sprite.position = new Vector2(part.Position.x - _sprite.size.x, part.Position.y);
        _sprite.position = part.Position;
        _sprite.transform.Translate(-_snakeBody.transform.lossyScale.x, 0, 0);
        previousPart = part;
        part.PartRotate += new System.Action<TargetPoint>(OnPartRotate);
    }

    void OnPartRotate(TargetPoint target)
    {
        AddTargetPoint(target);
    }

    public void AddTargetPoint(TargetPoint point)
    {
        _pointList.Add(point);
    }

    public void Translate(float x, float y, float z)
    {
        //Vector2 v = Position + new Vector2(x,y);
        _sprite.transform.Translate(x, y, z);
        //Debug.Log("lossy"+_snakeBody.transform.lossyScale.x);
        //Debug.Log("local"+_snakeBody.transform.localScale.x);
        //if (_pointList.Count != 0)
        //{
        float dist = Vector2.Distance(Position, previousPart.Position);
        if (dist > _snakeBody.transform.localScale.x)
        {
            Rotation = previousPart.Rotation;
            Position = previousPart.Position;
            _sprite.transform.Translate(-_snakeBody.transform.localScale.x, 0, 0);




            //Position.Set(previousPart.Position.x - _sprite.size.x, previousPart.Position.y);
            //dist = float.MaxValue;
            //if (PartRotate != null)
            //    PartRotate(_pointList[0]);
            //_pointList.RemoveAt(0);                
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
