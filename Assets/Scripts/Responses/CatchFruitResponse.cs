using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Responses
{
    public class CatchFruitResponse
    {
        public bool Catched { set; get; }
        public CatchFruitResponse(Dictionary<byte, object> data)
        {
            Catched = (bool)data[(byte)ParameterKey.FruitCatched];
        }
    }
}
