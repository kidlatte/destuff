namespace Destuff.Shared.Services;

public static class UtilityExtentions
{
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? models) => models ?? Enumerable.Empty<T>();
}