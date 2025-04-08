using GameCore.Jolt;
using UnityEngine;
using UnityToolkit;

namespace Game.Jolt
{
    public class JoltBody : MonoBehaviour
    {
        public BodyData data;
        public void OnWorldUpdate(BodyData body)
        {
            data = body;
            transform.position = body.position.T();
            transform.rotation = body.rotation.T();
        }
    }
}