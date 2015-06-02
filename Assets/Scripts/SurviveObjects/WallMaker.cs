using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.SurviveObjects
{
    public class WallMaker : MonoBehaviour
    {
        private static WallMaker _instance;
        public GameObject _wallPrefab;
		List<Vector3> _availablePositions;
		SnakeControllerOffline _snake;
		float _perfectSnakeSpeed; // идеальная сокрость змеи как если бы она ни разу не врезалась в кирпичную стену		
		float _deltaDelay = -1;
		OfflineFruit _fruit;
		float _leftTimeForCreateBrickWall;
		float _wallAddInterval; // в секундочках
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
				_availablePositions = new List<Vector3>(100);
				for (float i = -15; i <= 15; i += 3f)
					for (float j = -8; j <= 10; j += 2.7f)
						_availablePositions.Add(new Vector3(i, j, 0));
				_snake = GameObject.FindGameObjectWithTag(SnakeTags.SnakeHead).GetComponent<SnakeControllerOffline>();
				_fruit = GameObject.FindObjectOfType<OfflineFruit>();
				_fruit.FruitRepositionedTo += OnFruitRepositionedTo;
                OfflineGameStateController.GameStatusChanged += OnGameStatusChanged;
            }
        }
		/*public float LeftTimeForCreateBrickWall
		{
			get { return _leftTimeForCreateBrickWall; }
			set { _leftTimeForCreateBrickWall = value; }
		}*/

		object _lockForWallDelay = new object();		
		bool _coroutineIsStarted = false;
		void OnFruitRepositionedTo(Vector3 newPosition)
		{						
			float xdist = Math.Abs(_snake.transform.position.x - newPosition.x);
			float ydist = Math.Abs(_snake.transform.position.y - newPosition.y);
			_deltaDelay = (xdist + ydist) / _perfectSnakeSpeed;
			lock (_lockForWallDelay)
			{
				_wallAddInterval = _deltaDelay < 1 ? 1 : _deltaDelay;
			}
			if (!_coroutineIsStarted)
			{
				StartCoroutine("AddWall");
				_coroutineIsStarted = true;
			}
			_perfectSnakeSpeed += GameSettings.Instance.SurviveModeSpeedIncrease;
		}

        void Start()
		{			
        }

        void OnGameStatusChanged(GameStatus status)
        {
            switch(status)
            {
                case GameStatus.InGame:
					_perfectSnakeSpeed = _snake.Speed;
					AddWallImmediately().SwitchFromTransparent();
                    break;
				case GameStatus.InRoom:
					StopCoroutine("AddWall");
					break;
            }
        }

        void OnDestroy()
        {
            OfflineGameStateController.GameStatusChanged -= OnGameStatusChanged;
			_fruit.FruitRepositionedTo -= OnFruitRepositionedTo;
        }

        void Update()
        {
			/*if (_deltaDelay < 0) return;
			_deltaDelay -= Time.deltaTime;
			if (_deltaDelay < 0)
			{

				StartCoroutine("AddWall");
				_deltaDelay = (float)_lastCalculatedWallCreateDelay.TotalSeconds;
			}*/
        }
		

        IEnumerator AddWall()
        {
			while (_wallAddInterval > 0)
			{
				BrickWall wall = AddWallImmediately();
				wall.CountDownTime = _wallAddInterval;
				yield return new WaitForSeconds(_wallAddInterval);
				wall.SwitchFromTransparent();
			}							
        }

		BrickWall AddWallImmediately()
		{
			if (_availablePositions.Count > 0)
			{
				Vector3 position;
				int vectPos = UnityEngine.Random.Range(0, _availablePositions.Count);
				position = _availablePositions[vectPos];
				_availablePositions.Remove(position);
				GameObject brickWallObject = Instantiate(_wallPrefab, position, Quaternion.identity) as GameObject;
				BrickWall brickWall = brickWallObject.GetComponent<BrickWall>();
				brickWall.DestroyCallBack = OnBrickWallDestroyed;

				return brickWall;
			}
			else return null;
		}
		
		static object lockobj = new object();
		void OnBrickWallDestroyed(Vector3 usedPosition)
		{
			lock(lockobj)
			{
				_availablePositions.Add(usedPosition);
			}
		}
    }
}
