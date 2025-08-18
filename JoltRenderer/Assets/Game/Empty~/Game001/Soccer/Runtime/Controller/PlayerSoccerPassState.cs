using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityToolkit;

namespace Soccer
{
    public class PlayerSoccerPassState : IState<PlayerController>
    {
        public bool over;
        private AnimationClip _soccerClip;
        private float _soccerClipLengthSeconds;
        public void OnInit(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            foreach (var animationClip in owner.animator.runtimeAnimatorController.animationClips)
            {
                if(animationClip.name == "Soccer Pass")
                {
                    _soccerClip = animationClip;
                    break;
                }
            }

            _soccerClipLengthSeconds = _soccerClip.length;
        }

        public async void OnEnter(PlayerController owner, IStateMachine<PlayerController> stateMachine)
        {
            over = false;
            owner.animator.Play("Soccer Pass");
            await UniTask.WaitForSeconds(_soccerClipLengthSeconds);
            over = true;
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