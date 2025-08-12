using System;
using GameCore.Soccer;
using Network;
using Soccer.UI;
using TMPro;
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

        // private int redScore;
        // private int blueScore;

        public FindServerPanel findServerPanel;
        public GamePlayPanel gamePlayPanel;

        public PlayerInputSender playerInputSender;
        public GameNetworkReqRsp reqRsp;

        protected override void OnInit()
        {
            Application.targetFrameRate = -1;
            NetworkCenter.Singleton.OnDisconnectedEvent += OnDisconnected;
            NetworkCenter.Singleton.OnConnectedEvent += OnConnected;
            NetworkCenter.Singleton.messageHandler.Add<WorldData>(OnWorldDataReceived);
            NetworkCenter.Singleton.messageHandler.Add<RpcPlayerGoal>(OnRpcPlayerGoal);

            gamePlayPanel.gameObject.SetActive(false);
            findServerPanel.gameObject.SetActive(true);
            
            // 允许多点触控
            if (Application.isMobilePlatform)
            {
                Input.multiTouchEnabled = true;
            }
        }

        private async void OnConnected()
        {
            findServerPanel.gameObject.SetActive(false);
            gamePlayPanel.gameObject.SetActive(true);

            var (rsp, ok) = await reqRsp.Request<ReqJoinGame, RspJoinGame>(new ReqJoinGame());
            if (ok)
            {
                switch (rsp.identifier)
                {
                    case IdentifierEnum.RedPlayer:
                        playerInputSender = redPlayer.gameObject.AddComponent<PlayerInputSender>();
                        playerInputSender.identifier = IdentifierEnum.RedPlayer;
                        break;
                    case IdentifierEnum.BluePlayer:
                        playerInputSender = bluePlayer.gameObject.AddComponent<PlayerInputSender>();
                        playerInputSender.identifier = IdentifierEnum.BluePlayer;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnDisconnected()
        {
            findServerPanel.gameObject.SetActive(true);
            gamePlayPanel.gameObject.SetActive(false);
            if (playerInputSender != null)
            {
                Destroy(playerInputSender);
            }

            NetworkTime.Singleton.Stop();
        }

        public void JoinGame(string serverAddress, ushort port, ushort timeServerPort)
        {
            NetworkCenter.Singleton.StartConnect(
                serverAddress,
                port);
            NetworkTime.Singleton.Run(serverAddress, timeServerPort);
        }

        private void OnRpcPlayerGoal(in RpcPlayerGoal message)
        {
            Debug.Log($"Player Goal: {message.identifier}");
        }

        private void OnWorldDataReceived(in WorldData message)
        {
            this.worldData = worldData;

            gamePlayPanel.UpdateWorldData(message);
            soccerBall.transform.position = message.soccer.position.T();
            soccerBall.transform.rotation = message.soccer.rotation.T();

            redPlayer.transform.position = message.redPlayer.position.T();
            redPlayer.transform.rotation = message.redPlayer.rotation.T();

            bluePlayer.transform.position = message.bluePlayer.position.T();
            bluePlayer.transform.rotation = message.bluePlayer.rotation.T();
        }

        // private void OnGUI()
        // {
        //     // 所有信息
        //     GUILayout.BeginVertical("box");
        //     GUILayout.Label($"Red Player Position: {worldData.redPlayer.position}");
        //     GUILayout.Label($"Red Player Rotation: {worldData.redPlayer.rotation}");
        //     GUILayout.Label($"Red Player Linear Velocity: {worldData.redPlayer.linearVelocity}");
        //     GUILayout.Label($"Red Player Angular Velocity: {worldData.redPlayer.angularVelocity}");
        //     GUILayout.Label($"Blue Player Position: {worldData.bluePlayer.position}");
        //     GUILayout.Label($"Blue Player Rotation: {worldData.bluePlayer.rotation}");
        //     GUILayout.Label($"Blue Player Linear Velocity: {worldData.bluePlayer.linearVelocity}");
        //     GUILayout.Label($"Blue Player Angular Velocity: {worldData.bluePlayer.angularVelocity}");
        //     GUILayout.Label($"Soccer Ball Position: {worldData.soccer.position}");
        //     GUILayout.Label($"Soccer Ball Rotation: {worldData.soccer.rotation}");
        //     GUILayout.Label($"Soccer Ball Linear Velocity: {worldData.soccer.linearVelocity}");
        //     GUILayout.Label($"Soccer Ball Angular Velocity: {worldData.soccer.angularVelocity}");
        //     GUILayout.EndVertical();
        // }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}