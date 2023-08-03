using Microsoft.AspNetCore.Mvc;
using Destuff.Shared.Models;

namespace Destuff.Server.Models;

public class ListRequest : ListQuery
{
    [FromQuery(Name = "s")]
    public override string? Search { get; set; }

    [FromQuery(Name = "p")]
    public override int? Page { get; set; }

    [FromQuery(Name = "ps")]
    public override int? PageSize { get; set; }

    [FromQuery(Name = "sf")]
    public override string? SortField { get; set; }

    [FromQuery(Name = "sd")]
    public override SortDirection SortDir { get; set; }

    public int Take => PageSize ?? 20;
    public int Skip => Take * Page ?? 0;
}