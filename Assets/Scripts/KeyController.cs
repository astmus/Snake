using UnityEngine;
using System.Collections;
using System;

public class KeyController
{
    KeyCode _left;
    public KeyCode Left
    {
        get { return _left; }
        set { _left = value; }
    }

    KeyCode _right;
    public KeyCode Right
    {
        get { return _right; }
        set { _right = value; }
    }

    KeyCode _up;
    public KeyCode Up
    {
        get { return _up; }
        set { _up = value; }
    }

    KeyCode _down;
    public KeyCode Down
    {
        get { return _down; }
        set { _down = value; }
    }
    public KeyController()
    {
        _left = KeyCode.LeftArrow;
        _right = KeyCode.RightArrow;
        _up = KeyCode.UpArrow;
        _down = KeyCode.DownArrow;
    }

    public KeyController(KeyCode left, KeyCode right, KeyCode top, KeyCode down)
    {
        _left = left;
        _down = down;
        _up = top;
        _right = right;    
    }
}
