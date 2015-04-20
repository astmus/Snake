using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class TwoTouchHandler : SnakeMoveHandler
    {
        protected override int GetNewAngle(Vector2 snakePosition, int currentAngle, Touch t)
        {
            return (t.position.x > _halfScreenSize) ? currentAngle - 90 : currentAngle + 90;
        }
    }
}
