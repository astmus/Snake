using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Responses
{
    public class EnemySnakeSizeChangeData
    {
        public int NewSize { get; set; }
        public EnemySnakeSizeChangeData(Dictionary<byte, object> data)
        {
            NewSize = (int)data[(byte)ParameterKey.SnakeLength];            
        }
    }
}
