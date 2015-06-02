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
	private int _currentRankIndex;
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
    List<SnakeBodySpan> _snake;
    KeyController _directionData;
    public OfflineGameStateController _gameStateController;
    public OfflineFruit _fruit;
    public SoundManager _soundManager;
    private float _rotation;
    private float _startSpeed = 3f;
	private float _minimalSpeed;
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
	public float Speed
	{
		get { return speed; }
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
        get { return _snake; }
    }
    TargetPoint lastTurn;
    //public event Action<TargetPoint> PartRotate;
    // Use this for initialization
    void Start()
    {
        lastTurn = new TargetPoint();
        _snake = new List<SnakeBodySpan>();
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
		switch(GameSettings.Instance.CurrentGameType)
		{
			case GameType.SinglePlayer:
				PointsLabel.text = String.Format("{0}/{1} \n{2}/{3}", _currentPoints, _maxPoints, _snake.Count, _maxLength);
				break;
			case GameType.Survive:
				PointsLabel.text = String.Format("{0}/{1} \n{2}/{3}", _snake.Count, _maxLength, Math.Round(speed,2), _minimalSpeed);
				break;
		}        
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
		_minimalSpeed = (_startSpeed * 0.9f);
        speed = _startSpeed;
		UpdateUserHUD();
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

    //List<GameObject> _colidedwalls = new List<GameObject>();
    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case SnakeTags.Brick:				
                var body = collision.gameObject.GetComponent<Rigidbody2D>();
				body.AddForceAtPosition(AngleToVector(_rotation) * speed, collision.contacts[0].point, ForceMode2D.Impulse);
                break;  
        }
    }

    Vector2 AngleToVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Rad2Deg), -Mathf.Sin(angle * Mathf.Rad2Deg));
    }

    void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        //print(colliderInfo.gameObject.tag);
        if (colliderInfo.gameObject.tag == "SnakeHead") return;
        switch (colliderInfo.gameObject.tag)
        {
            case SnakeTags.Wall:
				switch (GameSettings.Instance.CurrentGameType)
				{
					case GameType.SinglePlayer:
						ResetSnake(true);
						break;
					case GameType.Survive:
						_gameStateController.StopGameAndDisplayeResult();
						break;
				}                
                //_snakeClient.SendSyncData(this, snake.Select(e => e as ISnakePart).ToList());
                break;
            case SnakeTags.Fruit:
                AddBody();
                if (_snake.Count > _maxLength)
                    MaxLength = _snake.Count;
				DestroyWallsByLigthning();
                UpdateUserHUD();
                break;
            case SnakeTags.SnakeBody:
                //if (!GameSettings.Instance.OfflineRules.WithSelfBody && !GameSettings.Instance.OfflineRules.WithEnemyBody) return;
                //int position = numberPartOfThisSnake(colliderInfo.gameObject);
                //if (GameSettings.Instance.OfflineRules.WithSelfBody && position > 2)
                switch(GameSettings.Instance.CurrentGameType)
				{
					case GameType.SinglePlayer:
						ResetSnake(false);
						break;
					case GameType.Survive:
						BiteOffTail(colliderInfo.gameObject);
						break;
				}
				
                //if (GameSettings.Instance.OfflineRules.WithEnemyBody && position == -1)
                //    ResetSnake(false);
                break;
            case SnakeTags.BrickWall:
				speed -= GameSettings.Instance.SurviveModeSpeedIncrease * 1.5f;
				print(speed);
				PlayColideWithWallSound();
				UpdateUserHUD();
				if (speed < _minimalSpeed)
					_gameStateController.StopGameAndDisplayeResult();
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

	

	void BiteOffTail(GameObject colidedBodyPart)
	{
		SnakeBodySpan body = colidedBodyPart.GetComponent<SnakeBodySpan>();
		int position = _snake.IndexOf(body);
		float speedDecrease = ((_snake.Count - 1) - position) * (GameSettings.Instance.SpeedIncreaseSecond * 0.5f);
		speed -= speedDecrease;
		RemoveBody(1.5f, position);
		UpdateUserHUD();
	}

	void DestroyWallsByLigthning()
	{
		GameObject lightObj = GameObject.Find("PolyLightning");
		if (lightObj == null) return; // возможно играют в классическую змейку тогда объекта полимолния не будет в сцене
		PolyLightning ligth = lightObj.GetComponent<PolyLightning>();
		int countOfDestroyWalls = (_snake.Count / 35) + 1;
		GameObject [] brickWalls = GameObject.FindGameObjectsWithTag(SnakeTags.BrickWall);
		if (brickWalls == null || brickWalls.Length == 0) return; // вдруг нету еще стен или они все разбиты, в этой жизни возможно все!
		if (countOfDestroyWalls > brickWalls.Length) // и даже момент когда нету столко стен сколько нам хотелось бы вхерачить молнией :)
			countOfDestroyWalls = brickWalls.Length;
		int wallPosition = UnityEngine.Random.Range(0, brickWalls.Length - countOfDestroyWalls); //ну а если есть то херачим по порядочку в котором они сгенерились
		for (int i = 0; i < countOfDestroyWalls; ++i)
		{
			GameObject wall = brickWalls[wallPosition + i];
			wall.tag = SnakeTags.ColidedBrickWall;
			ligth.TargetPoints.Add(wall);
		}		
		ligth.StartLightningStroke();
	}

    int numberPartOfThisSnake(GameObject part)
    {
        return _snake.FindIndex(b=>b.AsGameObject() == part);
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
    

    public void RemoveBody(float scaledTime, int fromPos = 0)
    {
        float timeAnimation = scaledTime / _snake.Count;
        int delayFactor = 0;
		while(_snake.Count != fromPos)
		{
			SnakeBodySpan body = _snake[fromPos];
			body.RemoveColiderAndRigibody();
			body.AnimationDestroy(timeAnimation, timeAnimation * delayFactor++);
			_snake.Remove(body);
		}
        _lastPart = fromPos == 0 ? this as ISnakePart : _snake[fromPos-1] as ISnakePart;
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
        if (Vector2.Distance(Position, lastTurn.Position) < 1) return false;

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
        if (_snake == null) return;
#endif
        foreach (SnakeBodySpan obj in _snake)
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
        if (part is SnakeControllerOffline) //убираем коллайдер только с первой горошин после головы что бы она не слала инфу про то что змея врезалась сама в себя
            Destroy(body.GetComponent<BoxCollider2D>());
        body.PreviousPart = part;
        _snake.Add(body);
        _lastPart = body;
       // CalcSpeedUp(snake.Count);
        speed += CalcSpeedUp(_snake.Count);
        //if (IsEnemyInstance()) return;
        DisplayGrownUpInfo();
    }

    float CalcSpeedUp(int snakeLength)
    { 
		switch(GameSettings.Instance.CurrentGameType)
		{
			case GameType.SinglePlayer:
				if (snakeLength < 5) return GameSettings.Instance.SpeedIncreaseFirst;
				if (snakeLength >= 5 && snakeLength < 13) return GameSettings.Instance.SpeedIncreaseSecond;
				else
					return GameSettings.Instance.SpeedIncreaseThird;		
			case GameType.Survive:
				return GameSettings.Instance.SurviveModeSpeedIncrease;
			default:
				return GameSettings.Instance.SpeedIncreaseFirst;
		}
    }

    void DisplayGrownUpInfo()
    {
        string _groweMessage = GetRangeByLength(_snake.Count);        
        if (_groweMessage == String.Empty) return;
        if (_snake.Count > _maxLength)
            PlayerPrefs.SetString(PLAYER_RANGE_KEY,_groweMessage);
		_currentRankIndex = GetRankPosition(_snake.Count);
        _informer.AddMessage(new InformerMessage(_groweMessage, false, PlayGrownUpSound, true));
    }

	static string[,] ranges = new string[,] { { "worm ", "(10)" }, { "little snake ", "(15)" }, { "gurza ", "(25)" }, { "bushmaster ", "(35)" }, { "black mamba ", "(50)" }, { "boa ", "(65)" },
	{ "cobra ", "(80)" }, { "tiger python ", "(95)" }, { "pythone ", "(110)" }, { "anakonda ", "(125)" }, { "MONSTER ", "(135)" }, { "king of snake ", "(145)" } };

	public string[,] SnakeRanges
	{
		get { return SnakeControllerOffline.ranges; }
	}
    public string GetRangeByLength(int snakeLength)
    {
		int pos = GetRankPosition(snakeLength);
        return pos == -1 ? String.Empty : ranges[pos, 0];
    }

	int GetRankPosition(int snakeLength)
	{
		switch (snakeLength)
		{
			case 145: return 11;
			case 135: return 10;
			case 125: return 9;
			case 110: return 8;
			case 95: return 7;
			case 80: return 6;
			case 65: return 5;
			case 50: return 4;
			case 35: return 3;
			case 25: return 2;
			case 15: return 1;
			case 10: return 0;
			default: return -1;
		}
	}

	public struct ResultRanks
	{
		public string CurrentRank;
		public string NextRank;
		public string PrevRank;
	}

	public ResultRanks GetResultRanks()
	{
		ResultRanks res = new ResultRanks();	
		int posNext = _currentRankIndex + 1;
		int posPrev = _currentRankIndex - 1;
		print(_currentRankIndex + " " + posNext + " " + posPrev);
		res.CurrentRank = String.Format("{0}({1})",ranges[_currentRankIndex,0], _snake.Count);
		res.NextRank = (posNext <= 11) ? String.Format("{0}{1}",ranges[posNext, 0], ranges[posNext, 1]) : String.Empty;
		res.PrevRank = (posPrev != -1) ? String.Format("{0}{1}", ranges[posPrev, 0], ranges[posPrev, 1]) : String.Empty;
		return res;
	}

    void PlayGrownUpSound()
    {
        _soundManager.PlaySound(SoundManagerClip.SnakeLevelUp);
    }

	void PlayColideWithWallSound()
	{
		_soundManager.PlaySound(SoundManagerClip.ColideWithBrickWall);
	}

    public Vector2 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
}

