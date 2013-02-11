using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using System;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Scripts.Responses;

public enum ConnectionStatus
{
    Disconnect = 0,
    Connect = 1,
    InRoom = 2,
    InGame = 3
}

public class SnakeClient : MonoBehaviour, IPhotonPeerListener
{
    LitePeer _peer;
    ConnectionStatus _connetionStatus;
    public event Action<RotateHeadData> RotateHead;
    public event Action<string> OpponentSendMessage;
    public event Action<FruitInfo> FruitRepositioned;
    public event Action<CatchFruitResponse> CatchFruitAnswer;
    public event Action EnemySnakeGrooveUp;

    public ConnectionStatus ConnetionStatus
    {
        get { return _connetionStatus; }
        private set { _connetionStatus = value; }
    }

    protected string ServerApplication = "ServerTest";
    private int _nextSendTickCount = Environment.TickCount;
    private bool _gameIsStarted = false;
    private static Dictionary<byte, object> parameters = new Dictionary<byte, object>();

    public int SendIntervalMs = 10;
    public string ServerAddress = "localhost:5055";
    public int ActorNumber;
    public bool _isDebugMode;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        _peer = new LitePeer(this, ConnectionProtocol.Udp);
        _isDebugMode = true;
        this.Connect();
	}

    public virtual void OnApplicationQuit()
    {        
        _peer.Disconnect();
    }

	// Update is called once per frame
	void Update () {
        if (Environment.TickCount > _nextSendTickCount)
        {
            _peer.Service();
            _nextSendTickCount = Environment.TickCount + this.SendIntervalMs;
        }	
	}

    internal virtual void Connect()
    {        
        // PhotonPeer.Connect() is described in the client reference doc: Photon-DotNet-Client-Documentation_v6-1-0.pdf
        Debug.Log("Connecting " + ServerAddress + ServerApplication);

        _peer.Connect(ServerAddress, ServerApplication);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (_isDebugMode == false) return;
        Debug.Log(message);
    }

    public void OnEvent(EventData eventData)
    {
        DebugReturn(DebugLevel.INFO, String.Format("OnEvent: {0}", eventData.ToStringFull()));
        switch (eventData.Code)
        {
            case (byte)EventCode.Join:                
                int[] actors = (int[])eventData.Parameters[(byte)ParameterKey.Actors]; 
                if (actors.Length == 2)
                    _connetionStatus = ConnectionStatus.InGame;
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
                if (EnemySnakeGrooveUp != null) EnemySnakeGrooveUp();
                break;                
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        DebugReturn(DebugLevel.INFO,String.Format("OnOperationResponse: {0}", operationResponse.ToStringFull()));
        switch (operationResponse.OperationCode)
        {
            case (byte)LiteOpCode.Join:
                //Debug.Log("OnOperationResponse == Join");
                _connetionStatus = ConnectionStatus.InRoom;
                JoinResponse response = new JoinResponse(operationResponse.Parameters);
                ActorNumber = response.ActorNumber;
                //this.ActorNumber = (int)operationResponse[(byte)LiteOpKey.ActorNr];
                break;
            case (byte)LiteOpCode.Leave:
                //this.State = ClientState.Connected;
                break;
            case (byte)GameCode.CatchFruit:
                Debug.Log("OnOperationResponse == CatchFruit");
                CatchFruitResponse catchResponse = new CatchFruitResponse(operationResponse.Parameters);
                if (CatchFruitAnswer != null) CatchFruitAnswer(catchResponse);
                break;
        }
    }

    public void SendRotateAngle(float angle, float syncCoord)
    {
        parameters.Clear();
        parameters.Add((Byte)ParameterKey.RotateAngle, angle);
        parameters.Add((Byte)ParameterKey.SyncCoord, syncCoord);
        _peer.OpCustom((byte)GameCode.RotateHead, parameters, true);
    }

    public void SendTextMessage(string message)
    {
        parameters.Clear();
        parameters.Add((Byte)ParameterKey.TextMessage, message);
        _peer.OpCustom((byte)GameCode.SendMessage, parameters, true);
    }

    public void SendCatchFruit(int fruitId)
    {
        parameters.Clear();
        parameters.Add((Byte)ParameterKey.FruitID, fruitId);
        _peer.OpCustom((byte)GameCode.CatchFruit, parameters, true);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        this.DebugReturn(DebugLevel.INFO,String.Format("OnStatusChanged: {0}", statusCode));

        switch (statusCode)
        {
            case StatusCode.Connect:
                _connetionStatus = ConnectionStatus.Connect;
                DebugReturn(DebugLevel.INFO, "Connected");
                _peer.OpJoin("");// комнату выдадут на сервере независимо от того что передадим параметром
                break;
            case StatusCode.Disconnect:
                DebugReturn(DebugLevel.INFO, "Discobbected");
                _connetionStatus = ConnectionStatus.Disconnect;
                this.ActorNumber = 0;
                break;
            case StatusCode.ExceptionOnConnect:
                DebugReturn(DebugLevel.ERROR,"Connection failed.\nIs the server online? Firewall open?");
                break;
            case StatusCode.SecurityExceptionOnConnect:
                DebugReturn(DebugLevel.ERROR,"Security Exception on connect.\nMost likely, the policy request failed.\nIs Photon and the Policy App running?");
                break;
            case StatusCode.Exception:
                DebugReturn(DebugLevel.ERROR,"Communication terminated by Exception.\nProbably the server shutdown locally.\nOr the network connection terminated.");
                break;
            case StatusCode.TimeoutDisconnect:
                DebugReturn(DebugLevel.INFO,"Disconnect due to timeout.\nProbably the server shutdown locally.\nOr the network connection terminated.");
                break;
            case StatusCode.DisconnectByServer:
                DebugReturn(DebugLevel.ERROR,"Timeout Disconnect by server.\nThe server did not get responses in time.");
                break;
            case StatusCode.DisconnectByServerLogic:
                DebugReturn(DebugLevel.ERROR,"Disconnect by server.\nThe servers logic (application) disconnected this client for some reason.");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                DebugReturn(DebugLevel.ERROR,"Server reached it's user limit.\nThe server is currently not accepting connections.\nThe license does not allow it.");
                break;
            default:
                this.DebugReturn(DebugLevel.INFO,"StatusCode not handled: " + statusCode);
                break;
        }
        
    }
}
