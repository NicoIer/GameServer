using System;
using JoltWrapper;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolkit;

namespace Soccer
{
    public class PlayerController : MonoBehaviour
    // , InputSystem_Actions.IPlayerActions
    {
        public InputSystem_Actions inputSystemActions { get; private set; }
        public InputSystem_Actions.PlayerActions actions => inputSystemActions.Player;

        public StateMachine<PlayerController> stateMachine { get; private set; }

        [field: SerializeField] public PlayerConfig config { get; private set; }

        [field: SerializeField] public Animator animator { get; private set; }
        [field: SerializeField] public JoltBody body { get; private set; }

        [field: SerializeField] public CinemachineCamera virtualCamera { get; private set; }
        [field: SerializeField] public bool isLocalPlayer { get; private set; } = true;

        private void Awake()
        {
            inputSystemActions = new InputSystem_Actions();
            // actions.SetCallbacks(this);
            stateMachine = new StateMachine<PlayerController>(this);
            stateMachine.Add<PlayerIdleState>();
            stateMachine.Add<PlayerDribbleState>();
            stateMachine.Add<PlayerSoccerPassState>();

            stateMachine.Run<PlayerIdleState>();
        }

        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        private void Update()
        {
            UpdateState();
            stateMachine.OnUpdate();
        }

        private void UpdateState()
        {
            // 状态切换逻辑
            var currentState = stateMachine.currentState;
            if (currentState is PlayerSoccerPassState { over: false })
                return; // 如果当前状态是射门状态 && 未结束射门，则不进行其他状态的切换 射门是一个非持续的短暂状态
            if (actions.Attack.triggered)
            {
                // 切换到射门状态
                stateMachine.Change<PlayerSoccerPassState>();
                return;
            }

            if (!actions.Move.inProgress && currentState is not PlayerIdleState) // 如果没有移动输入且当前状态不是待机状态，则切换到待机状态
            {
                stateMachine.Change<PlayerIdleState>();
                return;
            }

            if (actions.Move.inProgress && currentState is not PlayerDribbleState) // 如果有移动输入且当前状态不是跑动状态，则切换到跑动状态
            {
                stateMachine.Change<PlayerDribbleState>();
                return;
            }
        }

        #region Input System Callbacks

        //
        // public void OnMove(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnLook(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnAttack(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnInteract(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnCrouch(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnJump(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnPrevious(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnNext(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public void OnSprint(InputAction.CallbackContext context)
        // {
        //     throw new NotImplementedException();
        // }
        //

        #endregion
    }
}