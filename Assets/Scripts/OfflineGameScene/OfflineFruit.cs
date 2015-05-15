using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public enum FruitType
{
    Apple,
    Bannan,
    Blackberry,
    Orange,
    Raspberry,
    Watermelon,
    Strawbery
}

public class OfflineFruit : MonoBehaviour {

    // Use this for initialization
    //SnakeController _snake;
    //SnakeController _snake2;
    public Number NumberPrefab;
    public Sprite [] _spriteCollection;
    int _points = 0;
    public int Id { get; private set; }
    private float _y;
    private float _x;
    private float _sizeX; // ������ �������� ���� �� ����
    private float _sizeY; // �� ������
    private float _minDistByY; // 
    private float _minDistByX; // 
    private float _minDistBetweenFruit;
    private Vector3 _oldPos = Vector3.zero; // previous position fruit
    private Vector3 _newPos;
    
    //OTSprite _sprite;
    
    //public OfflineGameStateController _gameStateController;
    public UnityEngine.Vector3 CurrentPos
    {
        get { return _newPos; }
    }
    void Start()
    {
        //Debug.Log("/////////////////////////fruit start");
        //var snakes = GameObject.FindObjectsOfType(typeof(SnakeController));
        //_snake = (SnakeController)snakes[0];
        _minDistBetweenFruit = 4;
        GetComponent<AudioSource>().volume = GameSettings.Instance.SoundsVolume;
        //_sprite = transform.gameObject.GetComponent<OTSprite>();// ������� ��������� �������� ����� ���� ����� ��������� ������������
        //SwitchVisible(false);
        Random.seed = Environment.TickCount;
        OfflineGameStateController.GameStatusChanged += OnGameStatusChanged;
        //_snake2 = (SnakeController)snakes[1];
    }

    void OnGameStatusChanged(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.Disconnect:
                break;
            case GameStatus.Connect:
                break;
            case GameStatus.InRoom:
                break;
            case GameStatus.InGame:
                FruitReposition();
                SwitchVisible(true);
                break;
            case GameStatus.GameOver:
                break;
            case GameStatus.CountDown:
                break;
        }
    }

    void SwitchVisible(bool visible)
    {
        GetComponent<Renderer>().enabled = visible;
        enabled = visible;
        this.GetComponent<BoxCollider2D>().enabled = visible;
    }

    void FruitReposition()
    {
        do
        {
            _y = Random.Range(-9,9);
            _x = Random.Range(-15,15);
            _newPos = new Vector3(_x, _y, transform.position.z);
        }
        while (Vector3.Distance(_newPos, _oldPos) < _minDistBetweenFruit);
        _oldPos = _newPos;
        if (_points < 10) _points = 10;
        _points = (int)(10 * Vector3.Distance(Vector3.zero, _newPos)) * (int)GameSettings.Instance.DifficultOfCurrentGame;
        transform.position = _newPos;
        this.GetComponent<SpriteRenderer>().sprite = _spriteCollection[Random.Range(0, 6)];
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        //Debug.Log("colide");
        if (colliderInfo.gameObject.tag != "SnakeHead") return;
        SnakeControllerOffline colideSnake = colliderInfo.gameObject.GetComponent<SnakeControllerOffline>();
        //Vector2 newPos;
        Number pointsNumber = (Number)Instantiate(NumberPrefab, new Vector3(transform.position.x, transform.position.y, -18), Quaternion.identity);
        GetComponent<AudioSource>().Play();
        pointsNumber.GetComponent<TextMesh>().text = _points.ToString();
        colideSnake.PointsCount += _points;
        //if (_gameStateController.CheckWinRules(colideSnake) == false)
        FruitReposition();
        //Debug.Log("counttries = " + Counttries);
    }
   
    bool isColideWithSnake(Vector2 newPos)
    {
        /*if (Vector2.Distance(_snake.transform.position, newPos) < 3)
            return true;
        foreach (SnakeBodySpan body in _snake.SnakeBody)
            if (Vector2.Distance(body.AsGameObject().transform.position, newPos) < transform.localScale.x)
                return true;*/
        /*foreach (SnakeBodySpan body in _snake2.SnakeBody)
            if (Vector2.Distance(body.AsGameObject().transform.position, newPos) < transform.localScale.x)
                return true;
        */
        return false;
    }
}
