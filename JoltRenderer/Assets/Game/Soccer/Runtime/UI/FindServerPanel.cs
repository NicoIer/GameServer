using System;
using System.Collections.Concurrent;
using System.Net;
using MemoryPack;
using Network;
using Network.Server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soccer.UI
{
    public class FindServerPanel : MonoBehaviour
    {
        public TextMeshProUGUI infoText;
        private LocalNetwork localNetwork;
        public int port = 24421;
        public bool hasFindServer = false;
        public ServerInfo finedServerInfo;

        private ConcurrentQueue<ArraySegment<byte>> _buffer = new ConcurrentQueue<ArraySegment<byte>>();

        private void Awake()
        {
            localNetwork = new LocalNetwork(port, null, OnDataReceived);
            infoText.text = "Searching for server...";
        }


        private void OnDataReceived(in ArraySegment<byte> data, in IPEndPoint sender)
        {
            if (hasFindServer) return;
            try
            {
                _buffer.Enqueue(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize ServerInfo: {e.Message}");
                return;
            }
        }

        private void Update()
        {
            if(Time.frameCount % 30 != 0) return; // Update every 30 frames
            if (Time.frameCount % 30 == 0)
            {
                infoText.text = $"Searching for server..";
            }
            if (Time.frameCount % 60 == 0)
            {
                infoText.text = $"Searching for server...";
            }
            if (Time.frameCount % 90 == 0)
            {
                infoText.text = $"Searching for server....";
            }
            if (Time.frameCount % 120 == 0)
            {
                infoText.text = $"Searching for server.....";
            }
            if (Time.frameCount % 150 == 0)
            {
                infoText.text = $"Searching for server......";
            }
            if (Time.frameCount % 180 == 0)
            {
                infoText.text = $"Searching for server.......";
            }
            
            if (_buffer.IsEmpty) return;
            if (hasFindServer) return;
            if (_buffer.TryDequeue(out var data))
            {
                finedServerInfo = MemoryPackSerializer.Deserialize<ServerInfo>(data);
                hasFindServer = true;
                gameObject.SetActive(false);
                GameMgr.Singleton.JoinGame(finedServerInfo.serverAddress, finedServerInfo.port, finedServerInfo.timeServerPort);
                _buffer.Clear();
            }
        }

        private void OnEnable()
        {
            localNetwork.Start();
        }

        private void OnDisable()
        {
            localNetwork.Stop();
        }
    }
}