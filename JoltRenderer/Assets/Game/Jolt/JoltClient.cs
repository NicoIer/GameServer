using System;
using Cysharp.Threading.Tasks;
using GameCore.Jolt;
using Network;
using Network.Client;
using UnityEngine;
using UnityToolkit;

namespace Game.Jolt
{
    public partial class JoltClient : MonoSingleton<JoltClient>, IJoltRenderer
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

        protected async override void OnInit()
        {
            Application.runInBackground = true;
            if (!ShapeData.registered)
            {
                ShapeData.RegisterAll();
            }

            var socket = new TelepathyClientSocket();
            // socket.OnDisconnected += OnDisConnected;
            // socket.OnConnected += OnConnected;
            _client = new NetworkClient(socket);


            UriBuilder uriBuilder = new UriBuilder
            {
                Host = serverHost,
                Port = serverPort
            };
            await _client.Run(uriBuilder.Uri, false);
            lastTryConnectTime = Time.time;
            _client.socket.TickOutgoing(); // 主动向服务器发送数据
            ContinueConnect(uriBuilder.Uri).Forget();

            NetworkLoop.OnEarlyUpdate += OnEarlyUpdate;
            NetworkLoop.OnLateUpdate += OnLateUpdate;
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

                if (Time.time - lastTryConnectTime > retryDelay)
                {
                    _client.Stop();
                    lastTryConnectTime = Time.time;
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
            _client.messageHandler.Clear<WorldData>();
            _client.Stop();
            _client.Dispose();
        }

        private void OnEarlyUpdate()
        {
            _client.socket.TickIncoming();
        }

        private void OnLateUpdate()
        {
            _client.socket.TickOutgoing();
        }

        public void Send<T>(T msg) where T : INetworkMessage
        {
            _client.Send(msg);
        }
    }
}