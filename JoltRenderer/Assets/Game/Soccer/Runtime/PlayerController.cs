using System;
using GameCore.Soccer;
using Network;
using UnityEngine;

namespace Soccer
{
    public class PlayerController : MonoBehaviour
    {
        public IdentifierEnum identifier;
        public InputSystem_Actions inputSystemActions;
        public InputSystem_Actions.PlayerActions playerActions => inputSystemActions.Player;

        // public Vector2 moveInput;

        private void Awake()
        {
            inputSystemActions = new InputSystem_Actions();
        }

        private void Update()
        {
            var moveInput = playerActions.Move.ReadValue<Vector2>();
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