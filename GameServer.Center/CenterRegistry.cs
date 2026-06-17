using GameServer.Core.Protocol;

namespace GameServer.Center;

public sealed class CenterRegistry
{
    private readonly Dictionary<string, ServiceEndpoint> _services = new();
    private readonly Dictionary<string, long> _tokens = new();
    private readonly object _lock = new();

    public string CreateToken(long uid)
    {
        string token = $"demo-token-{uid}-{Guid.NewGuid():N}";

        lock (_lock)
        {
            _tokens[token] = uid;
        }

        return token;
    }

    public long ValidateToken(string token)
    {
        lock (_lock)
        {
            if (_tokens.TryGetValue(token, out long uid))
            {
                return uid;
            }
        }

        return 0;
    }

    public void Register(ServiceEndpoint endpoint)
    {
        lock (_lock)
        {
            _services[MakeKey(endpoint.GameId, endpoint.Target, endpoint.RouteId)] = endpoint.Clone();
        }
    }

    public ServiceEndpoint? Resolve(string gameId, string target, string routeId)
    {
        lock (_lock)
        {
            if (_services.TryGetValue(MakeKey(gameId, target, routeId), out ServiceEndpoint? endpoint))
            {
                return endpoint.Clone();
            }

            if (routeId.Length == 0)
            {
                foreach (ServiceEndpoint service in _services.Values)
                {
                    if (service.GameId == gameId && service.Target == target)
                    {
                        return service.Clone();
                    }
                }
            }
        }

        return null;
    }

    private static string MakeKey(string gameId, string target, string routeId)
    {
        return $"{gameId}:{target}:{routeId}";
    }
}
