using GameCore.Physics;
using UnityEngine;

namespace Network.Physics
{
    [RequireComponent(typeof(NetworkCenter))]
    public abstract class NetworkPush : MonoBehaviour
    {
        public delegate void OnPush<T>(in T data);
        private NetworkCenter _client;



        protected virtual void Start()
        {
            _client = GetComponent<NetworkCenter>();
        }
    }
}