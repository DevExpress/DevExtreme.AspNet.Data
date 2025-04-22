using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevExtreme.AspNet.Data {

    static class Compatibility {
        public static IList UnwrapList(IList deserializedList) {
            var unwrappedList = new List<object>();
            foreach(var item in deserializedList)
                unwrappedList.Add(UnwrapJsonElement(item));
            return unwrappedList;
        }

        static object UnwrapJsonElement(object deserializeObject) {
            if(deserializeObject == null)
                return null;

            if(!(deserializeObject is JsonElement jsonElement))
                throw new InvalidOperationException();

            switch(jsonElement.ValueKind) {
                case JsonValueKind.Array:
                    return jsonElement.EnumerateArray().Select(item => UnwrapJsonElement(item)).ToList();
                case JsonValueKind.String:
                    return jsonElement.GetString();
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return jsonElement.GetBoolean();
                case JsonValueKind.Number:
                    //same as IsIntegralType + unsigned?
                    if(jsonElement.TryGetInt32(out var intValue))
                        return intValue;
                    //or floating point as well?
                    //we primarily use Convert.ToDecimal everywhere
                    if(jsonElement.TryGetDecimal(out var decimalValue))
                        return decimalValue;
                    throw new NotImplementedException();
                case JsonValueKind.Object:
                    return jsonElement;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    class NumericAndStringConverter : JsonConverter<string> {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if(reader.TokenType == JsonTokenType.Number)
                return string.Format(CultureInfo.InvariantCulture, "{0}", GetNumber(ref reader));
            return reader.GetString();
        }

        static object GetNumber(ref Utf8JsonReader reader) {
            if(reader.TryGetInt32(out int intValue))
                return intValue;
            if(reader.TryGetDecimal(out decimal decimalValue))
                return decimalValue;
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
    }

    class ListConverter : JsonConverter<IList> {
        public override IList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var deserializedList = JsonSerializer.Deserialize<IList>(ref reader);
            return Compatibility.UnwrapList(deserializedList);
        }

        public override void Write(Utf8JsonWriter writer, IList value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

}
