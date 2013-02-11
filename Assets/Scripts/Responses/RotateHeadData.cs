using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Responses
{
    public class RotateHeadData
    {
        public float [] RotateAngle { set; get; }
        public float [] CoordX { set; get; }
        public float[] CoordY { set; get; }
        public RotateHeadData(Dictionary<byte, object> data)
        {
            RotateAngle = (float[])data[(byte)ParameterKey.RotateAngle];
            //Debug.Log("angele length = "+RotateAngle.Length);
            CoordX = (float[])data[(byte)ParameterKey.CoordX];
            //Debug.Log("CoordX = "+CoordX.Length);
            CoordY = (float[])data[(byte)ParameterKey.CoordY];
            //Debug.Log("CoordY = "+CoordY.Length);
        }
    }
}
