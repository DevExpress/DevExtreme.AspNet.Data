using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample {

    class BsonValueConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return typeof(BsonValue).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var bson = value as BsonValue;
            if(bson is BsonDateTime bsonDate) {
                writer.WriteValue(bsonDate.ToUniversalTime());
            } else {
                writer.WriteRawValue(bson.ToJson());
            }
        }
    }

}
