using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.Responses;
using Assets.Scripts.SendDataModel;
using Assets;

public class SnakeControllerOffline : MonoBehaviour, ISnakePart
{
    public const string MAX_POINTS_KEY = "maxPoints";
    public const string MAX_LENGTH_KEY = "maxLength";
    public const string PLAYER_RANGE_KEY = "playerRange";

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
    private float _startSpeed = 3f;
    int _maxPoints = 0;
    public int MaxPoints
    {
        get { return _maxPoints; }
        private set 
        { 
            _maxPoints = value;
            PlayerPrefs.SetInt(MAX_POINTS_KEY, _maxPoints);
        }
    }

    int _maxLength = 0;
    public int MaxLength
    {
        get { return _maxLength; }
        private set 
        { 
            _maxLength = value;
            PlayerPrefs.SetInt(MAX_LENGTH_KEY,_maxLength);
        }
    }

    ISnakePart _lastPart;
    public Vector2? LastRotatePoint { get; set; }
    //static float maxDist = 0;
    SnakeMoveHandler _moveController;
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
        if (PlayerPrefs.HasKey(MAX_POINTS_KEY))
            _maxPoints = PlayerPrefs.GetInt(MAX_POINTS_KEY);
        if (PlayerPrefs.HasKey(MAX_LENGTH_KEY))
            _maxLength = PlayerPrefs.GetInt(MAX_LENGTH_KEY);
        //rotateSpeed = speed * 0.5f;
        _playerNumber = _numberCounter++;
        _lastPart = this;        
        _directionData = GameSettings.Instance.Player1Control;
        
        switch (GameSettings.Instance.ControllerType)
        {
            case ControllerType.OneTouch:
                _moveController = new OneTouchHandler();
                break;
            case ControllerType.TwoTouch:
                _moveController = new TwoTouchHandler();
                break;
            case ControllerType.TwoTouchReverse:
                _moveController = new TwoTouchReverseHandler();
                break;
        }
        UpdateUserHUD();
        //
        //gameObject.GetComponent<OTSprite>().tintColor = _playerNumber == 1 ? Color.white : Color.red;
        //_WhoIsWhoLabel.text = "< player " + _playerNumber;
    }

    int _currentPoints;
    public int PointsCount
    {
        get { return _currentPoints; }
        set 
        { 
            _currentPoints = value;
            if (_currentPoints > _maxPoints)
                MaxPoints = _currentPoints;
            UpdateUserHUD();            
        }
    }

    private void UpdateUserHUD()
    {
        PointsLabel.text = String.Format("{0}/{1} \n{2}/{3}", _currentPoints, _maxPoints, snake.Count, _maxLength);
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
        
    }

    public bool IsEnemyInstance()
    {
        return _numberCounter != _playerNumber;
    }

    List<GameObject> _colidedwalls = new List<GameObject>();
    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case SnakeTags.Brick:
                GameObject brickWall = collision.gameObject.transform.parent.gameObject;
                if (_colidedwalls.Contains(brickWall)) return;
                print("brick colide");
                _colidedwalls.Add(brickWall);
                var bodies = brickWall.GetComponentsInChildren<Rigidbody2D>();                

                foreach (Rigidbody2D br in bodies)
                    br.gravityScale = 1;

                var body = collision.gameObject.GetComponent<Rigidbody2D>();
                body.AddForce(AngleToVector(_rotation) * speed, ForceMode2D.Impulse);
                var colidedColiders = collision.gameObject.transform.parent.gameObject.GetComponentsInChildren<BoxCollider2D>().ToList();
                var brickColliders = from brick in GameObject.FindGameObjectsWithTag(SnakeTags.Brick).Select(s=>s.GetComponent<BoxCollider2D>()) where !colidedColiders.Contains(brick) select brick; // выбираем все кирпичи которые не являются детками того префаба стены с которым столкнулась змея
                foreach (BoxCollider2D colider in brickColliders)
                    colidedColiders.ForEach((BoxCollider2D box) => { Physics2D.IgnoreCollision(box, colider); }); //и говорим игноирровать все кирпичи которые разлетаются от удара дабы не рушились стены в короые змея не врезалась
                                
                //print("all count exclude == " + all.Count());
                //print("brick count == " + bricks.Length);                
                break;  
        }
    }

/*
//     void FixedUpdate()
//     {
//         if (_r.Count == 0) return;
//         print("fized update");
//         do
//         {
//             BoxCollider2D[] coliders = _r.Dequeue();
//             foreach (BoxCollider2D bc in coliders)
//             {
//                 bc.isTrigger = true;
//             }
//         }
//         while (_r.Count > 0);
//     }
*/

    Vector2 AngleToVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Rad2Deg), -Mathf.Sin(angle * Mathf.Rad2Deg));
    }

    void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        //print(colliderInfo.gameObject.tag);
        //if (colliderInfo.gameObject.tag == "SnakeHead") return;
        switch (colliderInfo.gameObject.tag)
        {
            case SnakeTags.Wall:
                ResetSnake(true);
                //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                break;
            case SnakeTags.Fruit:
                AddBody();
                if (snake.Count > _maxLength)
                    MaxLength = snake.Count;
				DestroyWallsByLigthning();
                UpdateUserHUD();
                break;
            case SnakeTags.SnakeBody:
                //if (!GameSettings.Instance.OfflineRules.WithSelfBody && !GameSettings.Instance.OfflineRules.WithEnemyBody) return;
                //int position = numberPartOfThisSnake(colliderInfo.gameObject);
                //if (GameSettings.Instance.OfflineRules.WithSelfBody && position > 2)
                    ResetSnake(false);
                //if (GameSettings.Instance.OfflineRules.WithEnemyBody && position == -1)
                //    ResetSnake(false);
                break;
            case SnakeTags.BrickWall:
                return;               
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

	void DestroyWallsByLigthning()
	{
		GameObject lightObj = GameObject.Find("PolyLightning");
		if (lightObj == null) return; // возможно играют в классическую змейку тогда объекта полимолния не будет в сцене
		PolyLightning ligth = lightObj.GetComponent<PolyLightning>();
		int countOfDestroyWalls = UnityEngine.Random.Range(1, 3);
		GameObject [] brickWalls = GameObject.FindGameObjectsWithTag(SnakeTags.BrickWall);
		if (brickWalls == null || brickWalls.Length == 0) return; // вдруг нету еще стен или они все разбиты, в этой жизни возможно все!
		if (countOfDestroyWalls > brickWalls.Length) // и даже момент когда нету столко стен сколько нам хотелось бы вхерачить молнией :)
			countOfDestroyWalls = brickWalls.Length;
		int wallPosition = UnityEngine.Random.Range(0, brickWalls.Length - countOfDestroyWalls); //ну а если есть то херачим по порядочку в котором они сгенерились
		for (int i = 0; i < countOfDestroyWalls; ++i)
			ligth.TargetPoints.Add(brickWalls[wallPosition + i]);
		
		ligth.StartLightningStroke();
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
        _currentPoints = 0;
        boom.StartAnimation(1.5f);
        RemoveBody(1.5f);
        speed = 0;
        UpdateUserHUD();
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

#if UNITY_WP8
        rotateAngle = _moveController.HandleTouch(Position, (int)Rotation);
#endif

#if UNITY_EDITOR_WIN || UNITY_WEBPLAYER
            if (Input.GetKey(_directionData.Left))
                rotateAngle = BasicDirections.Left;
            if (Input.GetKey(_directionData.Right))
                rotateAngle = BasicDirections.Right;
            if (Input.GetKey(_directionData.Up))
                rotateAngle = BasicDirections.Up;
            if (Input.GetKey(_directionData.Down))
                rotateAngle = BasicDirections.Down;
#endif

            
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
#if UNITY_EDITOR_WIN || UNITY_WEBPLAYER        
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
#endif
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

    public void AddBody()
    {
        //print("add body");
        ISnakePart part = _lastPart;
        SnakeBodySpan body = ((GameObject)Instantiate(SnakeBodyPrefab, Vector3.zero, Quaternion.identity)).GetComponent<SnakeBodySpan>();
        if (part is SnakeControllerOffline) //убираем коллайдер только с первой горошинф после головы что бы она не слала инфу про то что змея врезалась сама в себя
            Destroy(body.GetComponent<BoxCollider2D>());
        body.PreviousPart = part;
        snake.Add(body);
        _lastPart = body;
       // CalcSpeedUp(snake.Count);
        speed += CalcSpeedUp(snake.Count);
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
        string _groweMessage = GetRangeByLength(snake.Count);        
        if (_groweMessage == String.Empty) return;
        if (snake.Count > _maxLength)
            PlayerPrefs.SetString(PLAYER_RANGE_KEY,_groweMessage);
        _informer.AddMessage(new InformerMessage(_groweMessage, false, PlayGrownUpSound, true));
    }

    public static string GetRangeByLength(int snakeLength)
    {
        switch (snakeLength)
        {
            case 145: return "king of snake";
            case 135: return "MONSTER";
            case 125: return "anakonda";
            case 110: return "pythone";
            case 95: return "tiger python";
            case 80: return "cobra";
            case 65: return "boa";             
            case 50: return "black mamba";                
            case 35: return "bushmaster";                
            case 25: return "gurza";
            case 15: return "little snake";                
            case 10: return "worm";
            default: return string.Empty;
        }
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

