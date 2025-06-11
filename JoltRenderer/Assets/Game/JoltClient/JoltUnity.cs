using Cysharp.Threading;
using Game.Jolt;
using GameCore.Jolt;
using UnityEngine;

namespace Game.JoltClient
{
    [RequireComponent(typeof(JoltApplication))]
    public class JoltUnity : MonoBehaviour, IJoltSystem<JoltApplication, LogicLooperActionContext>
    {
        private JoltApplication _app;
        private IPhysicsWorld _world;
        public bool autoSyncTransform = true;

        public void OnAdded(JoltApplication app, IPhysicsWorld world)
        {
            _app = app;
            _app.physicsWorld.OnBodyCreate += OnBodyCreated;

            _world = world;
        }


        public void OnRemoved()
        {
            _app.physicsWorld.OnBodyCreate -= OnBodyCreated;
        }


        private void OnBodyCreated(in uint bodyId)
        {
            // TODO 创建一个Unity对象和Jolt对应
        }

        public void BeforeRun()
        {
            // throw new System.NotImplementedException();
        }

        public void BeforeUpdate(in LogicLooperActionContext ctx)
        {
            // throw new System.NotImplementedException();
        }

        public void AfterUpdate(in LogicLooperActionContext ctx)
        {
            // throw new System.NotImplementedException();
        }

        public void AfterRun()
        {
            // throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            // throw new System.NotImplementedException();
        }
    }
}