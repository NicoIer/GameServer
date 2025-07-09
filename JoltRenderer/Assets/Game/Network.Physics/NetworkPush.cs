using GameCore.Physics;
using UnityEngine;

namespace Network.Physics
{
    [RequireComponent(typeof(NetworkCenter))]
    public class NetworkPush : MonoBehaviour
    {
        public delegate void OnPush<T>(in T data);

        
        private NetworkCenter _client;

        public event OnPush<WorldData> OnPushWorldData = delegate { };



        private void Start()
        {
            _client = GetComponent<NetworkCenter>();
            _client.messageHandler.Add<WorldData>(OnWorldData);
        }


        
        private void OnWorldData(in WorldData data)
        {
            OnPushWorldData(data);

        }
    }
}