using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameType
    {
        SinglePlayer,
        MultyPlayer,
        Survive
    }
    public class OfflineGameRules
    {
        public OfflineGameRules()
        {
            SnakeLengthEnabled = false;
            SnakeLengthWin = 15;
            PointsEnabled = true;
            PointsCountWin = 30000;
            GameType = GameType.MultyPlayer;
            WithEnemyBody = true;
            WithSelfBody = false;
        }
        public bool SnakeLengthEnabled { get; set; }
        public int SnakeLengthWin { get; set; }
        public bool PointsEnabled { get; set; }
        public int PointsCountWin { get; set; }
        public bool WithSelfBody { get; set; }
        public bool WithEnemyBody { get; set; }
        public GameType GameType { get; set; }
        public bool CheckGameWinSituation(SnakeControllerOffline snake)
        {
            if (GameType == GameType.Survive)
                throw new Exception("this function cant't be called when game type equal to survive");
            if (SnakeLengthEnabled && snake.SnakeBody.Count+1 >= SnakeLengthWin)
                return true;
            if (PointsEnabled && snake.PointsCount >= PointsCountWin)
                return true;
            return false;
        }

    }

    public class GameSettings
    {
        GameSettings()
        {
            ServerAddress = "54.228.222.73";
            //ServerAddress = "localhost";
            Port = "5055";
            MusicVolume = 0.5f;
            SoundsVolume = 0.5f;
            Player1Control = new KeyController();
            Player2Control = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
            OfflineRules = new OfflineGameRules();
        }

        public OfflineGameRules OfflineRules { get; set; }

        public float MusicVolume { get; set; }
        public float SoundsVolume { get; set; }
        public string ServerAddress { set; get; }
        public string Port { set; get; }
        public KeyController Player1Control { set; get; }
        public KeyController Player2Control { set; get; }
        private GameDifficult _currentDifficult;
        public GameDifficult DifficultOfCurrentGame 
        { 
            set
            {
                if (_currentDifficult == value) return;
                _currentDifficult = value;
                SpeedIncreaseFirst = 0.3f * (byte)_currentDifficult;
                SpeedIncreaseSecond = 0.1f * (byte)_currentDifficult;
                SpeedIncreaseThird = 0.05f * (byte)_currentDifficult;
            }
            get { return _currentDifficult; }
        }

        public float SpeedIncreaseFirst { get; private set; }
        public float SpeedIncreaseSecond { get; private set; }
        public float SpeedIncreaseThird { get; private set; }
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get{return _instance ?? (_instance = new GameSettings());}
        }
    }
}
