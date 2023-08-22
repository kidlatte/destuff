namespace Destuff.Shared.Models;

public class EventModel : IModel
{
    public required string Id { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public string? Summary { get; set; }

    public required StuffModel Stuff { get; set; }
}

public class EventListItem : IModel
{
    public required string Id { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public required string Type { get; set; }
    public string? Summary { get; set; }
}
