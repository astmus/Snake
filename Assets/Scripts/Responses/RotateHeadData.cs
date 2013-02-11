using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Responses
{
    public class RotateHeadData
    {
        public float RotateAngle { set; get; }
        public float SyncCoord { set; get; }
        public RotateHeadData(Dictionary<byte, object> data)
        {
            RotateAngle = (float)data[(byte)ParameterKey.RotateAngle];
            SyncCoord = (float)data[(byte)ParameterKey.SyncCoord];
        }
    }
}
