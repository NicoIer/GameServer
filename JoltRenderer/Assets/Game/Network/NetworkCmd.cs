using System;
using GameCore.Physics;
using UnityEngine;
using UnityToolkit;
using Activation = GameCore.Physics.Activation;
using MotionType = GameCore.Physics.MotionType;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Network.Physics
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