using HashidsNet;

namespace Destuff.Server.Services;

public interface ILocationIdService
{
    int Decode(string hash);
    string Encode(int id);
}

public class LocationIdService : ILocationIdService
{
    private Hashids Hashids { get; }

    public LocationIdService()
    {
        Hashids = new Hashids("kidlatte/destuff-locations-ef5ded03ca29", 5);
    }

    public string Encode(int id)
    {
        return Hashids.Encode(id);
    }

    public int Decode(string hash)
    {
        return Hashids.Decode(hash).FirstOrDefault();
    }
}