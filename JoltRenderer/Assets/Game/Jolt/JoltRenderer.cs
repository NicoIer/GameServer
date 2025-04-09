using System;
using System.Collections.Generic;
using GameCore.Jolt;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.Jolt
{
    [RequireComponent(typeof(JoltPush))]
    public sealed class JoltRenderer : MonoBehaviour
    {
        private JoltPush _push;


        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        // private Dictionary<JoltBody, BodyData> body2Data = new Dictionary<JoltBody, BodyData>();
        public readonly Dictionary<uint, JoltBody> bodyDict = new Dictionary<uint, JoltBody>();

        public JoltBody boxPrefab; // 1 * 1 * 1
        public JoltBody planePrefab; // 10 * 0 * 10
        public JoltBody spherePrefab; // 半径0.5

        [NonSerialized] public CircularBuffer<WorldData> snapshot;
        public int snapshotCapacity = 16;

        public WorldData currentWorld;

        private void Awake()
        {
            _push = GetComponent<JoltPush>();
            _push.OnPushWorldData += OnWorldData;
            snapshot = new CircularBuffer<WorldData>(snapshotCapacity);
        }

        private void OnWorldData(in WorldData data)
        {
            snapshot.PushBack(data);
        }


        private void Update()
        {
            ref var data = ref snapshot.backValue;
            UpdateWorld(ref data);
        }

        private void UpdateWorld(ref WorldData data)
        {
            currentWorld = data;
            // body2Data.Clear();
            HashSet<uint> allSet = HashSetPool<uint>.Get();

            foreach (var bodyData in data.bodies)
            {
                allSet.Add(bodyData.entityId);
                if (bodyDict.TryGetValue(bodyData.entityId, out var existingBody))
                {
                    Assert.IsTrue(bodyData.bodyType == existingBody.bodyType);
                    if (bodyData.motionType != MotionType.Static)
                    {
                        existingBody.OnBodyDataUpdate(bodyData);
                        existingBody.shape.OnShapeUpdate(in bodyData.shapeDataPacket);
                    }
                    continue;
                }


                JoltBody unityBody = CreateBodyFromData(in bodyData);
                // more assert like ......
                Assert.IsNotNull(unityBody);
                Assert.IsNotNull(unityBody.shape);
                unityBody.OnBodyDataUpdate(bodyData); 
                unityBody.shape.OnShapeUpdate(in bodyData.shapeDataPacket);
                
                Assert.IsTrue(unityBody.bodyType == bodyData.bodyType);
                Assert.IsTrue(unityBody.motionType == bodyData.motionType);

                bodyDict[bodyData.entityId] = unityBody;
            }

            HashSet<uint> toRemove = HashSetPool<uint>.Get();
            foreach (var key in bodyDict.Keys)
            {
                if (!allSet.Contains(key))
                {
                    toRemove.Add(key);
                }
            }

            foreach (var key in toRemove)
            {
                Destroy(bodyDict[key].gameObject);
                bodyDict.Remove(key);
            }

            HashSetPool<uint>.Release(allSet);
            HashSetPool<uint>.Release(toRemove);
        }

        private JoltBody CreateBodyFromData(in BodyData bodyData)
        {
            var iShape = ShapeDataPacket.Deserialize(in bodyData.shapeDataPacket);
            JoltBody unityBody;
            switch (iShape)
            {
                case BoxShapeData boxShapeData:
                    unityBody = Instantiate(boxPrefab);
                    unityBody.CreateShape<BoxShapeData, JoltBoxShape>(in boxShapeData);
                    unityBody.transform.localScale = boxShapeData.halfExtents.T() * 2;
                    break;
                case PlaneShapeData planeShapeData:
                    Assert.IsTrue(bodyData.motionType == MotionType.Static);
                    unityBody = Instantiate(planePrefab);
                    unityBody.CreateShape<PlaneShapeData, JoltPlaneShape>(in planeShapeData);
                    unityBody.transform.localScale = new Vector3(planeShapeData.halfExtent * 2, 1,
                        planeShapeData.halfExtent * 2) / 10; // 10 是因为我们的默认模型大小是10*0*10的 要转换一下
                    break;
                case SphereShapeData sphereShapeData:
                    unityBody = Instantiate(spherePrefab);
                    unityBody.CreateShape<SphereShapeData,JoltSphereShape>(in sphereShapeData);
                    unityBody.transform.localScale = Vector3.one * (sphereShapeData.radius * 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(iShape));
            }

            unityBody.gameObject.SetActive(true); // must be active
            return unityBody;
        }


        private void OnDestroy()
        {
            _push.OnPushWorldData -= OnWorldData;
            
            foreach (var (key, value) in bodyDict)
            {
                if (value == null) continue;
                if (value.gameObject == null) continue; // 退出Editor Play Mode 的时候 可能缓存的GameObject 已经被销毁了
                Destroy(value.gameObject);
            }

            bodyDict.Clear();
        }
    }
}