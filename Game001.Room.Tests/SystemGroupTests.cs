using GameServer.Core.Systems;
using Xunit;

namespace Game001.Room.Tests;

public sealed class SystemGroupTests
{
    [Fact]
    public void ExecuteAfterOrdersSystemAfterTarget()
    {
        var log = new List<string>();
        var group = new SystemGroup(new AfterRootSystem(log), new RootSystem(log));
        const long deltaTimeMs = 11;
        const int frame = 3;
        const long timeNowMs = 99;

        group.Update(in deltaTimeMs, in frame, in timeNowMs);

        Assert.Equal(new[]
        {
            "root.update:11:3:99",
            "after.update:11:3:99",
        }, log);
    }

    [Fact]
    public void ExecuteBeforeOrdersSystemBeforeTarget()
    {
        var log = new List<string>();
        var group = new SystemGroup(new RootSystem(log), new BeforeRootSystem(log));
        const long deltaTimeMs = 12;
        const int frame = 4;
        const long timeNowMs = 100;

        group.Update(in deltaTimeMs, in frame, in timeNowMs);

        Assert.Equal(new[]
        {
            "before.update:12:4:100",
            "root.update:12:4:100",
        }, log);
    }

    [Fact]
    public void SystemsWithoutDependenciesKeepRegistrationOrder()
    {
        var log = new List<string>();
        var group = new SystemGroup(new FirstSystem(log), new SecondSystem(log));
        const long deltaTimeMs = 1;
        const int frame = 2;
        const long timeNowMs = 3;

        group.Update(in deltaTimeMs, in frame, in timeNowMs);

        Assert.Equal(new[]
        {
            "first.update:1:2:3",
            "second.update:1:2:3",
        }, log);
    }

    [Fact]
    public void OnCreateAndOnDestroyUseExecutionOrderAndDestroyOnce()
    {
        var log = new List<string>();
        var group = new SystemGroup(new RootSystem(log), new BeforeRootSystem(log));
        const long deltaTimeMs = 5;
        const int frame = 7;
        const long timeNowMs = 9;

        group.OnCreate();
        group.Update(in deltaTimeMs, in frame, in timeNowMs);
        group.OnDestroy();
        group.OnDestroy();

        Assert.Equal(new[]
        {
            "before.create",
            "root.create",
            "before.update:5:7:9",
            "root.update:5:7:9",
            "root.destroy",
            "before.destroy",
        }, log);
    }

    [Fact]
    public void CyclicDependenciesThrow()
    {
        var log = new List<string>();

        Assert.Throws<InvalidOperationException>(() => new SystemGroup(new CycleFirstSystem(log), new CycleSecondSystem(log)));
    }

    [Fact]
    public void TryGetSystemReturnsRegisteredConcreteSystem()
    {
        var log = new List<string>();
        var firstSystem = new FirstSystem(log);
        var group = new SystemGroup(firstSystem, new SecondSystem(log));

        bool found = group.TryGetSystem(out FirstSystem? system);

        Assert.True(found);
        Assert.Same(firstSystem, system);
    }

    [Fact]
    public void TryGetSystemReturnsFalseForMissingSystem()
    {
        var log = new List<string>();
        var group = new SystemGroup(new FirstSystem(log));

        bool found = group.TryGetSystem(out SecondSystem? system);

        Assert.False(found);
        Assert.Null(system);
    }

    private abstract class TestSystem : ISystem
    {
        private readonly string _name;
        protected readonly List<string> Log;

        protected TestSystem(List<string> log, string name)
        {
            Log = log;
            _name = name;
        }

        public void OnCreate()
        {
            Log.Add($"{_name}.create");
        }

        public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
        {
            Log.Add($"{_name}.update:{deltaTimeMs}:{frame}:{timeNowMs}");
        }

        public void OnDestroy()
        {
            Log.Add($"{_name}.destroy");
        }
    }

    private sealed class RootSystem : TestSystem
    {
        public RootSystem(List<string> log)
            : base(log, "root")
        {
        }
    }

    [ExecuteAfter(typeof(RootSystem))]
    private sealed class AfterRootSystem : TestSystem
    {
        public AfterRootSystem(List<string> log)
            : base(log, "after")
        {
        }
    }

    [ExecuteBefore(typeof(RootSystem))]
    private sealed class BeforeRootSystem : TestSystem
    {
        public BeforeRootSystem(List<string> log)
            : base(log, "before")
        {
        }
    }

    private sealed class FirstSystem : TestSystem
    {
        public FirstSystem(List<string> log)
            : base(log, "first")
        {
        }
    }

    private sealed class SecondSystem : TestSystem
    {
        public SecondSystem(List<string> log)
            : base(log, "second")
        {
        }
    }

    [ExecuteAfter(typeof(CycleSecondSystem))]
    private sealed class CycleFirstSystem : TestSystem
    {
        public CycleFirstSystem(List<string> log)
            : base(log, "cycle-first")
        {
        }
    }

    [ExecuteAfter(typeof(CycleFirstSystem))]
    private sealed class CycleSecondSystem : TestSystem
    {
        public CycleSecondSystem(List<string> log)
            : base(log, "cycle-second")
        {
        }
    }
}
