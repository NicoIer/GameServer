using System.Numerics;
using GameCore.Soccer;

namespace Soccer;

public partial class SoccerGameServer
{
    public Vector2 redPlayerInput;
    public Vector2 bluePlayerInput;
    private void HandleCmd()
    {
        _server.messageHandler.Add<CmdMove>(OnCmdMove);
    }

    private void OnCmdMove(in int connectionId, in CmdMove message)
    {
        switch (message.identifier)
        {
            case IdentifierEnum.RedPlayer:
                redPlayerInput = message.moveInput;
                break;
            case IdentifierEnum.BluePlayer:
                bluePlayerInput = message.moveInput;
                break;
            default:
                return;
        }
    }
}