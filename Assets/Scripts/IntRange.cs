using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public struct IntRange
    {
        private readonly int _minVal;
        private readonly int _maxVal;
        private int _currentValue;
        public IntRange(int minVal, int maxVal)
        {
            _minVal = minVal;
            _maxVal = maxVal;
            _currentValue = _minVal;
        }

        public static implicit operator int(IntRange r)
        {
            return r._currentValue;
        }

        public static IntRange operator ++(IntRange r)
        {
            if (++r._currentValue > r._maxVal)
                r._currentValue = r._minVal;
            return  r;
        }
    }
}
