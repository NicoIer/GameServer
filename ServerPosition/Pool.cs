namespace ServerPosition;

public interface IPool<T>
{
    void Return(in PooledObject<T> pooledObject);
}

public struct PooledObject<T> : IDisposable
{
    private IPool<T> _pool;
    public readonly T message;

    public PooledObject(in T message, IPool<T> pool)
    {
        this.message = message;
        _pool = pool;
    }

    public void Dispose()
    {
        _pool.Return(this);
    }
}