using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.SurviveObjects
{
    internal class WallMaker : MonoBehaviour
    {
        private static WallMaker _instance;
        public GameObject _wallPrefab;
		List<Vector3> _availablePositions;       
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
				_availablePositions = new List<Vector3>(141);
				for (int i = -15; i <= 15; i += 2)
					for (int j = -9; j <= 9; j += 2)
						_availablePositions.Add(new Vector3(i, j, 0));
                OfflineGameStateController.GameStatusChanged += OnGameStatusChanged;
            }
        }

        void Start()
        {
        }

        void OnGameStatusChanged(GameStatus status)
        {
            switch(status)
            {
                case GameStatus.InGame: StartCoroutine("AddWall", TimeSpan.FromSeconds(4.5f / (int)GameSettings.Instance.DifficultOfCurrentGame));
                    break;
				case GameStatus.InRoom:
					StopCoroutine("AddWall");
					break;
            }
        }

        void OnDestroy()
        {
            OfflineGameStateController.GameStatusChanged -= OnGameStatusChanged;
        }

        void Update()
        {                      
            
        }

		static TimeSpan decrement = TimeSpan.FromMilliseconds(-50);
        static int i = 0;
        IEnumerator AddWall(TimeSpan interval)
        {   
            while (_availablePositions.Count > 0)
            {
				yield return new WaitForSeconds((float)interval.TotalSeconds);
				int vectPos = UnityEngine.Random.Range(0,_availablePositions.Count);
				Vector3 position = _availablePositions[vectPos];
				_availablePositions.Remove(position);
				GameObject brickWallObject = Instantiate(_wallPrefab, position,Quaternion.identity) as GameObject;
				BrickWall brickWall = brickWallObject.GetComponent<BrickWall>();
				brickWall.DestroyCallBack = OnBrickWallDestroyed;
								
                //2.061666
                //1.837083  
              	if (interval.TotalMilliseconds > (150 * (int)GameSettings.Instance.DifficultOfCurrentGame))
					interval += decrement;
				print(interval);
            }                       
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
