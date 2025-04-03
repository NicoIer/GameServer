using System;
using System.Collections.Generic;
using GameCore.Jolt;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.Jolt
{
    [RequireComponent(typeof(JoltPush))]
    public class JoltRenderer : MonoBehaviour
    {
        private JoltPush _push;


        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private Dictionary<Transform, BodyData> body2Data = new Dictionary<Transform, BodyData>();

        public readonly Dictionary<uint, Transform> bodyDict = new Dictionary<uint, Transform>();
        public MeshRenderer boxPrefab; // 1 * 1 * 1
        public MeshRenderer planePrefab; // 10 * 0 * 10
        public MeshRenderer spherePrefab; // 半径0.5

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
            // TODO Pooling
            snapshot.PushBack(data);
        }


        private void Update()
        {
            ref var data = ref snapshot.backValue;
            body2Data.Clear();
            HashSet<uint> allSet = HashSetPool<uint>.Get();

            foreach (var body in data.bodies)
            {
                allSet.Add(body.entityId);
                // if (body.isStatic) continue; // 静态物体
                if (bodyDict.TryGetValue(body.entityId, out var existingTransform))
                {
                    body2Data[existingTransform] = body;
                    existingTransform.position = body.position.T();
                    existingTransform.rotation = body.rotation.T();
                    continue;
                }

                var iShape = ShapeData.Revert(in body.shapeData);
                Transform shapeTransform = null;

                switch (iShape)
                {
                    case BoxShapeData boxShapeData:
                        var box = Instantiate(boxPrefab);
                        shapeTransform = box.transform;
                        shapeTransform.localScale = boxShapeData.halfExtents.T() * 2;
                        break;
                    case PlaneShapeData planeShapeData:
                        var plane = Instantiate(planePrefab);
                        shapeTransform = plane.transform;
                        // var normal = planeShapeData.normal.T();
                        // 根据法线计算旋转
                        // shapeTransform.rotation = Quaternion.LookRotation(normal, Vector3.up);
                        // 根据distance计算位置
                        // shapeTransform.position = normal * planeShapeData.distance;
                        // 根据halfExtent计算缩放
                        shapeTransform.localScale = new Vector3(planeShapeData.halfExtent * 2, 1,
                            planeShapeData.halfExtent * 2) / 10; // 10 是因为我们的默认模型大小是10*0*10的 要转换一下
                        break;
                    case SphereShapeData sphereShapeData:
                        var sphere = Instantiate(spherePrefab);
                        shapeTransform = sphere.transform;
                        shapeTransform.localScale = Vector3.one * (sphereShapeData.radius * 2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(iShape));
                }

                Assert.IsNotNull(shapeTransform);

                shapeTransform.gameObject.SetActive(true);
                shapeTransform.position = body.position.T();
                shapeTransform.rotation = body.rotation.T();
                bodyDict[body.entityId] = shapeTransform;
                body2Data[shapeTransform] = body;
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


        private void OnDestroy()
        {
            _push.OnPushWorldData -= OnWorldData;


            foreach (var (key, value) in bodyDict)
            {
                if (value.gameObject == null) continue; // 退出Editor Play Mode 的时候 可能缓存的GameObject 已经被销毁了
                Destroy(value.gameObject);
            }

            bodyDict.Clear();
        }
    }
}