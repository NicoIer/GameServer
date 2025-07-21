using Jolt;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltBody : MonoBehaviour
    {
        public BodyID bodyID { get; private set; }
        public JoltPhysicsWorld physicsWorld { get; private set; }
    }
}