using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class GameSettings
    {
        GameSettings()
        {
            //ServerAddress = "54.228.214.178";
            ServerAddress = "localhost";
            Port = "5055";
            MusicVolume = 0.8f;
            SoundsVolume = 0.8f;
        }

        public float MusicVolume { get; set; }
        public float SoundsVolume { get; set; }
        public string ServerAddress { set; get; }
        public string Port { set; get; }
        
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get{return _instance ?? (_instance = new GameSettings());}
        }
    }
}
