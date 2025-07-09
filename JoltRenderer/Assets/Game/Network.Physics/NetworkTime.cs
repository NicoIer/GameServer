using System;
using System.Threading;
using System.Threading.Tasks;
using Network.Physics.Time;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Network.Physics
{
    public class NetworkTime : MonoSingleton<NetworkTime>, IOnlyPlayingModelSingleton
    {
        protected override bool DontDestroyOnLoad() => true;

        private NetworkTimeClient _client;
        // private Thread _worker;
        public string ip = "127.0.0.1";
        public int port = 24420;

        protected override void OnInit()
        {
            _client = new NetworkTimeClient();
            _ = _client.Run(ip, port);
            // _worker = new Thread(() => { _client.Run(ip, port).Wait(); });
            // _worker.Start();
        }


        protected override void OnDispose()
        {
            // _worker.Abort();
            _client.Stop();
        }
#if UNITY_EDITOR

        public bool onGui = true;

        private void OnGUI()
        {
            if (!onGui) return;
            GUILayout.BeginVertical();
            GUILayout.Label($"rtt:{_client.rttMs}");
            GUILayout.Label($"server:{DateTimeOffset.FromUnixTimeMilliseconds(_client.serverTimeMs):G}");
            GUILayout.EndHorizontal();
        }

#endif
    }
}