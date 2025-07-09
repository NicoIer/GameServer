using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCore.Physics;
using Network.Physics;
using Network.Physics.Client;
using UnityEngine;
using UnityToolkit;

namespace Network.Physics
{
    public partial class NetworkCenter : MonoSingleton<NetworkCenter>
    {
        private NetworkClient _client;
        public string serverHost = "localhost";
        public ushort serverPort = 24419;
        public int maxRetries = int.MaxValue;
        public float retryDelay = 1.0f;

        public NetworkClientMessageHandler messageHandler => _client.messageHandler;

        private float lastTryConnectTime;

        // public int countDisconnectCount;
        public bool continueConnect = true;
        public bool keepAlive = true;

        private const float KEEP_ALIVE_INTERVAL = 5f;

        protected async override void OnInit()
        {
            Application.runInBackground = true;
            if (!ShapeDataPacket.registered)
            {
                ShapeDataPacket.RegisterAll();
            }

            var socket = new TelepathyClientSocket();
            // socket.OnDisconnected += OnDisConnected;
            // socket.OnConnected += OnConnected;
            _client = new NetworkClient(socket);
            _client.socket.OnConnected += OnConnected;
            _client.socket.OnDisconnected += OnDisConnected;


            UriBuilder uriBuilder = new UriBuilder
            {
                Host = serverHost,
                Port = serverPort
            };
            await _client.Run(uriBuilder.Uri, false);
            lastTryConnectTime = UnityEngine.Time.time;
            _client.socket.TickOutgoing(); // 主动向服务器发送数据
            ContinueConnect(uriBuilder.Uri).Forget();

            NetworkLoop.OnEarlyUpdate += OnEarlyUpdate;
            NetworkLoop.OnLateUpdate += OnLateUpdate;

            KeepAlive().Forget();
        }

        private void OnDisConnected()
        {
            
        }

        private void OnConnected()
        {
            
        }

        private async UniTask KeepAlive()
        {
            while (Application.isPlaying && keepAlive)
            {
                await Task.Delay(TimeSpan.FromSeconds(KEEP_ALIVE_INTERVAL));
                if (!_client.socket.connected) continue; // 没连接上
                ToolkitLog.Debug($"{nameof(NetworkCenter)}: Send Heartbeat");
                Send(HeartBeat.Default);
            }
        }


        private async UniTask ContinueConnect(Uri uri)
        {
            int retryCount = 0;
            while (Application.isPlaying && continueConnect && retryCount < maxRetries)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
                if (_client.socket.connected || _client.socket.connecting)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (UnityEngine.Time.time - lastTryConnectTime > retryDelay)
                {
                    ToolkitLog.Info($"{nameof(NetworkCenter)}: 连接断开 重试 {uri}");
                    _client.Stop();
                    lastTryConnectTime = UnityEngine.Time.time;
                    await _client.Run(uri, false);
                    _client.socket.TickOutgoing();
                    ++retryCount;
                }
            }
        }


        protected override void OnDispose()
        {
            NetworkLoop.OnEarlyUpdate -= OnEarlyUpdate;
            NetworkLoop.OnLateUpdate -= OnLateUpdate;
            continueConnect = false;
            // keepAlive = false;
            _client.messageHandler.Clear<WorldData>();
            _client.Stop();
            _client.Dispose();
        }

        private void OnEarlyUpdate()
        {
            // ToolkitLog.Info("OnEarlyUpdate");
            _client.socket.TickIncoming();
        }

        private void OnLateUpdate()
        {
            // ToolkitLog.Info("OnLateUpdate");
            _client.socket.TickOutgoing();
        }

        public void Send<T>(in T msg) where T : INetworkMessage
        {
            _client.Send(in msg);
        }
    }
}