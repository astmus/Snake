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
    Vector2? LastRotatePoint {set; get;}
}

static class BasicDirections
{
    public static float Left = 180;
    public static float Right = 0;
    public static float Up = 90;
    public static float Down = 270;
}

public class SnakeController : MonoBehaviour, ISnakePart
{
    private static IntRange _numberCounter = new IntRange(1,2);
    int _playerNumber;
    float speed;
    //float rotateSpeed;
    bool _isRemoteControling;
    public Texture SnakeBodyTexture;
    public GameObject SnakeBodyPrefab; // заполняется в редакторе
    public SnakeClient _snakeClient;
    public GameInformer _informer;
    public TextMesh _WhoIsWhoLabel; // label на котором будет указано изграет этим червыком игрок или противник
    public Boom _boomPref; // префаб взыра при столкновении
    List<SnakeBodySpan> snake;
    KeyController _directionData;
    GameObject[] _labels;
    TargetPoint lastTurn;
    public event Action<TargetPoint> PartRotate;
    public SoundManager _soundManager;
    //static float maxDist = 0;
    //public OnGuiWriter _writer;

    //fake
    float rotation;
    Vector2 position;

    /*public List<SnakeBodySpan> SnakeBody
    {
        get { return snake; }
    }   
    
    // Use this for initialization
    new void Start()
    {
        lastTurn = new TargetPoint();
        snake = new List<SnakeBodySpan>();
        speed = 6;
        //rotateSpeed = speed * 0.5f;
        _playerNumber = _numberCounter++;
        _labels = GameObject.FindGameObjectsWithTag("PointLabel");        
        // позже перенести этот код в класс настроек и оттуда по номеру игрока получать настройки управления
        //if (_playerNumber != 0)  
        _directionData = GameSettings.Instance.Player1Control;
        //else
        //_directionData = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        _snakeClient.RotateHead += OnRotateHead;
        _snakeClient.CatchFruitAnswer += OnCatchFruitAnswer;
        _snakeClient.EnemySnakeGrooveUp += OnEnemySnakeGrooveUp;
        _snakeClient.EnemyPointsCountUpdated += OnEnemyPointsCountUpdated;
        _snakeClient.GameStatusChanged += OnGameStatusChanged;
        _snakeClient.EnemySnakeReset += OnEnemySnakeReset;
    }

    void OnEnemySnakeReset(bool colideWithWall)
    {
        Debug.Log("enemy snake reset");
        if (IsEnemyInstance()) ResetSnake(colideWithWall);
    }

    void OnGameStatusChanged(GameStatus status)
    {        
        if (status != GameStatus.InRoom) return;
        if (IsEnemyInstance())
        {
            gameObject.GetComponent<OTSprite>().tintColor = Color.red;
            _WhoIsWhoLabel.text = "< is enemy";
        }
        else
        {
            gameObject.GetComponent<OTSprite>().tintColor = Color.white;
            _WhoIsWhoLabel.text = "< is you";
        }
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
        Debug.Log("sizeData = "+sizeData.NewSize+" snakesize = "+snake.Count);
        while (snake.Count != sizeData.NewSize)
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

    void OnRotateHead(RotateHeadData rotateData)
    {
        if (!IsEnemyInstance()) return;
        Rotation = rotateData.RotateAngle[0];
        transform.position = new Vector3(rotateData.CoordX[0], rotateData.CoordY[0], 0);
        //string str = rotateData.RotateAngle[0] + ";" + rotateData.CoordX[0] + ";" + rotateData.CoordY[0] + "|"+Environment.NewLine;
        
        / *if (snake.Count >= rotateData.CoordX.Length)
        {
            _writer.DebugString2("Rotate head data different size");
            ResetSnake(false);
        }* /
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
                Debug.Log("Wall Snake reset");
                ResetSnake(true);
                //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                break;
            case "Fruit":
                break;
            default:
                if (snake.Count > 0 && colliderInfo.gameObject != snake[0].AsGameObject()/ * && colliderInfo.gameObject != snake[1].AsGameObject()* /)
                {
                    //ResetSnake(false);
                    _snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                    //Time.timeScale = 0f;
                }
                break;
        }
    }

    void ResetSnake(bool colideWithWall)
    {
        Debug.Log("Reset snake collide wall == " + colideWithWall);
        if ((int)speed == 0) 
        {
            Debug.Log(speed);
            return; // если скорость змеи 0 значит она уже врезалась и идет анимация столкновения и нам делать ничего не надо
        }
        speed = 0;
        if (IsEnemyInstance() == false) // отправляем только если обнуляется червяк игрока
            _snakeClient.SendSnakeReset(true);
        Boom boom = (Boom)Instantiate(_boomPref);
        boom.transform.position = new Vector3(transform.position.x, transform.position.y, boom.transform.position.z);
        if (colideWithWall) // если врезались в стену то по окончании взрыва переведем червяка в центр игрового поля
            boom.BoomCompleted += () =>
                {
                    transform.position = new Vector3(0, 0, 0);
                    speed = 6;
                };
        boom.StartAnimation(1.5f);
        RemoveBody(1.5f);
        
        //Camera.main.audio.Stop();
        //Camera.main.audio.Play();
        //GameObject[] labels = GameObject.FindGameObjectsWithTag("PointLabel");
        //TextMesh label = _labels[LabelPosForThisSnake()].GetComponent<TextMesh>();
        //label.text = "0";
    }

    public void RemoveBody(float scaledTime)
    {
        float timeAnimation = scaledTime/snake.Count;
        int delayFactor = 0;
        while (snake.Count != 0)
        {
            snake[snake.Count-1].AnimationDestroy(timeAnimation,timeAnimation*delayFactor++);
            snake.RemoveAt(snake.Count - 1);
        }
    }

    public int LabelPosForThisSnake()
    {
        return System.Convert.ToInt32((_playerNumber != _snakeClient.ActorNumber));// знаю это трудночитаемое условие, но оп сути здесь только определение какой лейбл с очками изменять
        // в зависимости от того каким червяком он был взят, для того что бы очки игрока всегда были в левом верхнем углу а противника в правом независимо первым игрок зашел в игру или вторым
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
                //float syncCoord = (sendAngle == 0 || sendAngle == 180) ? this.transform.position.y : this.transform.position.x;
                _snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
            }
        }
        
        / *if (Input.GetKeyUp(KeyCode.Space))
        {
            if (IsEnemyInstance()) return;
            //OTSprite headSprite = gameObject.GetComponent<OTSprite>();
            
            
            //OTSprite sprite = (OTSprite)OTSprite.Instantiate(headSprite);
            //sprite.position = headSprite.position;
            //sprite.tintColor = Color.red;
            //iTween.ColorTo(headSprite, new Color(255, 255, 255, 0), 1.5f);
            //iTween.ColorTo(headSprite, iTween.Hash("color", new Color(200, 0, 0, 0), "time", .5));
            //iTween.ScaleTo(headSprite, new Vector3(2, 2), 1.5f);
            AddBody();
            //Time.timeScale = 1f;
        }* /
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    speed = 2;

        float dist = Time.deltaTime * speed; // расстояние на которое надо передвинуть змейку с последнего момента ее отрисовки 
        / *if (dist > maxDist)
        {
            maxDist = dist;
        }* /

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
        / *foreach (distance obj in snake)
        {
            oldPos = obj.AsGameObject().transform.position;
            obj.AsGameObject().transform.position = newPos;
            newPos = oldPos;
        }* /
        //for (int i = snake.Count - 1; i > 0; i--)
        / *{
            snake
        }* /
    }

    void AddBody()
    {
        speed += 0.25f;
        GameObject bodySpan = (GameObject)Instantiate(SnakeBodyPrefab, Vector3.zero, Quaternion.identity);
        ISnakePart part = snake.Count != 0 ? (ISnakePart)snake.Last() : (ISnakePart)this;
        SnakeBodySpan span = new SnakeBodySpan(bodySpan, part);
        snake.Add(span);
        if (IsEnemyInstance()) return;
        DisplayGrownUpInfo();
    }

    void DisplayGrownUpInfo()
    {
        string _groweMessage = "";
        switch (snake.Count)
        {
            case 5:
                _groweMessage = "pythone";
                break;
            case 10:
                _groweMessage = "anakonda";
                break;
            case 15:
                _groweMessage = "MONSTER";
                break;
            case 20:
                _groweMessage = "king snake";
                break;
        }
        if (_groweMessage == String.Empty) return;
        _soundManager.PlaySound(SoundManagerClip.SnakeLevelUp);
        _informer.AddMessage(new InformerMessage(_groweMessage, false, true));
        
    }*/

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

    public Vector2? LastRotatePoint
    {
        get;
        set;
    }
}
