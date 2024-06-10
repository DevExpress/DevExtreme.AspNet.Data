#if NET6_0
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

#if NET6_0 || NET7_0
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#endif

#if NET6_0
// TODO: Remove after migrate to net7+
// https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-7/
// https://stackoverflow.com/questions/74246482/
public sealed class Net6DateOnlyJsonConverter : JsonConverter<DateOnly> {
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateOnly.FromDateTime(reader.GetDateTime());
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("O"));
}
public sealed class Net6TimeOnlyJsonConverter : JsonConverter<TimeOnly> {
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => TimeOnly.Parse(reader.GetString());
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("HH:mm:ss.fff"));
}
#endif

#if NET6_0 || NET7_0
public sealed class NetLess8DateOnlyValueConverter : ValueConverter<DateOnly, DateTime> {
    public NetLess8DateOnlyValueConverter() : base(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime)) { }
}

public sealed class NetLess8TimeOnlyValueConverter : ValueConverter<TimeOnly, TimeSpan> {
    public NetLess8TimeOnlyValueConverter() : base(
        timeOnly => timeOnly.ToTimeSpan(),
        timeSpan => TimeOnly.FromTimeSpan(timeSpan)) { }
}
#endif
