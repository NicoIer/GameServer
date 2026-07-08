using GameServer.Core.Protocol;
using ProtocolGameId = GameServer.Core.Protocol.GameId;

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

    public ServiceEndpoint? Resolve(ProtocolGameId gameId, string target, string routeId)
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

    public List<ServiceEndpoint> ListServices(string target)
    {
        var result = new List<ServiceEndpoint>();
        lock (_lock)
        {
            foreach (ServiceEndpoint service in _services.Values)
            {
                if (target.Length != 0 && service.Target != target)
                {
                    continue;
                }

                result.Add(service.Clone());
            }
        }

        result.Sort(CompareEndpoint);
        return result;
    }

    private static int CompareEndpoint(ServiceEndpoint a, ServiceEndpoint b)
    {
        int result = a.GameId.CompareTo(b.GameId);
        if (result != 0)
        {
            return result;
        }

        result = string.CompareOrdinal(a.Target, b.Target);
        if (result != 0)
        {
            return result;
        }

        return string.CompareOrdinal(a.RouteId, b.RouteId);
    }

    private static string MakeKey(ProtocolGameId gameId, string target, string routeId)
    {
        return $"{(int)gameId}:{target}:{routeId}";
    }
}
