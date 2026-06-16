// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    [Serializable]
    public class PhysicsLayerDefinition
    {
        public string name = "Unnamed";
        /// <summary>
        /// BroadPhase layer index. Typically 0=Static, 1=Dynamic.
        /// Layers sharing the same broadPhase group are tested together.
        /// </summary>
        public BroadPhaseLayer broadPhaseLayer;
    }

    /// <summary>
    /// Pure C# physics settings data — no Unity dependency.
    /// Usable on server or any .NET host. In Unity, wrapped by JoltPhysicsSettings (ScriptableObject).
    /// </summary>
    [Serializable]
    public class PhysicsSettingsData
    {
        public Float3 gravity = new(0, -9.81f, 0);
        public int collisionSteps = 1;
        public uint maxBodies { get; set; }= 10240;
        public uint maxContactConstraints = 10240;

        /// <summary>
        /// GroupFilterTable 的 SubGroup 数量。用于 CollisionGroup 内的细粒度碰撞过滤。
        /// 设为 0 则不创建 GroupFilterTable。
        /// </summary>
        public uint numSubGroups = 0;

        public PhysicsLayerDefinition[] layers = new[]
        {
            new PhysicsLayerDefinition { name = "Static", broadPhaseLayer = 0 },
            new PhysicsLayerDefinition { name = "Dynamic", broadPhaseLayer = 1 },
        };

        /// <summary>
        /// Per-layer collision bitmask. collisionMask[i] bit j == 1 means layer i collides with layer j.
        /// Symmetric: if layer A collides with B, then B also collides with A.
        /// </summary>
        public uint[] collisionMask = new uint[]
        {
            0b10, // Static: collides with Dynamic
            0b11, // Dynamic: collides with Static + Dynamic
        };

        public int LayerCount => layers?.Length ?? 0;

        /// <summary>
        /// Returns the layer index by name, or -1 if not found.
        /// </summary>
        public int GetLayerIndex(string layerName)
        {
            if (layers == null) return -1;
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].name == layerName)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the layer names for display in dropdowns.
        /// </summary>
        public string[] GetLayerNames()
        {
            if (layers == null) return Array.Empty<string>();
            var names = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                names[i] = $"{layers[i].name} ({i})";
            return names;
        }

        /// <summary>
        /// Whether two object layers should collide with each other.
        /// </summary>
        public bool GetCollisionEnabled(int layerA, int layerB)
        {
            if (collisionMask == null) return false;
            int n = layers?.Length ?? 0;
            if (layerA < 0 || layerA >= n || layerB < 0 || layerB >= n) return false;
            if (layerA >= collisionMask.Length) return false;
            return (collisionMask[layerA] & (1u << layerB)) != 0;
        }

        /// <summary>
        /// Set whether two object layers should collide. Keeps the matrix symmetric.
        /// </summary>
        public void SetCollisionEnabled(int layerA, int layerB, bool enabled)
        {
            if (layers == null) return;
            int n = layers.Length;
            if (layerA < 0 || layerA >= n || layerB < 0 || layerB >= n) return;

            EnsureCollisionMaskSize();

            if (enabled)
            {
                collisionMask[layerA] |= 1u << layerB;
                collisionMask[layerB] |= 1u << layerA;
            }
            else
            {
                collisionMask[layerA] &= ~(1u << layerB);
                collisionMask[layerB] &= ~(1u << layerA);
            }
        }

        /// <summary>
        /// Ensures the collision mask array matches the current layer count.
        /// </summary>
        public void EnsureCollisionMaskSize()
        {
            int n = layers?.Length ?? 0;
            if (collisionMask == null || collisionMask.Length != n)
            {
                var newMask = new uint[n];
                if (collisionMask != null)
                {
                    int copyLen = Math.Min(collisionMask.Length, n);
                    Array.Copy(collisionMask, newMask, copyLen);
                }
                collisionMask = newMask;
            }
        }

        /// <summary>
        /// Returns the number of unique broad phase layers used.
        /// </summary>
        public int GetBroadPhaseLayerCount()
        {
            if (layers == null) return 0;
            int max = 0;
            foreach (var layer in layers)
            {
                if (layer.broadPhaseLayer > max)
                    max = layer.broadPhaseLayer;
            }
            return max + 1;
        }

        /// <summary>
        /// Converts layers to <see cref="LayerData"/> array (for WorldData export).
        /// </summary>
        public LayerData[] ToLayerDataArray()
        {
            if (layers == null) return Array.Empty<LayerData>();
            EnsureCollisionMaskSize();

            var result = new LayerData[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                result[i] = new LayerData
                {
                    name = layers[i].name,
                    broadPhaseLayer = layers[i].broadPhaseLayer,
                    collisionMask = i < collisionMask.Length ? collisionMask[i] : 0u,
                };
            }
            return result;
        }

        /// <summary>
        /// Populates layers and collision masks from <see cref="LayerData"/> array (for WorldData import).
        /// </summary>
        public void FromLayerDataArray(LayerData[] layerDataArray)
        {
            if (layerDataArray == null || layerDataArray.Length == 0)
            {
                layers = Array.Empty<PhysicsLayerDefinition>();
                collisionMask = Array.Empty<uint>();
                return;
            }

            layers = new PhysicsLayerDefinition[layerDataArray.Length];
            collisionMask = new uint[layerDataArray.Length];
            for (int i = 0; i < layerDataArray.Length; i++)
            {
                layers[i] = new PhysicsLayerDefinition
                {
                    name = layerDataArray[i].name,
                    broadPhaseLayer = layerDataArray[i].broadPhaseLayer,
                };
                collisionMask[i] = layerDataArray[i].collisionMask;
            }
        }

        /// <summary>
        /// Creates a <see cref="PhysicsSettingsData"/> from <see cref="WorldData"/> (ignoring bodies).
        /// </summary>
        public static PhysicsSettingsData FromWorldData(WorldData data)
        {
            var settings = new PhysicsSettingsData
            {
                gravity = data.gravity,
                collisionSteps = data.collisionSteps,
                maxBodies = data.maxBodies,
                maxContactConstraints = data.maxContactConstraints,
            };
            settings.FromLayerDataArray(data.layers);
            return settings;
        }

        /// <summary>
        /// Fills a <see cref="WorldData"/> with settings (does not touch bodies).
        /// </summary>
        public void ApplyToWorldData(WorldData data)
        {
            data.gravity = gravity;
            data.collisionSteps = collisionSteps;
            data.maxBodies = maxBodies;
            data.maxContactConstraints = maxContactConstraints;
            data.layers = ToLayerDataArray();
        }
    }
}
