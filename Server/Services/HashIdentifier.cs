using HashidsNet;
using Destuff.Server.Data.Entities;

namespace Destuff.Server.Services;

public interface IHashIdentifier<T> where T : Entity
{
    int Decode(string hash);
    string Encode(int id);
}


public class HashIdentifier<T> : IHashIdentifier<T> where T : Entity
{
    private Hashids Hashids { get; }

    public HashIdentifier() => Hashids = new Hashids($"destuff-{typeof(T).Name.ToLowerInvariant()}-ef5ded03ca29", 5);

    public string Encode(int id)
    {
        return Hashids.Encode(id);
    }

    public int Decode(string hash)
    {
        return Hashids.TryDecodeSingle(hash, out var id) ? id : -1;
    }
}

public interface ILocationIdentifier : IHashIdentifier<Location> { }
public class LocationIdentifier : HashIdentifier<Location>, ILocationIdentifier { }

public interface IStuffIdentifier : IHashIdentifier<Stuff> { }
public class StuffIdentifier : HashIdentifier<Stuff>, IStuffIdentifier { }

public interface IUploadIdentifier : IHashIdentifier<Upload> { }
public class UploadIdentifier : HashIdentifier<Upload>, IUploadIdentifier { }

public static class HashIdentifierExtensions
{
    public static IServiceCollection AddHashIdentifiers(this IServiceCollection services)
    {
        services.AddSingleton<ILocationIdentifier, LocationIdentifier>();
        services.AddSingleton<IStuffIdentifier, StuffIdentifier>();
        services.AddSingleton<IUploadIdentifier, UploadIdentifier>();
        return services;
    }
}
