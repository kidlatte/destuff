using Destuff.Shared.Models;

namespace Destuff.Shared.Services;

public static class UtilityExtentions
{
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? models) => models ?? Enumerable.Empty<T>();

    public static IEnumerable<(T Item, int Index)> ToIndex<T>(this IEnumerable<T>? models) => 
        models.OrEmpty().Select((x, i) => (x, i));
}