using Network;
using UnityEngine;
using UnityToolkit;

namespace Game001
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        public Transform soccerBall;
        public PlayerController redPlayer;
        public PlayerController bluePlayer;

        protected override void OnInit()
        {
            NetworkCenter.Singleton.messageHandler.Add<WorldData>(OnWorldDataReceived);
        }

        private void OnWorldDataReceived(in WorldData message)
        {
            soccerBall.transform.position = message.soccer.position.T();
            soccerBall.transform.rotation = message.soccer.rotation.T();
            
            redPlayer.transform.position = message.redPlayer.position.T();
            redPlayer.transform.rotation = message.redPlayer.rotation.T();
            
            bluePlayer.transform.position = message.bluePlayer.position.T();
            bluePlayer.transform.rotation = message.bluePlayer.rotation.T();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}