using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class JoinResponse
    {
        public JoinResponse(Dictionary<byte, object> param)
        {
            if (!IsValidJoinResponseData(param)) return;
            RoomName = (string)param[(byte)ParameterKey.RoomName];
            ActorNumber = (int)param[(byte)ParameterKey.ActorNumber];
        }

        bool IsValidJoinResponseData(Dictionary<byte, object> param)
        {
            if (param.ContainsKey((byte)ParameterKey.RoomName) && param.ContainsKey((byte)ParameterKey.ActorNumber))
                return true;
            else
                return false;
        }

        public string RoomName { get; set; }

        public int ActorNumber { get; set; }
    }
}
