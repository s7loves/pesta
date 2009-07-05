using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Pesta.Utilities.Helpers
{
    public static class JSONHelper
    {
        /*
        /// <summary>
        /// serialize object into JSON string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            string json = string.Empty;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                json = Encoding.UTF8.GetString(ms.ToArray());
            }
            return json; 
        }

        /// <summary>
        /// Deserialize a JSON string into an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJSON<T>(this string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }
        */
        /// <summary>
        /// modifies a Dictionary with value of type object to be serializable using JavascriptSerializer
        /// </summary>
        /// <param name="target"></param>
        /// <param name="criterion">only modify values matching this criteria</param>
        public static void SerializeDataContracts(this Dictionary<string, object> target)
        {
            for (int i = 0; i < target.Count; i++)
            {
                var key = target.Keys.ElementAt(i);
                var value = target.Values.ElementAt(i);
                if (value.GetType().GetCustomAttributes(typeof(DataContractAttribute), false).Length != 0)
                {
                    MemoryStream ms = new MemoryStream();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(value.GetType());
                    ser.WriteObject(ms, value);
                    target[key] = Encoding.UTF8.GetString(ms.ToArray());
                    //BeanJsonConverter converter = new BeanJsonConverter();
                    //target[key] = converter.ConvertToJson(value);
                }
            }
        }
    }
}