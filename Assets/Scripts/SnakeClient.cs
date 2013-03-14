using System.Reflection;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using System;
using System.Collections.Generic;
using Assets.Scripts.Responses;
using Assets.Scripts.SendDataModel;

namespace Assets.Scripts
{
    public enum GameStatus
    {
        Disconnect = 0,
        Connect = 1,
        InRoom = 2,
        InGame = 3,
        GameOver = 4
    }

    public class SnakeConnection : LitePeer
    {
        public SnakeConnection()
            : base(ConnectionProtocol.Udp)
        {
        }

        public SnakeConnection(IPhotonPeerListener listener)
            : base(listener, ConnectionProtocol.Udp)
        {
        }

        public void ChangeListener(IPhotonPeerListener newListener)
        {
            this.Listener = newListener;
        }
    }

    public class SnakeClient : MonoBehaviour, IPhotonPeerListener
    {
        static SnakeConnection _connection;
        GameStatus _connectionStatus;

        public event Action<RotateHeadData> RotateHead;
        public event Action<string> OpponentSendMessage;
        public event Action<FruitInfo> FruitRepositioned;
        public event Action<CatchFruitResponse> CatchFruitAnswer;
        public event Action<EnemySnakeSizeChangeData> EnemySnakeGrooveUp;
        public event Action<int> EnemyPointsCountUpdated;
        public event Action<GameStatus> GameStatusChanged;
        public event Action<int> CountDownTick;
        public event Action<bool> GameOver;
        public event Action<bool> EnemySnakeReset;

        public GameStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            private set
            {
                _connectionStatus = value;
                if (GameStatusChanged != null)
                    GameStatusChanged(value);
            }
        }

        protected string ServerApplication = "ServerTest";
        private int _nextSendTickCount = Environment.TickCount;
        private bool _gameIsStarted = false;
        private static Dictionary<byte, object> parameters = new Dictionary<byte, object>();

        public int SendIntervalMs = 10;
        //public string ServerAddress = "localhost:5055";
        public int ActorNumber;
        public bool _isDebugMode;

        // Use this for initialization
        void Start()
        {

            Application.runInBackground = true;
            _isDebugMode = true;
            // initialize connection only if it null for situation when lplayer press button "play again"
            if (_connection == null)
                _connection = new SnakeConnection();
            _connection.ChangeListener(this);
            // if user play again then connection already established and we don't must call "Connect"
            if (_connection.PeerState == PeerStateValue.Disconnected)
            {
                //Debug.Log("Peer connect");
                Connect();
            }
            else // else we join to new game
            {
                //Debug.Log("peer join to new game");
                JoinToNewGame();
            }

        }

        public string ConnectionString()
        {
            return GameSettings.Instance.ServerAddress + ":" + GameSettings.Instance.Port;
        }

        public virtual void OnApplicationQuit()
        {
            _connection.Disconnect();
        }

        // Update is called once per frame
        void Update()
        {
            if (Environment.TickCount <= _nextSendTickCount) return;
            _connection.Service();
            _nextSendTickCount = Environment.TickCount + this.SendIntervalMs;
        }

        internal virtual void Connect()
        {
            // PhotonPeer.Connect() is described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf
            //Debug.Log("Connecting =" + ConnectionString());
            _connection.Connect(ConnectionString(), ServerApplication);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            if (_isDebugMode == false) return;
            Debug.Log(message);
        }

        public void OnEvent(EventData eventData)
        {
            //DebugReturn(DebugLevel.INFO, String.Format("OnEvent: {0}", eventData.ToStringFull()));
            switch (eventData.Code)
            {
                case (byte)EventCode.Join:
                    //Debug.Log("OnIvent Join");
                    int[] actors = (int[])eventData.Parameters[(byte)ParameterKey.Actors];
                    /*if (actors.Length == 2)
                    ConnectionStatus = GameStatus.InGame;*/
                    //Debug.Log("actors count = " + actors.Length);
                    break;
                case (byte)EventCode.Leave:
                    break;
                case (byte)EventCode.RotateHead:
                    if (RotateHead != null) RotateHead(new RotateHeadData(eventData.Parameters));
                    break;
                case (byte)EventCode.SendMessage:
                    if (OpponentSendMessage != null) OpponentSendMessage((string)eventData.Parameters[(byte)ParameterKey.TextMessage]);
                    break;
                case (byte)EventCode.FruitReposition:
                    if (FruitRepositioned != null) FruitRepositioned(new FruitInfo(eventData.Parameters));
                    break;
                case (byte)EventCode.NewEnemySnakeSize:
                    if (EnemySnakeGrooveUp != null) EnemySnakeGrooveUp(new EnemySnakeSizeChangeData(eventData.Parameters));
                    break;
                case (byte)EventCode.EnemyPointsUpdated:
                    if (EnemyPointsCountUpdated != null) EnemyPointsCountUpdated((int)eventData.Parameters[(byte)ParameterKey.PointsCount]);
                    break;
                case (byte)EventCode.CountDownTick:
                    int seconds = (int)eventData.Parameters[(byte)ParameterKey.CountDownSec];
                    if (seconds == 0) ConnectionStatus = GameStatus.InGame;
                    if (CountDownTick != null) CountDownTick(seconds);
                    break;
                case (byte)EventCode.GameOver:
                    ConnectionStatus = GameStatus.GameOver;
                    if (GameOver != null) GameOver((bool)eventData.Parameters[(byte)ParameterKey.WinResult]);
                    break;
                case (byte)EventCode.EnemySnakeReset:
                    if (EnemySnakeReset != null) EnemySnakeReset((bool)eventData.Parameters[(byte)ParameterKey.SnakeColideWithWall]);
                    break;
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            DebugReturn(DebugLevel.INFO, String.Format("OnOperationResponse: {0}", operationResponse.ToStringFull()));
            switch (operationResponse.OperationCode)
            {
                case (byte)LiteOpCode.Join:
                    //Debug.Log("OnOperationResponse == Join");
                    JoinResponse response = new JoinResponse(operationResponse.Parameters);
                    ActorNumber = response.ActorNumber;
                    ConnectionStatus = GameStatus.InRoom;
                    //this.ActorNumber = (int)operationResponse[(byte)LiteOpKey.ActorNr];
                    break;
                case (byte)LiteOpCode.Leave:
                    //this.State = ClientState.Connected;
                    break;
                case (byte)GameCode.CatchFruit:
                    //Debug.Log("OnOperationResponse == CatchFruit");
                    CatchFruitResponse catchResponse = new CatchFruitResponse(operationResponse.Parameters);
                    if (CatchFruitAnswer != null) CatchFruitAnswer(catchResponse);
                    SendSnakeGroweUp();
                    break;
            }
        }

        void SendSnakeGroweUp()
        {
            //Debug.Log("SnakeClient SendSnakeGroweUp");
            parameters.Clear();
            _connection.OpCustom((byte)GameCode.SnakeGroweUp, parameters, true);
        }

        public void SendSnakeReset(bool colideWithWall)
        {
            parameters.Clear();
            parameters.Add((byte)ParameterKey.SnakeColideWithWall,colideWithWall);
            _connection.OpCustom((byte)GameCode.SnakeReset, parameters, true);
        }

        public void SendSyncData(ISnakePart head, List<ISnakePart> body)
        {
            SnakeSyncData syncData = new SnakeSyncData(head);
            syncData.Add(body);
            parameters.Clear();
            syncData.DictionaryForSend(ref parameters);
            _connection.OpCustom((byte)GameCode.RotateHead, parameters, true);
        }

        public void SendTextMessage(string message)
        {
            parameters.Clear();
            parameters.Add((Byte)ParameterKey.TextMessage, message);
            _connection.OpCustom((byte)GameCode.SendMessage, parameters, true);
        }

        public void SendCatchFruit(int fruitId)
        {
            parameters.Clear();
            parameters.Add((Byte)ParameterKey.FruitID, fruitId);
            _connection.OpCustom((byte)GameCode.CatchFruit, parameters, true);
        }

        public void JoinToNewGame()
        {
            _connection.OpJoin("");// комнату выдадут на сервере независимо от того что передадим параметром
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            DebugReturn(DebugLevel.INFO, String.Format("OnStatusChanged: {0}", statusCode));

            switch (statusCode)
            {
                case StatusCode.Connect:
                    ConnectionStatus = GameStatus.Connect;
                    DebugReturn(DebugLevel.INFO, "Connected");
                    JoinToNewGame();
                    break;
                case StatusCode.Disconnect:
                    DebugReturn(DebugLevel.INFO, "Discobbected");
                    ConnectionStatus = GameStatus.Disconnect;
                    ActorNumber = 0;
                    break;
                case StatusCode.ExceptionOnConnect:
                    DebugReturn(DebugLevel.ERROR, "Connection failed.\nIs the server online? Firewall open?");
                    break;
                case StatusCode.SecurityExceptionOnConnect:
                    DebugReturn(DebugLevel.ERROR, "Security Exception on connect.\nMost likely, the policy request failed.\nIs Photon and the Policy App running?");
                    break;
                case StatusCode.Exception:
                    DebugReturn(DebugLevel.ERROR, "Communication terminated by Exception.\nProbably the server shutdown locally.\nOr the network connection terminated.");
                    break;
                case StatusCode.TimeoutDisconnect:
                    DebugReturn(DebugLevel.INFO, "Disconnect due to timeout.\nProbably the server shutdown locally.\nOr the network connection terminated.");
                    break;
                case StatusCode.DisconnectByServer:
                    DebugReturn(DebugLevel.ERROR, "Timeout Disconnect by server.\nThe server did not get responses in time.");
                    break;
                case StatusCode.DisconnectByServerLogic:
                    DebugReturn(DebugLevel.ERROR, "Disconnect by server.\nThe servers logic (application) disconnected this client for some reason.");
                    break;
                case StatusCode.DisconnectByServerUserLimit:
                    DebugReturn(DebugLevel.ERROR, "Server reached it's user limit.\nThe server is currently not accepting connections.\nThe license does not allow it.");
                    break;
                default:
                    DebugReturn(DebugLevel.INFO, "StatusCode not handled: " + statusCode);
                    break;
            }

        }
    }
}