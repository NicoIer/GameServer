using System;
using System.Collections.Generic;
using Cysharp.Threading;
using GameCore.Jolt;
using Jolt;
using UnityEngine;

namespace Game.Jolt
{
    public class JoltApplication : MonoBehaviour
    {
        public JoltPhysicsWorld physicsWorld { get; private set; }

        protected static void SetupCollisionFiltering(ref PhysicsSystemSettings settings)
        {
            // We use only 2 layers: one for non-moving objects and one for moving objects
            ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
            objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.NonMoving, (byte)ObjectLayers.Moving);
            objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.Moving, (byte)ObjectLayers.Moving);

            // We use a 1-to-1 mapping between object layers and broadphase layers
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.NonMoving,
                (byte)BroadPhaseLayers.NonMoving);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.Moving,
                (byte)BroadPhaseLayers.Moving);

            ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
                new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

            settings.ObjectLayerPairFilter = objectLayerPairFilter;
            settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
            settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;
        }

        protected const int TargetFPS = 60;
        private const int CollisionSteps = 1;
        private LogicLooper _physicsLooper;
        public event Action OnCreatedWorld = delegate { };
        public event Action OnDestroyWorld = delegate { };


        private List<IJoltSystem<JoltApplication, LogicLooperActionContext>> systems;

        private bool _initialized = false;

        private void Awake()
        {
            if (_initialized) return;
            if (!global::Jolt.Jolt.Initialized) throw new NotImplementedException("Jolt Physics Not Initialized");

            // 获取所有System
            systems = new List<IJoltSystem<JoltApplication, LogicLooperActionContext>>(
                GetComponents<IJoltSystem<JoltApplication, LogicLooperActionContext>>()
            );

            physicsWorld = new JoltPhysicsWorld(SetupCollisionFiltering);
            physicsWorld.physicsSystem.OptimizeBroadPhase();

            OnCreatedWorld();

            foreach (var system in systems)
            {
                system.BeforeRun();
            }


            _physicsLooper = new LogicLooper(TargetFPS);
            _physicsLooper.RegisterActionAsync(((in LogicLooperActionContext ctx) =>
            {
                foreach (var system in systems)
                {
                    system.BeforeUpdate(in ctx);
                }

                physicsWorld.Simulate(Time.fixedDeltaTime, CollisionSteps);

                foreach (var system in systems)
                {
                    system.AfterUpdate(in ctx);
                }
                
                return true;
            }));
            _initialized = true;
        }


        private void OnDestroy()
        {
            if (!_initialized) return;
            OnDestroyWorld();
            _physicsLooper.Dispose();
            physicsWorld.Dispose();
        }
    }
}