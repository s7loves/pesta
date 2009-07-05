using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;

namespace Pesta.Engine.protocol.conversion
{
    public class DataContractJSConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException("Unable to deserialize " + type.Name);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var info in properties)
            {
                var value = info.GetValue(obj, null);
                if (info.IsDefined(typeof(DataMemberAttribute), false) && value != null)
                {
                    var valueType = value.GetType();
                    // following needed, otherwise enums are serialised into numbers
                    if (valueType.IsEnum)
                    {
                        value = value.ToString();
                    }
                    // handle scenario where only one entry
                    else if (typeof(IRestfulCollection).IsAssignableFrom(type) &&
                                typeof(IList).IsAssignableFrom(valueType))
                    {
                        var entry = ((IList) value);

                        if (entry.Count == 1)
                        {
                            result.Add(info.Name, entry[0]);
                            continue;
                        }
                    }
                    result.Add(info.Name, value);
                }
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[]
                           {
                               typeof(Person), 
                               typeof(Account), 
                               typeof(Activity), 
                               typeof(Address), 
                               typeof(BodyType), 
                               typeof(ListField),
                               typeof(MediaItem),
                               typeof(Message),
                               typeof(MessageCollection),
                               typeof(Name),
                               typeof(Organization),
                               typeof(Url),
                               typeof(RestfulCollection<Person>),
                               typeof(RestfulCollection<Activity>),
                           };
            }
        }
    }
}
