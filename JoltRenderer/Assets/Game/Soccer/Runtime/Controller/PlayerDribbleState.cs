using Jolt;
using JoltWrapper;
using Unity.Mathematics;
using UnityEngine;
using UnityToolkit;

namespace Soccer
{
    public class PlayerDribbleState : IState<PlayerController>
    {
        public void OnInit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnEnter(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            owner.animator.Play("Dribble");
        }

        public void Transition(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnUpdate(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // var moveInput = owner.actions.Move.ReadValue<Vector2>();
            Quaternion rotation = owner.transform.rotation;
            
            // 玩家的朝向要和相机的朝向一致
            if (owner.virtualCamera != null)
            {
                rotation = Quaternion.LookRotation(owner.virtualCamera.transform.forward, Vector3.up);
            }
            
            // rotation只考虑Y轴的旋转
            rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
            
            Vector3 moveVec = owner.transform.forward * (owner.config.dribbleSpeed * Time.fixedDeltaTime);
            var position = owner.body.position + moveVec;
            owner.body.SetPositionAndRotation(position, rotation);
        }

        public void OnExit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }
    }
}