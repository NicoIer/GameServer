namespace GameCore.Jolt
{
    public interface IRenderer
    {
        IConnector PhysicsData { get; }
    }

    public interface IConnector
    {
    }


    public interface IPhysicsWorld
    {
        protected static long worldIdCounter;
        public byte worldId { get; }

        /// <summary>
        /// 模拟一帧
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="collisionSteps"></param>
        public PhysicsUpdateError Simulate(float deltaTime, int collisionSteps);
        
        /// <summary>
        /// 获取一个实体的数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public bool QueryBody(in uint id, out BodyData bodyData);

        /// <summary>
        /// 更新一个实体的数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public bool UpdateBody(in uint id, in BodyData bodyData);


        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="worldData"></param>
        /// <returns></returns>
        public bool QueryHistoryData(byte delta, out WorldData worldData);

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