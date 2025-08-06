
using System.Numerics;
using Network;
using Soccer;

namespace GameCore.Soccer
{
    public partial struct CmdUpdateInput : INetworkMessage
    {
        public IdentifierEnum identifier;
        public Vector2 moveInput;
        public float kickPressed;
        public CmdUpdateInput(IdentifierEnum identifier, Vector2 moveInput, float kickPressed)
        {
            this.identifier = identifier;
            this.moveInput = moveInput;
            this.kickPressed = kickPressed;
        }
    }
}