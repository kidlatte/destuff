namespace Destuff.Shared.Models;

public class ListQuery
{
    public virtual string? Search { get; set; }
    public virtual int? Page { get; set; }
    public virtual int? PageSize { get; set; }
    public virtual string? SortField { get; set; }
    public virtual SortDirection SortDir { get; set; }

    public override string ToString()
    {
        var result = "";

        if (!string.IsNullOrEmpty(Search))
            result += $"s={Search}&";

        if (Page.HasValue)
            result += $"p={Page}&";

        if (PageSize.HasValue)
            result += $"ps={PageSize}&";

        if (!string.IsNullOrEmpty(SortField))
            result += $"sf={SortField}&sd={SortDir:d}";

        return result.TrimEnd('&');
    }
}

public enum SortDirection : byte
{ 
    Ascending = 0,
    Descending = 1,
}