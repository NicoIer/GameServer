using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Network.Client;
using UnityEngine;
using UnityToolkit;

namespace Network
{
    public partial class NetworkCenter : MonoSingleton<NetworkCenter>
    {
        private NetworkClient _client;
        public string serverHost = "localhost";
        public ushort serverPort = 24419;
        public int maxRetries = int.MaxValue;
        public float retryDelay = 1.0f;

        public NetworkClientMessageHandler messageHandler => _client.messageHandler;
        // public static NetworkClientMessageHandler messageHandler=> Singleton._client.messageHandler;

        private float lastTryConnectTime;

        // public int countDisconnectCount;
        public bool continueConnect = true;
        public bool keepAlive = true;
        public bool autoConnect = true;
        public event Action OnDisconnectedEvent;
        public event Action OnConnectedEvent;

        private const float KEEP_ALIVE_INTERVAL = 5f;

        protected async override void OnInit()
        {
            Application.runInBackground = true;
            var socket = new TelepathyClientSocket();
            _client = new NetworkClient(socket);
            _client.socket.OnConnected += OnConnected;
            _client.socket.OnDisconnected += OnDisconnected;
            NetworkLoop.OnEarlyUpdate += OnEarlyUpdate;
            NetworkLoop.OnLateUpdate += OnLateUpdate;

            if (autoConnect)
            {
                StartConnect(serverHost, serverPort);
            }
        }
#if UNITY_EDITOR && ODIN_INSPECTOR_3
        [Sirenix.OdinInspector.Button]
        private void DebugConnect()
        {
            StartConnect(serverHost, serverPort);
        }
#endif

        public async void StartConnect(string host, ushort port)
        {
            if (_client.socket.connecting)
            {
                _client.Stop();
            }

            serverHost = host;
            serverPort = port;

            UriBuilder uriBuilder = new UriBuilder
            {
                Host = serverHost,
                Port = serverPort
            };
            await _client.Run(uriBuilder.Uri, false);
            lastTryConnectTime = UnityEngine.Time.time;
            _client.socket.TickOutgoing(); // 主动向服务器发送数据
            if (continueConnect && !_client.socket.connected)
            {
                await ContinueConnect(uriBuilder.Uri);
            }

            if (_client.socket.connected)
            {
                KeepAlive().Forget();
            }
        }


        private void OnDisconnected()
        {
            OnDisconnectedEvent?.Invoke();
        }

        private void OnConnected()
        {
            OnConnectedEvent?.Invoke();
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

            try
            {
                _client.Stop();
                _client.Dispose();
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void OnEarlyUpdate()
        {
            if (_client.socket.connecting || _client.socket.connected)
            {
                _client.socket.TickIncoming();
            }
        }

        private void OnLateUpdate()
        {
            if (_client.socket.connecting || _client.socket.connected)
            {
                _client.socket.TickOutgoing();
            }
        }

        public void Send<T>(in T msg) where T : INetworkMessage
        {
            _client.Send(in msg);
        }
    }
}