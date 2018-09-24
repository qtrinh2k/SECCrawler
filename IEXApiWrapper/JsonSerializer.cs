using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IEXApiHandler
{
    public class SerializerHandler
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented,
            ConstructorHandling = ConstructorHandling.Default,
            Error = (se, ev) => { ev.ErrorContext.Handled = true; }
        };

        public static T DeserializeObj<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, jsonSerializerSettings);
        }

        public static string SerializeString<T>(T serialObj)
        {
            return JsonConvert.SerializeObject(serialObj, jsonSerializerSettings);
        }
    }
}
