namespace swashbuckletest.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class CustomIEnumerableConverter : JsonConverter
    {
        private readonly IEnumerable<string> types;

        public CustomIEnumerableConverter(IEnumerable<string> types)
        {
            this.types = types ?? throw new ArgumentNullException("types");
        }

        public override bool CanRead => false;

        public override void WriteJson(JsonWriter jsonWriter, object value, JsonSerializer serializer)
        {
            var strategy = (serializer.ContractResolver as DefaultContractResolver)?.NamingStrategy;

            jsonWriter.WriteStartObject();

            var foundInnerEnumerable = false;
            foreach (PropertyInfo property in value.GetType().GetTypeInfo().GetProperties())
            {
                try
                {
                    object propVal = property.GetValue(value, null);
                    foundInnerEnumerable |= IsIenumerable(propVal);
                    jsonWriter.WritePropertyName(strategy == null ? property.Name : strategy.GetPropertyName(property.Name, false));
                    serializer.Serialize(jsonWriter, propVal);
                }
                catch (TargetParameterCountException)
                {

                }
            }

            if (!foundInnerEnumerable)
            {
                jsonWriter.WritePropertyName(strategy == null ? "Items" : strategy.GetPropertyName("Items", false));
                jsonWriter.WriteStartArray();
                foreach (var item in (IEnumerable)value)
                {
                    serializer.Serialize(jsonWriter, item);
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        private static bool IsIenumerable(object propVal)
        {
            return propVal is IEnumerable;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return this.types.Any(t => objectType.FullName.StartsWith(t));
        }
    }
}
