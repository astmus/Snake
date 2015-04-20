using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class TwoTouchReverseHandler : SnakeMoveHandler
    {
        protected override int GetNewAngle(Vector2 snakePosition, int currentAngle, Touch t)
        {
            switch (currentAngle)
            {
                case -90:
                case 270:
                    return (t.position.x < _halfScreenSize) ? currentAngle - 90 : currentAngle + 90;
                // часть кода для "умных" поворотов
                /*case 0:
                    rotateAngle = (Position.y > _fruit.CurrentPos.y) ? Rotation - 90 : Rotation + 90;
                    break;
                case 180:
                    rotateAngle = (Position.y < _fruit.CurrentPos.y) ? Rotation - 90 : Rotation + 90;
                    break;*/
                default:
                    return (t.position.x > _halfScreenSize) ? currentAngle - 90 : currentAngle + 90;
            }
        }
    }
}
