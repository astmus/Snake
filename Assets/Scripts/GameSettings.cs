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
            ServerAddress = "54.228.214.178";
            Port = "5055";
        }

        public string ServerAddress { set; get; }
        public string Port { set; get; }
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get{return _instance ?? (_instance = new GameSettings());}
        }
    }
}
