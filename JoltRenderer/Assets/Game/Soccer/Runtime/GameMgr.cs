using System;
using Network;
using UnityEngine;
using UnityToolkit;

namespace Soccer
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        public Transform soccerBall;
        public PlayerView redPlayer;
        public PlayerView bluePlayer;
        
        public WorldData worldData;

        protected override void OnInit()
        {
            NetworkCenter.Singleton.messageHandler.Add<WorldData>(OnWorldDataReceived);
        }

        private void OnWorldDataReceived(in WorldData message)
        {
            this.worldData = worldData;
            
            soccerBall.transform.position = message.soccer.position.T();
            soccerBall.transform.rotation = message.soccer.rotation.T();
            
            redPlayer.transform.position = message.redPlayer.position.T();
            redPlayer.transform.rotation = message.redPlayer.rotation.T();
            
            bluePlayer.transform.position = message.bluePlayer.position.T();
            bluePlayer.transform.rotation = message.bluePlayer.rotation.T();
        }

        private void OnGUI()
        {
            // 所有信息
            GUILayout.BeginVertical("box");
            GUILayout.Label($"Red Player Position: {worldData.redPlayer.position}");
            GUILayout.Label($"Red Player Rotation: {worldData.redPlayer.rotation}");
            GUILayout.Label($"Red Player Linear Velocity: {worldData.redPlayer.linearVelocity}");
            GUILayout.Label($"Red Player Angular Velocity: {worldData.redPlayer.angularVelocity}");
            GUILayout.Label($"Blue Player Position: {worldData.bluePlayer.position}");
            GUILayout.Label($"Blue Player Rotation: {worldData.bluePlayer.rotation}");
            GUILayout.Label($"Blue Player Linear Velocity: {worldData.bluePlayer.linearVelocity}");
            GUILayout.Label($"Blue Player Angular Velocity: {worldData.bluePlayer.angularVelocity}");
            GUILayout.Label($"Soccer Ball Position: {worldData.soccer.position}");
            GUILayout.Label($"Soccer Ball Rotation: {worldData.soccer.rotation}");
            GUILayout.Label($"Soccer Ball Linear Velocity: {worldData.soccer.linearVelocity}");
            GUILayout.Label($"Soccer Ball Angular Velocity: {worldData.soccer.angularVelocity}");
            GUILayout.EndVertical();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}