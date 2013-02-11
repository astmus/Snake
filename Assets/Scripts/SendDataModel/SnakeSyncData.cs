using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Assets.Scripts.SendDataModel
{
    public class SnakeSyncData
    {   
        List<float> _xcoord;
        List<float> _ycoord;
        List<float> _angle;

        SnakeSyncData()
        {            
            _xcoord = new List<float>();
            _ycoord = new List<float>();
            _angle = new List<float>();
        }

        public SnakeSyncData(ISnakePart part)
            : this()
        {
            _xcoord.Add(part.Position.x);
            _ycoord.Add(part.Position.y);
            _angle.Add(part.Rotation);
        }
        public SnakeSyncData(List<ISnakePart> parts)
            : this()
        {
            foreach (ISnakePart part in parts)
            {
                _xcoord.Add(part.Position.x);
                _ycoord.Add(part.Position.y);
                _angle.Add(part.Rotation);
            }
        }

        public void Add(ISnakePart part)
        {
            _xcoord.Add(part.Position.x);
            _ycoord.Add(part.Position.y);
            _angle.Add(part.Rotation);
        }

        public void Add(List<ISnakePart> parts)
        {
            foreach (ISnakePart part in parts)
            {
                _xcoord.Add(part.Position.x);
                _ycoord.Add(part.Position.y);
                _angle.Add(part.Rotation);
            }
        }

        public void DictionaryForSend(ref Dictionary<byte, object> res)
        {            
            res.Add((Byte)ParameterKey.RotateAngle, _angle.ToArray());
            res.Add((Byte)ParameterKey.CoordX, _xcoord.ToArray());
            res.Add((Byte)ParameterKey.CoordY, _ycoord.ToArray());
        }
    }
}
