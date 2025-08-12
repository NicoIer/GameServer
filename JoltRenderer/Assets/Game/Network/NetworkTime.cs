using System;
using Network.Time;
using UnityEngine;
using UnityToolkit;

namespace Network
{
    public class NetworkTime : MonoSingleton<NetworkTime>, IOnlyPlayingModelSingleton
    {
        protected override bool DontDestroyOnLoad() => true;

        private NetworkTimeClient _client;

        public double rttMs => _client.rttMs;

        // private Thread _worker;
        public string ip = "127.0.0.1";
        public int port = 24420;
        public bool autoRun = true;

        protected override void OnInit()
        {
            _client = new NetworkTimeClient();
            if (autoRun)
            {
                Run(ip, port);
            }
            // _worker = new Thread(() => { _client.Run(ip, port).Wait(); });
            // _worker.Start();
        }

        public void Run(string ip, int port)
        {
            _client.Stop();
            this.ip = ip;
            this.port = port;
            _ = _client.Run(this.ip, this.port);
        }

        public void Stop()
        {
            _client.Stop();
        }

        protected override void OnDispose()
        {
            // _worker.Abort();
            _client.Stop();
        }


        public bool onGui = true;

        private void OnGUI()
        {
            if (!onGui) return;
            GUILayout.BeginVertical();
            GUILayout.Label($"rtt:{_client.rttMs}");
            GUILayout.Label($"server:{DateTimeOffset.FromUnixTimeMilliseconds(_client.serverTimeMs):G}");
            GUILayout.EndHorizontal();
        }
    }
}