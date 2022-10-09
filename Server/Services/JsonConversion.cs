using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Destuff.Server.Services;

public static class JsonConversion
{
    public static PropertyBuilder<T?> HasJsonConversion<T>(this PropertyBuilder<T?> propertyBuilder) where T : class, new()
    {
        var converter = new ValueConverter<T?, string>
        (
            obj => JsonSerializer.Serialize(obj, default(JsonSerializerOptions)),
            json => JsonSerializer.Deserialize<T>(json, default(JsonSerializerOptions))
        );

        var comparer = new ValueComparer<T?>
        (
            (left, right) => JsonSerializer.Serialize(left, default(JsonSerializerOptions)) == JsonSerializer.Serialize(right, default(JsonSerializerOptions)),
            obj => obj == null ? 0 : JsonSerializer.Serialize(obj, default(JsonSerializerOptions)).GetHashCode(),
            obj => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj, default(JsonSerializerOptions)), default(JsonSerializerOptions))
        );

        propertyBuilder.HasConversion(converter, comparer);

        return propertyBuilder;
    }
}