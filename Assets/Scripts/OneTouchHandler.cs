using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class OneTouchHandler : SnakeMoveHandler
    {
        protected override int GetNewAngle(Vector2 snakePosition, int currentAngle, Touch t)
        {
            switch (currentAngle)
            {

                case -90:
                case 270:
                    return (t.position.x < snakePosition.x) ? currentAngle - 90 : currentAngle + 90;
                case 90:
                    return (t.position.x < snakePosition.x) ? currentAngle + 90 : currentAngle - 90;
                case 0:
                    return (t.position.y < snakePosition.y) ? currentAngle - 90 : currentAngle + 90;                
                case 180:
                    return (t.position.y < snakePosition.y) ? currentAngle + 90 : currentAngle - 90;                
            }
            return -1;
        }
    }
}
