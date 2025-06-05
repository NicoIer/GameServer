using GameCore.Jolt;
using UnityEngine;

namespace Game.Jolt
{
    [RequireComponent(typeof(JoltClient))]
    public class JoltPush : MonoBehaviour
    {
        public delegate void OnPush<T>(in T data);

        
        private JoltClient _client;

        public event OnPush<WorldData> OnPushWorldData = delegate { };



        private void Start()
        {
            _client = GetComponent<JoltClient>();
            _client.messageHandler.Add<WorldData>(OnWorldData);
        }


        
        private void OnWorldData(in WorldData data)
        {
            OnPushWorldData(data);

        }
    }
}