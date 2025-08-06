
using System.Numerics;
using Network;
using Soccer;

namespace GameCore.Soccer
{
    public partial struct CmdMove : INetworkMessage
    {
        public IdentifierEnum identifier;
        public Vector2 moveInput;
        public CmdMove(IdentifierEnum identifier, Vector2 moveInput)
        {
            this.identifier = identifier;
            this.moveInput = moveInput;
        }
    }
}