using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.Responses;
using Assets.Scripts.SendDataModel;
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
    private static IntRange _numberCounter = new IntRange(1,2);
    int _playerNumber;
    float speed;
    float rotateSpeed;
    bool _isRemoteControling;
    public Texture SnakeBodyTexture;
    public GameObject SnakeBodyPrefab; // ����������� � ���������
    public SnakeClient _snakeClient;
    List<SnakeBodySpan> snake;
    KeyController _directionData;
    GameObject[] _labels;
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
        _playerNumber = _numberCounter++;
        _labels = GameObject.FindGameObjectsWithTag("PointLabel");        
        // ����� ��������� ���� ��� � ����� �������� � ������ �� ������ ������ �������� ��������� ����������
        //if (_playerNumber != 0)  
        _directionData = new KeyController();
        //else
        //_directionData = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        _snakeClient.RotateHead += OnRotateHead;
        _snakeClient.CatchFruitAnswer += OnCatchFruitAnswer;
        _snakeClient.EnemySnakeGrooveUp += OnEnemySnakeGrooveUp;
        _snakeClient.EnemyPointsCountUpdated += OnEnemyPointsCountUpdated;
        _snakeClient.GameStatusChanged += OnGameStatusChanged;
        //
    }

    void OnGameStatusChanged(GameStatus status)
    {        
        if (status != GameStatus.InRoom) return;
        gameObject.GetComponent<OTSprite>().tintColor = IsEnemyInstance() ? Color.red : Color.white;
        //Debug.Log("GameStatus == " + gameObject.GetComponent<OTSprite>().tintColor);
        //Debug.Log("SnakeClient number == "+_snakeClient.ActorNumber);
    }

    void OnEnemyPointsCountUpdated(int enemyPoints)
    {
        if (!IsEnemyInstance()) return;
        //GameObject[] _labels = GameObject.FindGameObjectsWithTag("PointLabel");
        TextMesh label = _labels[LabelPosForThisSnake()].GetComponent<TextMesh>();
        label.text = enemyPoints.ToString();
    }

    void OnEnemySnakeGrooveUp(EnemySnakeSizeChangeData sizeData)
    {
        if (!IsEnemyInstance()) return;
        if (sizeData.NewSize >= snake.Count)
            AddBody();
        else
            ResetSnake(false);
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

    void OnRotateHead(RotateHeadData rotateData)
    {
        if (!IsEnemyInstance()) return;
        Rotation = rotateData.RotateAngle[0];
        transform.position = new Vector3(rotateData.CoordX[0], rotateData.CoordY[0], 0);
        //string str = rotateData.RotateAngle[0] + ";" + rotateData.CoordX[0] + ";" + rotateData.CoordY[0] + "|"+Environment.NewLine;
        
        if (snake.Count >= rotateData.CoordX.Length)
        {
            _writer.DebugString("OnRotateHead");
            ResetSnake(false);
        }
            

        for (int i = 0; i < snake.Count; i++)
        {
            SnakeBodySpan span = snake[i];
            span.Rotation = rotateData.RotateAngle[i + 1];
            span.Position = new Vector2(rotateData.CoordX[i + 1], rotateData.CoordY[i + 1]);
            //str += rotateData.RotateAngle[i+1] + ";" + rotateData.CoordX[i+1] + ";" + rotateData.CoordY[i+1] + "|"+Environment.NewLine;

        }
        //_writer.DebugString(str);
    }

    public bool IsEnemyInstance()
    {
        return _snakeClient.ActorNumber != _playerNumber;
    }

    void OnTriggerEnter(Collider colliderInfo)
    {
        if (IsEnemyInstance()) return;
        if (colliderInfo.gameObject.tag == "SnakeHead") return;
        switch (colliderInfo.gameObject.tag)
        {
            case "Wall":
                ResetSnake(true);
                _snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                break;
            case "Fruit":
                break;
            default:
                if (snake.Count > 0 && colliderInfo.gameObject != snake[0].AsGameObject()/* && colliderInfo.gameObject != snake[1].AsGameObject()*/)
                {
                    //ResetSnake(false);
                    _snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                    //Time.timeScale = 0f;
                    _writer.DebugString("OnTrigger");
                }
                break;
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
        //GameObject[] labels = GameObject.FindGameObjectsWithTag("PointLabel");
        
        //TextMesh label = _labels[LabelPosForThisSnake()].GetComponent<TextMesh>();
        //label.text = "0";
    }

    public int LabelPosForThisSnake()
    {
        return System.Convert.ToInt32((_playerNumber != _snakeClient.ActorNumber));// ���� ��� �������������� �������, �� �� ���� ����� ������ ����������� ����� ����� � ������ ��������
        // � ����������� �� ���� ����� �������� �� ��� ����, ��� ���� ��� �� ���� ������ ������ ���� � ����� ������� ���� � ���������� � ������ ���������� ������ ����� ����� � ���� ��� ������
    }

    // Update is called once per frame
    new void Update()
    {
        //if (_isRemoteControling) return;
        //Debug.Log("actor num = "+_snakeClient.ActorNumber);
        //Debug.Log("player number = "+_playerNumber);

        if (_snakeClient.ConnectionStatus != GameStatus.InGame) return;
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
                _snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
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
