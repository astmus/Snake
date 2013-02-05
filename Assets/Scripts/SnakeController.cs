using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.Responses;
public interface ISnakePart
{
    float Rotation { set; get; }
    Vector2 Position { set; get; }
    event Action<TargetPoint> PartRotate;
}

static class BasicDirections
{
    public static float Left = 180;
    public static float Right = 0;
    public static float Up = 90;
    public static float Down = 270;
}

public class SnakeController : OTSprite, ISnakePart
{
    static int numberCounter = 1;
    int _playerNumber;

    
    float speed;
    float rotateSpeed;
    bool _isRemoteControling;
    public Texture SnakeBodyTexture;
    public GameObject SnakeBodyPrefab; // заполняется в редакторе
    public SnakeClient _snakeClient;
    List<SnakeBodySpan> snake;
    KeyController _directionData;
    static float maxDist = 0;

    public List<SnakeBodySpan> SnakeBody
    {
        get { return snake; }
    }
    TargetPoint lastTurn;
    public event Action<TargetPoint> PartRotate;
    // Use this for initialization
    new void Start()
    {
        lastTurn = new TargetPoint();
        snake = new List<SnakeBodySpan>();
        speed = 6;
        rotateSpeed = speed * 0.5f;
        _playerNumber = numberCounter++;
        
        // позже перенести этот код в класс настроек и оттуда по номеру игрока получать настройки управления
        //if (_playerNumber != 0)
            _directionData = new KeyController();
        //else
        //_directionData = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        _snakeClient.RotateHead += _snakeClient_RotateHead;
        _snakeClient.CatchFruitAnswer += OnCatchFruitAnswer;
        //
    }

    void OnCatchFruitAnswer(CatchFruitResponse answer)
    {
        if (answer.Catched)
            AddBody();
    }

    public int PlayerNumber
    {
        get { return _playerNumber; }
        set { _playerNumber = value; }
    }

    void _snakeClient_RotateHead(float rotateAngle)
    {
        if (_snakeClient.ActorNumber == _playerNumber) return;
        if ((int)rotateAngle == 180)
            RotateHeadTo(BasicDirections.Left);
        if ((int)rotateAngle == 0)
            RotateHeadTo(BasicDirections.Right);
        if ((int)rotateAngle == 90)
            RotateHeadTo(BasicDirections.Up);
        if ((int)rotateAngle == 270)
            RotateHeadTo(BasicDirections.Down);
    }

    void OnTriggerEnter(Collider colliderInfo)
    {
        if (colliderInfo.gameObject.tag == "SnakeHead") return;
        if (colliderInfo.gameObject.tag == "Wall")
            ResetSnake(true);
        else
            if (colliderInfo.gameObject.tag == "Fruit")
            {
            }            
            else
                if (snake.Count > 0 && colliderInfo.gameObject != snake[0].AsGameObject() /*&& colliderInfo.gameObject != snake[1].AsGameObject()*/)
                {
                    ResetSnake(false);
                    Time.timeScale = 0f;
                }
    }

    void ResetSnake(bool colideWithWall)
    {
        RemoveBody();
        if (colideWithWall)
            transform.position = new Vector3(0, 0, 0);
        speed = 6;
        //Camera.main.audio.Stop();
        //Camera.main.audio.Play();
        if (_snakeClient.ActorNumber == _playerNumber)
        {
            TextMesh label = GameObject.FindGameObjectsWithTag("PointLabel")[LabelPosForCurrentSnake()].GetComponent<TextMesh>();
            label.text = "0";
        }        
    }

    public int LabelPosForCurrentSnake()
    {
        return System.Convert.ToInt32(!(_playerNumber != _snakeClient.ActorNumber));// знаю это трудночитаемое условие, но оп сути здесь только определение какой лейбл с очками изменять
        // в зависимости от того каким червяком он был взят, для того что бы очки игрока всегда были в левом верхнем углу а противника в правом независимо первым игрок зашел в игру или вторым
    }

    // Update is called once per frame
    new void Update()
    {
        //if (_isRemoteControling) return;
        //Debug.Log("actor num = "+_snakeClient.ActorNumber);
        //Debug.Log("player number = "+_playerNumber);
        
        if (_snakeClient.ConnetionStatus != ConnectionStatus.InGame) return;
#if UNITY_EDITOR
        if (_directionData == null) return;
#endif
        if (_snakeClient.ActorNumber == _playerNumber)
        {
            float rotateAngle = -1;
            if (Input.GetKey(_directionData.Left))
                rotateAngle = BasicDirections.Left;
            if (Input.GetKey(_directionData.Right))
                rotateAngle = BasicDirections.Right;
            if (Input.GetKey(_directionData.Up))
                rotateAngle =  BasicDirections.Up;
            if (Input.GetKey(_directionData.Down))
                rotateAngle = BasicDirections.Down;
            RotateHeadTo(rotateAngle);
            int sendAngle = (int)rotateAngle;
            if (sendAngle >= 0)
                _snakeClient.SendRotateAngle(sendAngle);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
           AddBody();
           Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            speed = 2;

        float dist = Time.deltaTime * speed; // расстояние на которое надо передвинуть змейку с последнего момента ее отрисовки 
        if (dist > maxDist)
        {            
            maxDist = dist;
        }
        
        //Debug.Log(dist);
        if (dist < 0.4)
            MoveSnake(dist);
        else
            for (int i = 0; i < 4; i++)
                MoveSnake(dist * 0.25f);

    }

    void RotateHeadTo(float angle)
    {
        if (angle < 0) return;
        if (Vector2.Distance(Position, lastTurn.Position) < 1.3) return;
        if ((int)rotation == angle || (int)Math.Abs(rotation - angle) == 180) return;
        rotation = angle;
        lastTurn = new TargetPoint(angle, position);
        //Debug.Log("Send code = " + sendCode);
        //_snakeClient.SendTextMessage(sendCode);
        //if (PartRotate != null)
        //    PartRotate(lastTurn);
    }

    void MoveSnake(float distance)
    {

        transform.Translate(distance, 0, 0);
#if UNITY_EDITOR
        if (snake == null) return;
#endif        
        foreach (SnakeBodySpan obj in snake)
            obj.Translate(distance, 0, 0);
        /*foreach (distance obj in snake)
        {
            oldPos = obj.AsGameObject().transform.position;
            obj.AsGameObject().transform.position = newPos;
            newPos = oldPos;
        }*/
        //for (int i = snake.Count - 1; i > 0; i--)
        /*{
            snake
        }*/
    }

    void AddBody()
    {
        speed += 0.25f;
        GameObject bodySpan = (GameObject)Instantiate(SnakeBodyPrefab, Vector3.zero, Quaternion.identity);
        ISnakePart part = snake.Count != 0 ? (ISnakePart)snake.Last() : (ISnakePart)this;
        SnakeBodySpan span = new SnakeBodySpan(bodySpan, part);
        snake.Add(span);
    }
    public float Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    public void RemoveBody()
    {
        while (snake.Count > 0)
        {
            Destroy(snake[0].AsGameObject());
            snake.RemoveAt(0);
        }

    }
}
