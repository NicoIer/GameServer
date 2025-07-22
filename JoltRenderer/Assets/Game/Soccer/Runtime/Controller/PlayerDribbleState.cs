using Jolt;
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
            var moveInput = owner.actions.Move.ReadValue<Vector2>();
            Quaternion rotation = owner.transform.rotation;
            Vector3 moveVec = owner.transform.forward * (owner.config.dribbleSpeed * Time.fixedDeltaTime);
            var position = owner.body.position + moveVec;
            if (moveInput.magnitude > 0.1f)
            {
                rotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
            }
            
            owner.body.SyncToPhysics(position, rotation);

            // var phy = owner.body.physicsWorld;
            // phy.physicsSystem.BodyInterface.SetPositionAndRotationWhenChanged(owner.body.bodyID, position, rotation,
                // Activation.Activate);
        }

        public void OnExit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }
    }
}