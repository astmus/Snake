using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class OfflineFruit : MonoBehaviour {

    // Use this for initialization
    //SnakeController _snake;
    //SnakeController _snake2;
    public Number NumberPrefab;
    public TextMesh PointLabel1;
    public TextMesh PointLabel2;
    int _points = 0;
    public int Id { get; private set; }
    private float _y;
    private float _x;
    private float _sizeX; // размер игрового поля по иксу
    private float _sizeY; // по игрику
    private float _minDistByY; // 
    private float _minDistByX; // 
    private float _minDistBetweenFruit;
    private Vector3 _oldPos = Vector3.zero; // previous position fruit
    private Vector3 _newPos;
    OTSprite _sprite;
    private Random _posGenerator;
    public OfflineGameStateController _gameStateController;

    void Start()
    {
        //Debug.Log("/////////////////////////fruit start");
        //var snakes = GameObject.FindObjectsOfType(typeof(SnakeController));
        //_snake = (SnakeController)snakes[0];
        _minDistBetweenFruit = 3;
        audio.volume = GameSettings.Instance.SoundsVolume;
        _sprite = transform.gameObject.GetComponent<OTSprite>();// поучаем компонент родителя через него будем отключать столкновения
        SwitchVisible(false);
        _posGenerator = new Random();
        Random.seed = Environment.TickCount;
        _gameStateController.GameStatusChanged += OnGameStatusChanged;
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
        renderer.enabled = visible;
        enabled = visible;
        _sprite.rigidbody.detectCollisions = visible;
    }

    void FruitReposition()
    {
        do
        {
            _y = Random.Range(-9,9);
            _x = Random.Range(-12,12);
            _newPos = new Vector3(_x, _y, 0);
        }
        while (Vector3.Distance(_newPos, _oldPos) < _minDistBetweenFruit);
        if (_points < 10) _points = 10; // но менее 10 быть не может
        _points = (int)(10 * Vector3.Distance(Vector3.zero, _newPos));
        transform.position = _newPos;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider colliderInfo)
    {
        //Debug.Log("colide");
        if (colliderInfo.gameObject.tag != "SnakeHead") return;
        SnakeControllerOffline colideSnake = colliderInfo.gameObject.GetComponent<SnakeControllerOffline>();
        //Vector2 newPos;
        Number pointsNumber = (Number)Instantiate(NumberPrefab, new Vector3(transform.position.x, transform.position.y, -18), Quaternion.identity);
        audio.Play();
        
        pointsNumber.GetComponent<TextMesh>().text = _points.ToString();
        UpdatePointsCountOnLabel(_points, colliderInfo.gameObject);
        FruitReposition();
        //Debug.Log("counttries = " + Counttries);
    }



    void UpdatePointsCountOnLabel(int points, GameObject colideGameObject)
    {
        SnakeControllerOffline colideSnake = colideGameObject.GetComponent<SnakeControllerOffline>();
        TextMesh label = colideSnake.PlayerNumber == 1 ? PointLabel1 : PointLabel2;
        int currentVal = System.Convert.ToInt32(label.text);
        currentVal += points;
        label.text = currentVal.ToString();
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
