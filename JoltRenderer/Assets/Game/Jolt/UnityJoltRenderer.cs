using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.Jolt;
using Network;
using Network.Client;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.Jolt
{
    public class UnityJoltRenderer : MonoBehaviour, IJoltRenderer
    {
        private NetworkClient _client;
        public string serverHost = "localhost";
        public ushort serverPort = 24419;
        public int maxRetries = int.MaxValue;
        public float retryDelay = 1.0f;


        public MeshRenderer boxPrefab; // 1 * 1 * 1
        public MeshRenderer planePrefab; // 10 * 0 * 10
        public MeshRenderer spherePrefab; // 半径0.5

        public Dictionary<uint, Transform> bodyDict = new Dictionary<uint, Transform>();

        private float lastTryConnectTime;

        // public int countDisconnectCount;
        public bool continueConnect = true;

        private async void Awake()
        {
            Application.runInBackground = true;
            if (!ShapeData.registered)
            {
                ShapeData.RegisterAll();
            }

            var socket = new TelepathyClientSocket();
            socket.OnDisconnected += OnDisConnected;
            socket.OnConnected += OnConnected;
            _client = new NetworkClient(socket);

            _client.messageHandler.Add<WorldData>(OnWorldData);
            UriBuilder uriBuilder = new UriBuilder
            {
                Host = serverHost,
                Port = serverPort
            };
            await _client.Run(uriBuilder.Uri, false);
            lastTryConnectTime = Time.time;
            _client.socket.TickOutgoing(); // 主动向服务器发送数据
            ContinueConnect(uriBuilder.Uri).Forget();

            NetworkLoop.OnEarlyUpdate += OnEarlyUpdate;
            NetworkLoop.OnLateUpdate += OnLateUpdate;
        }

        private async UniTask ContinueConnect(Uri uri)
        {
            while (Application.isPlaying && continueConnect)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
                if (_client.socket.connected || _client.socket.connecting)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (Time.time - lastTryConnectTime > retryDelay)
                {
                    _client.Stop();
                    lastTryConnectTime = Time.time;
                    await _client.Run(uri, false);
                }
            }
        }


        private void OnDisConnected()
        {
            Debug.Log("Disconnected from server!");
            // ++countDisconnectCount;
            // if (countDisconnectCount == 1)
            // {
            //     UriBuilder uriBuilder = new UriBuilder
            //     {
            //         Host = serverHost,
            //         Port = serverPort
            //     };
            //     ContinueConnect(uriBuilder.Uri).Forget();
            // }
        }

        private void OnConnected()
        {
            Debug.Log("Connected to server!");
        }

        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private Dictionary<Transform, BodyData> body2Data = new Dictionary<Transform, BodyData>();

        private void OnWorldData(in WorldData data)
        {
            // TODO Pooling
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
                        shapeTransform.localScale = Vector3.one * sphereShapeData.radius * 2;
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


        [Sirenix.OdinInspector.Button]
        private void CmdSpawnBox(System.Numerics.Vector3 halfExtents, System.Numerics.Vector3 position,
            MotionType motionType = MotionType.Dynamic,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.Moving
        )
        {
            CmdSpawnBox cmd = new CmdSpawnBox
            {
                halfExtents = halfExtents,
                position = position,
                rotation = System.Numerics.Quaternion.Identity,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }

        [Sirenix.OdinInspector.Button]
        private void CmdSpawnPlane(
            Vector3 position,
            float distance = 0,
            float halfExtent = 10,
            MotionType motionType = MotionType.Static,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.NonMoving
        )
        {
            CmdSpawnPlane cmd = new CmdSpawnPlane
            {
                position = position.T(),
                rotation = System.Numerics.Quaternion.Identity,
                normal = new System.Numerics.Vector3(0, 1, 0),
                distance = distance,
                halfExtent = halfExtent,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }

        private void OnDestroy()
        {
            NetworkLoop.OnEarlyUpdate -= OnEarlyUpdate;
            NetworkLoop.OnLateUpdate -= OnLateUpdate;
            continueConnect = false;
            _client.messageHandler.Clear<WorldData>();
            _client.Stop();
            _client.Dispose();

            foreach (var (key, value) in bodyDict)
            {
                GameObject.Destroy(value.gameObject);
            }

            bodyDict.Clear();
        }

        private void OnEarlyUpdate()
        {
            _client.socket.TickIncoming();
        }

        private void OnLateUpdate()
        {
            _client.socket.TickOutgoing();
        }
    }
}