using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.Responses;
using Assets.Scripts.SendDataModel;

public class SnakeControllerOffline : MonoBehaviour, ISnakePart
{
    private static IntRange _numberCounter = new IntRange(1, 2);
    public TextMesh PointsLabel;
    int _playerNumber;
    float speed;
    //float rotateSpeed;
    bool _isRemoteControling;
    public Texture SnakeBodyTexture;
    public GameObject SnakeBodyPrefab; // заполняется в редакторе
    //public SnakeClient _snakeClient;
    public GameInformer _informer;
    public TextMesh _WhoIsWhoLabel; // label на котором будет указано изграет этим червыком игрок или противник
    public Boom _boomPref; // префаб взыра при столкновении
    List<SnakeBodySpan> snake;
    KeyController _directionData;
    public OfflineGameStateController _gameStateController;
    public OfflineFruit _fruit;
    public SoundManager _soundManager;
    private float _rotation;
    float _halfScreenSize;
    private float _startSpeed = 3f;
    ISnakePart _lastPart;
    public Vector2? LastRotatePoint { get; set; }
    //static float maxDist = 0;

    //must del if not used
    //public OfflineGUIWriter _writer;

    public List<SnakeBodySpan> SnakeBody
    {
        get { return snake; }
    }
    TargetPoint lastTurn;
    //public event Action<TargetPoint> PartRotate;
    // Use this for initialization
    void Start()
    {
        lastTurn = new TargetPoint();
        snake = new List<SnakeBodySpan>();
        
        //rotateSpeed = speed * 0.5f;
        _playerNumber = _numberCounter++;
        _lastPart = this;        
        _directionData = GameSettings.Instance.Player1Control;
        _halfScreenSize = Screen.width * 0.5f;
        //
        //gameObject.GetComponent<OTSprite>().tintColor = _playerNumber == 1 ? Color.white : Color.red;
        //_WhoIsWhoLabel.text = "< player " + _playerNumber;
    }

    public int PointsCount
    {
        get { return System.Convert.ToInt32(PointsLabel.text); }
        set { PointsLabel.text = value.ToString(); }
    }

    void OnGameStatusChanged(GameStatus status)
    {
        /*if (status != GameStatus.InRoom) return;
        if (IsEnemyInstance())
        {*/
            
        /*}
        else
        {
            gameObject.GetComponent<OTSprite>().tintColor = Color.white;
            _WhoIsWhoLabel.text = "< is you";
        }*/
        //Debug.Log("GameStatus == " + gameObject.GetComponent<OTSprite>().tintColor);
        //Debug.Log("SnakeClient number == "+_snakeClient.ActorNumber);
    }

   /* void OnEnemyPointsCountUpdated(int enemyPoints)
    {
        if (!IsEnemyInstance()) return;
        //GameObject[] _labels = GameObject.FindGameObjectsWithTag("PointLabel");
        TextMesh label = _labels[LabelPosForThisSnake()].GetComponent<TextMesh>();
        label.text = enemyPoints.ToString();
    }*/

    /*void OnEnemySnakeGrooveUp(EnemySnakeSizeChangeData sizeData)
    {
        if (!IsEnemyInstance()) return;
        Debug.Log("sizeData = " + sizeData.NewSize + " snakesize = " + snake.Count);
        while (snake.Count != sizeData.NewSize)
            AddBody();
    }*/

    /*void OnCatchFruitAnswer(CatchFruitResponse answer)
    {
        if (IsEnemyInstance()) return;
        if (answer.Catched)
            AddBody();
    }*/

    public void DifficultSelected(int startSpeedIncrease)
    {
        _startSpeed = 4 + startSpeedIncrease;
        speed = _startSpeed;
    }

    public int PlayerNumber
    {
        get { return _playerNumber; }
        set { _playerNumber = value; }
    }

    void OnRotateHead(RotateHeadData rotateData)
    {
        /*if (!IsEnemyInstance()) return;
        Rotation = rotateData.RotateAngle[0];
        transform.position = new Vector3(rotateData.CoordX[0], rotateData.CoordY[0], 0);
        //string str = rotateData.RotateAngle[0] + ";" + rotateData.CoordX[0] + ";" + rotateData.CoordY[0] + "|"+Environment.NewLine;
        */
        /*if (snake.Count >= rotateData.CoordX.Length)
        {
            _writer.DebugString2("Rotate head data different size");
            ResetSnake(false);
        }*/
        /*for (int i = 0; i < snake.Count; i++)
        {
            SnakeBodySpan span = snake[i];
            span.Rotation = rotateData.RotateAngle[i + 1];
            span.Position = new Vector2(rotateData.CoordX[i + 1], rotateData.CoordY[i + 1]);
            //str += rotateData.RotateAngle[i+1] + ";" + rotateData.CoordX[i+1] + ";" + rotateData.CoordY[i+1] + "|"+Environment.NewLine;
        }*/
        //_writer.DebugString(str);
    }

    public bool IsEnemyInstance()
    {
        return _numberCounter != _playerNumber;
    }

    void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        //print(colliderInfo.gameObject.tag);
        //if (colliderInfo.gameObject.tag == "SnakeHead") return;
        
        switch (colliderInfo.gameObject.tag)
        {
            case "Wall":
                ResetSnake(true);
                //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                break;
            case "Fruit":
                AddBody();
                break;
            case "SnakeBody":
                if (!GameSettings.Instance.OfflineRules.WithSelfBody && !GameSettings.Instance.OfflineRules.WithEnemyBody) return;
                int position = numberPartOfThisSnake(colliderInfo.gameObject);
                if (GameSettings.Instance.OfflineRules.WithSelfBody && position > 2)
                    ResetSnake(false);
                if (GameSettings.Instance.OfflineRules.WithEnemyBody && position == -1)
                    ResetSnake(false);
                break;
            default:
                /*if (snake.Count > 0 && colliderInfo.gameObject != snake[0].AsGameObject())
                {
                    //ResetSnake(false);
                    //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                    //Time.timeScale = 0f;
                }*/
                break;
        }
    }

    int numberPartOfThisSnake(GameObject part)
    {
        return snake.FindIndex(b=>b.AsGameObject() == part);
    }

    void ResetSnake(bool colideWithWall)
    {
        //_snakeClient.SendSnakeReset(true);
        if (speed == 0) return; // если скорость змеи 0 хначит она и так уже врезалась
        Boom boom = (Boom)Instantiate(_boomPref);
        boom.transform.position = new Vector3(transform.position.x, transform.position.y, boom.transform.position.z);
        if (colideWithWall) // если врезались в стену то по окончании взрыва переведем червяка в центр игрового поля
            boom.BoomCompleted += () =>
            {
                transform.position = new Vector3(0, 0, 0);
                speed = _startSpeed;
            };
        else
            boom.BoomCompleted += () =>
            {
                speed = _startSpeed;
            };
        boom.StartAnimation(1.5f);
        RemoveBody(1.5f);
        speed = 0;
        //Camera.main.audio.Stop();
        //Camera.main.audio.Play();
        //GameObject[] labels = GameObject.FindGameObjectsWithTag("PointLabel");
        //TextMesh label = _labels[LabelPosForThisSnake()].GetComponent<TextMesh>();
        //label.text = "0";
    }



    public void RemoveBody(float scaledTime)
    {
        float timeAnimation = scaledTime / snake.Count;
        int delayFactor = 0;
        while (snake.Count != 0)
        {
            snake[snake.Count - 1].AnimationDestroy(timeAnimation, timeAnimation * delayFactor++);
            snake.RemoveAt(snake.Count - 1);
        }
        _lastPart = this;
    }

    public int LabelPosForThisSnake()
    {
        return System.Convert.ToInt32((_playerNumber != _numberCounter));// знаю это трудночитаемое условие, но оп сути здесь только определение какой лейбл с очками изменять
        // в зависимости от того каким червяком он был взят, для того что бы очки игрока всегда были в левом верхнем углу а противника в правом независимо первым игрок зашел в игру или вторым
    }

    // Update is called once per frame
    void Update()
    {
        //if (_isRemoteControling) return;
        //Debug.Log("actor num = "+_snakeClient.ActorNumber);
        //Debug.Log("player number = "+_playerNumber);

        if (_gameStateController.GameStatus != GameStatus.InGame) return;
#if UNITY_EDITOR
        if (_directionData == null) return;
#endif
        //if (_numberCounter == _playerNumber)
        //{
            float rotateAngle = -1;
            if (Input.GetKey(_directionData.Left))
                rotateAngle = BasicDirections.Left;
            if (Input.GetKey(_directionData.Right))
                rotateAngle = BasicDirections.Right;
            if (Input.GetKey(_directionData.Up))
                rotateAngle = BasicDirections.Up;
            if (Input.GetKey(_directionData.Down))
                rotateAngle = BasicDirections.Down;            

            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                    switch ((int)Rotation)
                    {
                        case -90:
                        case 270:
                            rotateAngle = (t.position.x < _halfScreenSize) ? Rotation - 90 : Rotation + 90;                    
                            break;
                        /*case 0:
                            rotateAngle = (Position.y > _fruit.CurrentPos.y) ? Rotation - 90 : Rotation + 90;
                            break;
                        case 180:
                            rotateAngle = (Position.y < _fruit.CurrentPos.y) ? Rotation - 90 : Rotation + 90;
                            break;*/
                        default:
                            rotateAngle = (t.position.x > _halfScreenSize) ? Rotation - 90 : Rotation + 90;
                            break;
                    }                
            }
            //Debug.developerConsoleVisible = true;
            //Debug.Log(rotateAngle.ToString());

            bool headIsRotated = RotateHeadTo((int)rotateAngle);
            //int sendAngle = (int)rotateAngle;
            //if (sendAngle >= 0 && headIsRotated)
            //{
                //float syncCoord = (sendAngle == 0 || sendAngle == 180) ? this.transform.position.y : this.transform.position.x;
                //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
            //}
        //}
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //if (IsEnemyInstance()) return;
            //OTSprite headSprite = gameObject.GetComponent<OTSprite>();
            //OTSprite sprite = (OTSprite)OTSprite.Instantiate(headSprite);
            //sprite.position = headSprite.position;
            //sprite.tintColor = Color.red;
            //iTween.ColorTo(headSprite, new Color(255, 255, 255, 0), 1.5f);
            //iTween.ColorTo(headSprite, iTween.Hash("color", new Color(200, 0, 0, 0), "time", .5));
            //iTween.ScaleTo(headSprite, new Vector3(2, 2), 1.5f);
            AddBody();

            //Time.timeScale = Time.timeScale > 0 ? 0f : 1f;
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    speed = 2;

        float dist = Time.deltaTime * speed; // расстояние на которое надо передвинуть змейку с последнего момента ее отрисовки 
        /*if (dist > maxDist)
        {
            maxDist = dist;
        }*/

        //Debug.Log(dist);
        //if (dist < 0.2)
            MoveSnake(dist);
        //else
        //    for (int i = 0; i < 5; i++)
        //       MoveSnake(dist * 0.2f);
    }


    /// <summary>
    /// return true is head is rotated
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    bool RotateHeadTo(float angle)
    {
        if (angle == -1) return false;
        if (Vector2.Distance(Position, lastTurn.Position) < 1.3) return false;

        if ((int)Rotation == angle || (int)Math.Abs(Rotation - angle) == 180) return false;
        Rotation = angle;
        LastRotatePoint = Position;
        lastTurn = new TargetPoint(angle, transform.position);
        return true;
        //Debug.Log("Send code = " + sendCode);
        //_snakeClient.SendTextMessage(sendCode);
        //if (PartRotate != null)
        //    PartRotate(lastTurn);
    }

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
            
            transform.rotation = Quaternion.Euler(0, 0, val);
            _rotation = val;
        }
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
        ISnakePart part = _lastPart;
        SnakeBodySpan body = ((GameObject)Instantiate(SnakeBodyPrefab, Vector3.zero, Quaternion.identity)).GetComponent("SnakeBodySpan") as SnakeBodySpan;
        body.PreviousPart = part;
        snake.Add(body);
        _lastPart = body;
       // CalcSpeedUp(snake.Count);
        speed += CalcSpeedUp(snake.Count);
        print(speed);
        //if (IsEnemyInstance()) return;
        DisplayGrownUpInfo();
    }

    float CalcSpeedUp(int snakeLength)
    { 
        if (snakeLength < 5) return GameSettings.Instance.SpeedIncreaseFirst;
        if (snakeLength >= 5 && snakeLength < 13) return GameSettings.Instance.SpeedIncreaseSecond;
        else
            return GameSettings.Instance.SpeedIncreaseThird;
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
        _informer.AddMessage(new InformerMessage(_groweMessage, false, PlayGrownUpSound, true));
    }

    void PlayGrownUpSound()
    {
        _soundManager.PlaySound(SoundManagerClip.SnakeLevelUp);
    }

    public Vector2 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
}

