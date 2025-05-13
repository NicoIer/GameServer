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
        /// <summary>
        /// 模拟一帧
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Simulate(double deltaTime);

        /// <summary>
        /// 回滚指定的模拟次数
        /// </summary>
        /// <param name="delta">必须 > 0</param>
        public void Rollback(byte delta);


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
        public void Serialize(out WorldData worldData);

        /// <summary>
        /// 从数据中恢复物理世界
        /// </summary>
        /// <param name="worldData"></param>
        public void Deserialize(in WorldData worldData);
    }
}