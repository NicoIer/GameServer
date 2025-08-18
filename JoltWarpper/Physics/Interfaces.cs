// TODO 使用自动生成工具生成这些枚举

using System;
using System.Numerics;

namespace GameCore.Physics
{
    public interface IJoltSystem<in TApp, TCtx, in TPhysicsWorld> where TPhysicsWorld : IPhysicsWorld
    {
        public void OnAdded(TApp app, TPhysicsWorld world);


        void OnRemoved();
        public void BeforePhysicsStart();
        public void BeforePhysicsUpdate(in TCtx ctx);
        public void AfterPhysicsUpdate(in TCtx ctx);
        public void AfterPhysicsStop();

        public bool NeedShutdown()
        {
            return false;
        }

        public void Dispose();
    }


    public interface IPhysicsWorld
    {
        public const int ServerId = 0;
        public const int MaxBodies = 65536;
        public const int MaxBodyPairs = 65536;
        public const int MaxContactConstraints = 65536;
        public const int NumBodyMutexes = 0;

        protected static long worldIdCounter;
        public byte worldId { get; }

        /// <summary>
        /// 模拟一帧
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="collisionSteps"></param>
        public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps);

        /// <summary>
        /// 创建一个物理实体
        /// </summary>
        /// <param name="shapeData"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="motionType"></param>
        /// <param name="layers"></param>
        /// <param name="activation"></param>
        /// <returns>body Id</returns>
        public uint CreateAndAdd(IShapeData shapeData, in Vector3 position, in Quaternion rotation, MotionType motionType,
            ObjectLayers layers, Activation activation);

        
        public bool IsAdded(in uint id);
        
        /// <summary>
        /// 获取一个实体的数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public bool QueryBody(in uint id, out BodyData bodyData);
        
        public Vector3 GetPosition(in uint id);
        
        public Quaternion GetRotation(in uint id);

        // /// <summary>
        // /// 更新一个实体的数据
        // /// </summary>
        // /// <param name="id"></param>
        // /// <param name="bodyData"></param>
        // /// <returns></returns>
        // public bool UpdateBody(in uint id, in BodyData bodyData);


        /// <summary>
        /// 序列化物理世界
        /// </summary>
        /// <param name="worldData"></param>
        public void Serialize(ref WorldData worldData);

        /// <summary>
        /// 从数据中恢复物理世界
        /// </summary>
        /// <param name="worldData"></param>
        public void Deserialize(in WorldData worldData);


        public void Activate(in uint id);

        public void Deactivate(in uint id);

        public void RemoveAndDestroy(in uint id);

        public void Dispose();
    }
}