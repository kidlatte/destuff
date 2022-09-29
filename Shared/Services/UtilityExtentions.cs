using Destuff.Shared.Models;

namespace Destuff.Shared.Services;

public static class UtilityExtentions
{
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? models) => models ?? Enumerable.Empty<T>();

    public static StuffCreateModel ToCreate(this StuffModel model) 
    { 
        return new StuffCreateModel
        {
            Name = model.Name,
            Notes = model.Notes,
            LocationId = model.IsSingleLocation ? model.FirstLocation?.Id : null
        };
    }
}