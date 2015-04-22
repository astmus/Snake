using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameType : byte
    {
        SinglePlayer,
        MultyPlayer,
        Survive
    }

    public enum ControllerType : byte
    {
        OneTouch,
        TwoTouch,
        TwoTouchReverse
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
        const string CONTROLLER_TYPE_KEY = "controllerType";
        const string SOUND_VOLUME_KEY = "soundVolume";
        const string MUSIC_VOLUME_KEY = "musicVolume";
        GameSettings()
        {            
            ServerAddress = "54.228.222.73";
            //ServerAddress = "localhost";
            Port = "5055";
            MusicVolume = 0.8f;
            SoundsVolume = 0.8f;
            Player1Control = new KeyController();
            Player2Control = new KeyController(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
            OfflineRules = new OfflineGameRules();
            if (PlayerPrefs.HasKey(CONTROLLER_TYPE_KEY))
                _controllerType = (ControllerType)PlayerPrefs.GetInt(CONTROLLER_TYPE_KEY);
            if (PlayerPrefs.HasKey(SOUND_VOLUME_KEY))
                _soundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY);            
            if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
                _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);            
        }

        public OfflineGameRules OfflineRules { get; set; }

        float _musicVolume;
        public float MusicVolume 
        { 
            get { return _musicVolume;}
            set 
            {
                _musicVolume = value;
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _musicVolume);
            }
        }
        float _soundVolume;
        public float SoundsVolume
        {
            get { return _soundVolume; }
            set
            {
                _soundVolume = value;
                PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, _soundVolume);
            }
        }
        public string ServerAddress { set; get; }
        public string Port { set; get; }
        public KeyController Player1Control { set; get; }
        public KeyController Player2Control { set; get; }
        private GameDifficult _currentDifficult;
        public GameDifficult DifficultOfCurrentGame 
        { 
            set
            {
                if (value == GameDifficult.NotSelected) return;
                if (_currentDifficult == value) return;
                _currentDifficult = value;
                SpeedIncreaseFirst = 0.25f * (byte)_currentDifficult;
                SpeedIncreaseSecond = 0.08f * (byte)_currentDifficult;
                SpeedIncreaseThird = 0.03f * (byte)_currentDifficult;
            }
            get { return _currentDifficult; }
        }

        public float SpeedIncreaseFirst { get; private set; }
        public float SpeedIncreaseSecond { get; private set; }
        public float SpeedIncreaseThird { get; private set; }

        ControllerType _controllerType;
        public ControllerType ControllerType 
        {
            get { return _controllerType; }
            set {                
                _controllerType = value;
                PlayerPrefs.SetInt("controllerType",(int)value);
            }
        }
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get{return _instance ?? (_instance = new GameSettings());}
        }
    }
}
