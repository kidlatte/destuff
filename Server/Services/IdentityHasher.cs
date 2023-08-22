using HashidsNet;
using Destuff.Server.Data.Entities;

namespace Destuff.Server.Services;

public interface IIdentityHasher<T> where T : Entity
{
    string? Encode(int? id);
    int? Decode(string? hash);
}

public class IdentityHasher<T> : IIdentityHasher<T> where T : Entity
{
    private Hashids Hashids { get; }

    public IdentityHasher()
    {
        Hashids = new Hashids($"destuff-{typeof(T).Name.ToLowerInvariant()}-ef5ded03ca29", 5);
    }

    public string? Encode(int? id)
    {
        return id.HasValue ? Hashids.Encode(id.Value) : default;
    }

    public int? Decode(string? hash)
    {
        return Hashids.TryDecodeSingle(hash, out var id) ? id : null;
    }
}

public static class HashIdentifierExtensions
{
    public static IServiceCollection AddHashIdentifiers(this IServiceCollection services)
    {
        services.AddHashIdentifier<Location>();
        services.AddHashIdentifier<Stuff>();
        services.AddHashIdentifier<Upload>();
        services.AddHashIdentifier<Supplier>();
        services.AddHashIdentifier<Purchase>();
        services.AddHashIdentifier<PurchaseItem>();
        services.AddHashIdentifier<Event>();
        return services;
    }

    private static IServiceCollection AddHashIdentifier<T>(this IServiceCollection services) where T : Entity
    {
        services.AddSingleton<IIdentityHasher<T>, IdentityHasher<T>>();
        return services;
    }
}
