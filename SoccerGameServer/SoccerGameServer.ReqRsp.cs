using GameCore.Soccer;
using Network;

namespace Soccer;

public partial class SoccerGameServer
{
    private readonly ReqRspServerCenter _reqRspServerCenter = new ReqRspServerCenter();

    private int joinedPlayerCount = 0;

    public int redPlayerId;
    public int bluePlayerId;

    public void HandleReqRsp()
    {
        _server.messageHandler.Add<ReqHead>(OnReqBody);
        _reqRspServerCenter.Register<ReqJoinGame, RspJoinGame>(OnReqJoinGame);
        _server.socket.OnDisconnected += OnDisconnected;
    }

    private void OnDisconnected(int connectionId)
    {
        if (connectionId == redPlayerId)
        {
            --joinedPlayerCount;
            redPlayerId = 0;
        }
        else if (connectionId == bluePlayerId)
        {
            --joinedPlayerCount;
            bluePlayerId = 0;
        }
    }

    private void OnReqJoinGame(in int connectionId, in ReqJoinGame message, out RspJoinGame rsp,
        out ErrorCode errorCode, out string errorMsg)
    {
        if (joinedPlayerCount >= 2)
        {
            errorCode = ErrorCode.InvalidArgument;
            errorMsg = "Game is full, cannot join.";
            rsp = default;
        }
        else
        {
            ++joinedPlayerCount;
            errorCode = ErrorCode.Success;
            errorMsg = string.Empty;
            rsp = new RspJoinGame();
            if (joinedPlayerCount == 1)
            {
                redPlayerId = connectionId;
                rsp.identifier = IdentifierEnum.RedPlayer;
            }
            else if (joinedPlayerCount == 2)
            {
                bluePlayerId = connectionId;
                rsp.identifier = IdentifierEnum.BluePlayer;
            }
            else
            {
                errorCode = ErrorCode.InvalidArgument;
                errorMsg = "Unexpected player count.";
                rsp = default;
            }
        }
    }

    private void OnReqBody(in int connectionId, in ReqHead message)
    {
        var rsp = _reqRspServerCenter.HandleRequest(connectionId, message);
        _server.Send(connectionId, rsp);
    }
}