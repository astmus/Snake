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
    public GameObject SnakeBodyPrefab; // ����������� � ���������
    public SnakeClient _snakeClient;
    List<SnakeBodySpan> snake;
    KeyController _directionData;
    object lockObj = new object();
    //static float maxDist = 0;
    public OnGuiWriter _writer;

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

        // ����� ��������� ���� ��� � ����� �������� � ������ �� ������ ������ �������� ��������� ����������
        //if (_playerNumber != 0)
        _directionData = new KeyController();
        //else
        //_directionData = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        _snakeClient.RotateHead += OnRotateHead;
        _snakeClient.CatchFruitAnswer += OnCatchFruitAnswer;
        _snakeClient.EnemySnakeGrooveUp += OnEnemySnakeGrooveUp;
        //
    }

    void OnEnemySnakeGrooveUp()
    {
        if (IsEnemyInstance())
            AddBody();
    }

    void OnCatchFruitAnswer(CatchFruitResponse answer)
    {
        if (IsEnemyInstance()) return;
        if (answer.Catched)
            AddBody();
    }

    public int PlayerNumber
    {
        get { return _playerNumber; }
        set { _playerNumber = value; }
    }

    void OnRotateHead(RotateHeadData data)
    {
        if (!IsEnemyInstance()) return;
        switch ((int)data.RotateAngle)
        {
            case 0: RotateHeadTo(BasicDirections.Right);
                break;
            case 90: RotateHeadTo(BasicDirections.Up);
                break;
            case 180: RotateHeadTo(BasicDirections.Left);
                break;
            case 270: RotateHeadTo(BasicDirections.Down);
                break;
        }
        Debug.Log(data.RotateAngle);
        // now sync snake head
        float syncX = transform.position.x;
        float syncY = transform.position.y;

        if ((int)data.RotateAngle == 0 || (int)data.RotateAngle == 180)
        {
            //diffDist = Math.Abs(syncY - data.SyncCoord) * -1;
            syncY = data.SyncCoord;
        }
        else
        {
            //diffDist = Math.Abs(syncX - data.SyncCoord) * -1;
            syncX = data.SyncCoord;
        }

        Vector3 newHeadPos = new Vector3(syncX, syncY, 0);
        float diffDist = Vector3.Distance(transform.position, newHeadPos) * -1;
        _writer.DebugString("dif dist = " + diffDist);

        /*for (int i = snake.Count - 1; i > 1; i--)
        //foreach (SnakeBodySpan obj in snake.Reverse<SnakeBodySpan>())
        {
            SnakeBodySpan obj = snake[i];
            //float rotate = obj.Rotation;
            //obj.Rotation = 0;
            //if ((int)data.RotateAngle == 0 || (int)data.RotateAngle == 180)
            //    obj.Translate(0, diffDist, 0);
            //else
            obj.Translate(diffDist, 0, 0,_writer);
            //obj.Rotation = rotate;
        }*/
        transform.position = newHeadPos;
    }

    public bool IsEnemyInstance()
    {
        return _snakeClient.ActorNumber != _playerNumber;
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
                if (snake.Count > 0 && colliderInfo.gameObject != snake[0].AsGameObject()/* && colliderInfo.gameObject != snake[1].AsGameObject()*/)
                {
                    ResetSnake(false);
                    Time.timeScale = 0f;
                    _writer.DebugString("OnTrigger");
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
        GameObject[] labels = GameObject.FindGameObjectsWithTag("PointLabel");
        TextMesh label = labels[LabelPosForCurrentSnake()].GetComponent<TextMesh>();
        label.text = "0";
    }

    public int LabelPosForCurrentSnake()
    {
        return System.Convert.ToInt32(!(_playerNumber != _snakeClient.ActorNumber));// ���� ��� �������������� �������, �� �� ���� ����� ������ ����������� ����� ����� � ������ ��������
        // � ����������� �� ���� ����� �������� �� ��� ����, ��� ���� ��� �� ���� ������ ������ ���� � ����� ������� ���� � ���������� � ������ ���������� ������ ����� ����� � ���� ��� ������
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
                rotateAngle = BasicDirections.Up;
            if (Input.GetKey(_directionData.Down))
                rotateAngle = BasicDirections.Down;
            bool headIsRotated = RotateHeadTo(rotateAngle);
            int sendAngle = (int)rotateAngle;
            if (sendAngle >= 0 && headIsRotated)
            {
                float syncCoord = (sendAngle == 0 || sendAngle == 180) ? this.transform.position.y : this.transform.position.x;
                _snakeClient.SendRotateAngle(sendAngle, syncCoord);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddBody();
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            speed = 2;

        float dist = Time.deltaTime * speed; // ���������� �� ������� ���� ����������� ������ � ���������� ������� �� ��������� 
        /*if (dist > maxDist)
        {
            maxDist = dist;
        }*/

        //Debug.Log(dist);
        if (dist < 0.4)
            MoveSnake(dist);
        else
            for (int i = 0; i < 4; i++)
                MoveSnake(dist * 0.25f);

    }


    /// <summary>
    /// return true is head is rotated
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    bool RotateHeadTo(float angle)
    {
        if (angle < 0) return false;
        if (Vector2.Distance(Position, lastTurn.Position) < 1.3) return false;
        if ((int)rotation == angle || (int)Math.Abs(rotation - angle) == 180) return false;
        rotation = angle;
        lastTurn = new TargetPoint(angle, position);
        return true;
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
