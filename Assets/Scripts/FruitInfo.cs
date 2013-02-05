using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct FruitInfo
{
    public float X;
    public float Y;
    public int Points;
    public int ID;
    public FruitInfo(Dictionary<byte, object> data)
    {
        X = (float)data[(byte)ParameterKey.FruitNewX];
        Y = (float)data[(byte)ParameterKey.FruitNewY];
        Points = (int)data[(byte)ParameterKey.FruitPoints];
        ID = (int)data[(byte)ParameterKey.FruitID];
    }
}

