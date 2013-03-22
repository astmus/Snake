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
        MultyPlayer
    }
    public class OfflineGameRules
    {
        public OfflineGameRules()
        {
            SnakeLengthEnabled = false;
            SnakeLengthWin = 11;
            PointsEnabled = false;
            PointsCountWin = 3000;
            GameType = GameType.MultyPlayer;
            WithEnemyBody = false;
            WithSelfBody = false;
        }
        public bool SnakeLengthEnabled { get; set; }
        public int SnakeLengthWin { get; set; }
        public bool PointsEnabled { get; set; }
        public int PointsCountWin { get; set; }
        public bool WithSelfBody { get; set; }
        public bool WithEnemyBody { get; set; }
        public GameType GameType { get; set; }
    }

    public class GameSettings
    {
        GameSettings()
        {
            //ServerAddress = "54.228.214.178";
            ServerAddress = "localhost";
            Port = "5055";
            MusicVolume = 0.35f;
            SoundsVolume = 0.35f;
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

        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get{return _instance ?? (_instance = new GameSettings());}
        }
    }
}
