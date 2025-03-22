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


        public MeshRenderer boxPrefab;

        public Dictionary<uint, Transform> bodyDict = new Dictionary<uint, Transform>();

        private async void Awake()
        {
            ShapeData.RegisterAll();
            var socket = new TelepathyClientSocket();
            _client = new NetworkClient(socket);
            UriBuilder uriBuilder = new UriBuilder
            {
                Host = serverHost,
                Port = serverPort
            };
            await _client.Run(uriBuilder.Uri, false);
            await UniTask.Delay(TimeSpan.FromSeconds(retryDelay * 2));
            int retries = 0;
            while (!_client.socket.connected)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(retryDelay));
                ++retries;
                Debug.Log($"Retrying connection...{retries}");
                if (retries >= maxRetries)
                {
                    Debug.LogError("Failed to connect to server!");
                    return;
                }
            }

            Debug.Log("Connected to server!");

            _client.messageHandler.Add<WorldData>(OnWorldData);


            NetworkLoop.OnEarlyUpdate += OnEarlyUpdate;
            NetworkLoop.OnLateUpdate += OnLateUpdate;
        }

        private void OnWorldData(in WorldData data)
        {
            // TODO Pooling

            HashSet<uint> allSet = HashSetPool<uint>.Get();

            foreach (var body in data.bodies)
            {
                allSet.Add(body.entityId);
                if (bodyDict.TryGetValue(body.entityId, out var existingTransform))
                {
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
                    case SphereShapeData sphereShapeData:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(iShape));
                }

                Assert.IsNotNull(shapeTransform);

                shapeTransform.gameObject.SetActive(true);
                shapeTransform.position = body.position.T();
                shapeTransform.rotation = body.rotation.T();
                bodyDict[body.entityId] = shapeTransform;
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
        private void CmdSpawnBox(Vector3 halfExtents, Vector3 position, Quaternion rotation, MotionType motionType,
            Activation activation, ObjectLayers objectLayer)
        {
            CmdSpawnBox cmd = new CmdSpawnBox
            {
                halfExtents = halfExtents.T(),
                position = position.T(),
                rotation = rotation.T(),
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }

        private void OnDestroy()
        {
            _client.messageHandler.Clear<WorldData>();
            _client.Stop();
            _client.Dispose();
            NetworkLoop.OnEarlyUpdate -= OnEarlyUpdate;
            NetworkLoop.OnLateUpdate -= OnLateUpdate;
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