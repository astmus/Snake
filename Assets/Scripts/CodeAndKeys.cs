using UnityEngine;
using System.Collections;
public enum GameCode : byte
{
    /// <summary>
    ///   The operation code for the <see cref="JoinRequest">join</see> operation.
    /// </summary>
    Join = 255,

    /// <summary>
    ///   Operation code for the <see cref="LeaveRequest">leave</see> operation.
    /// </summary>
    Leave = 254,

    /// <summary>
    ///   Operation code for the <see cref="RaiseEventRequest">raise event</see> operation.
    /// </summary>
    RaiseEvent = 253,

    /// <summary>
    ///   Operation code for the <see cref="SetPropertiesRequest">set properties</see> operation.
    /// </summary>
    SetProperties = 252,

    /// <summary>
    ///   Operation code for the <see cref="GetPropertiesRequest">get properties</see> operation.
    /// </summary>
    GetProperties = 251,

    /// <summary>
    ///   Operation code for the ping operation.
    /// </summary>
    Ping = 249,

    /// <summary>
    /// send text message code
    /// </summary>
    SendMessage = 248,

    /// <summary>
    /// code for event enemy rotate head
    /// </summary>
    RotateHead = 247,

    CatchFruit = 246,

    UpdatedEnemyPosition = 245,

    SnakeGroweUp = 244,

    SnakeReset = 243
}

public enum GameMessageCodes : byte
{
    /// <summary>
    /// Message is an operatzion.
    /// </summary>
    Operation = 0,

    /// <summary>
    /// Message to remove peer from game.
    /// </summary>
    RemovePeerFromGame = 1,
}

public enum EventCode : byte
{
    /// <summary>
    ///   Specifies that no event code is set.
    /// </summary>
    NoCodeSet = 0,

    /// <summary>
    ///   The event code for the <see cref="JoinEvent"/>.
    /// </summary>
    Join = 255,

    /// <summary>
    ///   The event code for the <see cref="LeaveEvent"/>.
    /// </summary>
    Leave = 254,

    /// <summary>
    ///   The event code for the <see cref="PropertiesChangedEvent"/>.
    /// </summary>
    PropertiesChanged = 253,

    SendMessage = 248,

    RotateHead = 247,

    FruitReposition = 246,

    NewEnemySnakeSize = 245,

    EnemyPointsUpdated = 244,

    CountDownTick = 243,

    GameOver = 242,

    EnemySnakeReset = 241
}

public enum ParameterKey : byte
{
    RoomName = 255,

    ActorNumber = 254,

    Actors = 252,

    TextMessage = 240,

    RotateAngle = 239,

    CoordX = 238,

    CoordY = 237,

    FruitPoints = 236,

    FruitID = 235,

    FruitCatched = 234,

    UpdatedEnemyPosition = 233,

    SyncCoord = 232,

    SnakeLength = 231,

    PointsCount = 230,

    CountDownSec = 229,

    WinResult = 228,

    SnakeColideWithWall = 227
}
