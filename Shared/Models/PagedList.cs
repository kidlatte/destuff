namespace Destuff.Shared.Models;

public class PagedList<T>
{
    public int Count { get; set; }
    public ICollection<T> List { get; set; }

    public PagedList(int count, ICollection<T> list)
    {
        Count = count;
        List = list;
    }
}