namespace GameServer.Core.Rooms;

public readonly record struct RoomWorkerMetrics(
    int RoomCount,
    int ClosingRoomCount,
    int OnlineConnectionCount,
    long RequestCount,
    long RequestErrorCount,
    TimeSpan LastRequestElapsed,
    TimeSpan MaxRequestElapsed,
    long RoomCreatedCount,
    long RoomClosedCount,
    long DisconnectionCount,
    long RoomConnectCount,
    long PushSentCount,
    long PushDroppedCount);

public readonly record struct RoomMetrics(
    string RoomId,
    RoomLifecycleState LifecycleState,
    int PlayerCount,
    int ConnectionCount,
    TimeSpan LastFrameElapsed,
    TimeSpan MaxFrameElapsed);
