using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    abstract class SnakeMoveHandler
    {
        protected float _halfScreenSize;

        public SnakeMoveHandler()
        {
            _halfScreenSize = Screen.width * 0.5f;
        }

        public int HandleTouch(Vector2 snakePosition, int currentAngle)
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                    return GetNewAngle(Camera.main.WorldToScreenPoint(snakePosition), currentAngle, t);
                else
                    return -1;
            }
            else
                return -1;
        }

        protected abstract int GetNewAngle(Vector2 snakePosition, int currentAngle, Touch t);        
    }
}
