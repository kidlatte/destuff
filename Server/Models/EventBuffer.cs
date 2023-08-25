using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Models;

internal class EventBuffer : IEvent
{
    public int Id { get; set; }
    public EventType Type { get; set; }
    public int StuffId { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public string? Summary { get; set; }
    public EventData? Data { get; set; }

}
