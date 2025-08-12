using System;
using GameCore.Soccer;
using Network;
using UnityEngine;

namespace Soccer
{
    public class PlayerInputSender : MonoBehaviour
    {
        public IdentifierEnum identifier;
        public InputSystem_Actions inputSystemActions;
        public InputSystem_Actions.PlayerActions playerActions => inputSystemActions.Player;
        

        private void Awake()
        {
            inputSystemActions = new InputSystem_Actions();
        }

        private void Update()
        {
            var moveInput = playerActions.Move.ReadValue<Vector2>();
            if (Application.isMobilePlatform)
            {
                moveInput = GameMgr.Singleton.joystick.Direction;
            }
            // Debug.Log($"joystick moveInput: {GameMgr.Singleton.joystick.Direction}");
            var kickPressed = playerActions.Attack.ReadValue<float>();
            NetworkCenter.Singleton.Send(new CmdUpdateInput(identifier,
                new System.Numerics.Vector2(moveInput.x, moveInput.y), kickPressed));
        }

        private void OnEnable()
        {
            inputSystemActions.Player.Enable();
        }

        private void OnDisable()
        {
            inputSystemActions.Player.Disable();
        }
    }
}