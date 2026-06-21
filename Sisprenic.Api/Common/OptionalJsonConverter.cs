using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sisprenic.Api.Common;

/// <summary>
/// System.Text.Json only accepts converters for closed types, not open generics like
/// Optional<T>, so it can't be told "use OptionalJsonConverter<T> for Optional<T>" directly.
/// This factory builds the matching OptionalJsonConverter<T> at runtime for whatever T shows up.
///
/// This is what lets Optional<T> implement JSON Merge Patch (RFC 7396) semantics on top
/// of System.Text.Json: separating a field that was never sent apart from one explicitly
/// set to null.
/// </summary>
public class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(OptionalJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return Optional<T>.Some(default);

        T? value = JsonSerializer.Deserialize<T>(ref reader, options);
        return Optional<T>.Some(value);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (!value.HasValue || value.Value is null)
            writer.WriteNullValue();
        else
            JsonSerializer.Serialize(writer, value.Value, options);
    }
}
