using Destuff.Shared.Models;

namespace Destuff.Server.Models;

public class EventBuffer
{
    public int Id { get; set; }
    public EventType Type { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public string? Summary { get; set; }
}
