using Assets.Scripts;
using UnityEngine;
using System.Collections;
public class Fruit : MonoBehaviour {
	// Use this for initialization
    //SnakeController _snake;
    //SnakeController _snake2;
    public SnakeClient _client;
    public Number NumberPrefab;
    int _points = 0;
    public int Id { get; private set; }

    OTSprite _sprite;

	void Start () {
        //Debug.Log("/////////////////////////fruit start");
        //var snakes = GameObject.FindObjectsOfType(typeof(SnakeController));
        //_snake = (SnakeController)snakes[0];
        _client.FruitRepositioned += OnFruitRepositioned;                
        _sprite = this.transform.gameObject.GetComponent<OTSprite>();// поучаем компонент родителя через него будем отключать столкновения
        SwitchVisible(false);
        
        //_snake2 = (SnakeController)snakes[1];
	}

    void SwitchVisible(bool visible)
    {        
        renderer.enabled = visible;
        enabled = visible;
        _sprite.rigidbody.detectCollisions = visible;
    }
    
    void OnFruitRepositioned(FruitInfo fruitInfo)
    {
        Id = fruitInfo.ID;
        _points = fruitInfo.Points;
        transform.position = new Vector2(fruitInfo.X,fruitInfo.Y);
        SwitchVisible(true);
    }
	
	// Update is called once per frame
	void Update () {
	}
    
    void OnTriggerEnter(Collider colliderInfo)
    {
        //Debug.Log("colide");
        if (colliderInfo.gameObject.tag != "SnakeHead") return;
        SnakeController colideSnake = colliderInfo.gameObject.GetComponent<SnakeController>();
        if (colideSnake.IsEnemyInstance()) return;
        SwitchVisible(false);
        //Vector2 newPos;
        Number pointsNumber = (Number)Instantiate(NumberPrefab, new Vector3(this.transform.position.x,this.transform.position.y,-18), Quaternion.identity);
        audio.Play();
        /*int Counttries = 0;
        do 
        {
            Counttries++;
            float y = Random.Range(-9.0f, 9.0f);
            float x = Random.Range(-12.0f, 12.0f);
            newPos = new Vector2(x,y);
        } while (isColideWithSnake(newPos));*/
        // вычисляем количество очком кот будет начислено за подбор нового фрукта
        //int points = (int)(10 * Vector3.Distance(Vector3.zero, this.transform.position));
        
        //if (points < 10) points = 10; // но менее 10 быть не может
        pointsNumber.GetComponent<TextMesh>().text = _points.ToString();
        UpdatePointsCountOnLabel(_points, colliderInfo.gameObject);
        _client.SendCatchFruit(Id);
        //Debug.Log("counttries = " + Counttries);
    }

    

    void UpdatePointsCountOnLabel(int points, GameObject colideGameObject)
    {
        GameObject[] labels = GameObject.FindGameObjectsWithTag("PointLabel");
        SnakeController colideSnake = colideGameObject.GetComponent<SnakeController>();
        if (colideSnake.IsEnemyInstance()) return;
        TextMesh label = labels[colideSnake.LabelPosForThisSnake()].GetComponent<TextMesh>();
        
        //Debug.Log("update point cv = " + label.text);
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
