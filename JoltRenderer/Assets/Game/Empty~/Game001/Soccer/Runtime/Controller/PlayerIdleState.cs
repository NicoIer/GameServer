using UnityToolkit;

namespace Soccer
{
    public class PlayerIdleState : IState<PlayerController>
    {
        public void OnInit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
        }

        public void OnEnter(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            owner.animator.Play("Idle");
        }

        public void Transition(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnUpdate(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnExit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            // throw new System.NotImplementedException();
        }
    }
}