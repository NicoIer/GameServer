using System.Numerics;
using GameCore.Soccer;

namespace Soccer;

public partial class SoccerGameServer
{
    public CmdUpdateInput redPlayerInput;
    public CmdUpdateInput bluePlayerInput;
    private void HandleCmd()
    {
        _server.messageHandler.Add<CmdUpdateInput>(OnCmdUpdateInput);
    }

    private void OnCmdUpdateInput(in int connectionId, in CmdUpdateInput message)
    {
        switch (message.identifier)
        {
            case IdentifierEnum.RedPlayer:
                redPlayerInput = message;
                break;
            case IdentifierEnum.BluePlayer:
                bluePlayerInput = message;
                break;
            default:
                return;
        }
    }
}