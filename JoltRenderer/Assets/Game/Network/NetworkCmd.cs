using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkCenter))]
    public abstract class NetworkCmd : MonoBehaviour
    {
        private NetworkCenter _client;

        protected virtual void Start()
        {
            _client = GetComponent<NetworkCenter>();
        }
    }
}