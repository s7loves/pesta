using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Pesta.Engine.protocol.conversion
{
    public class JsonSurrogate : IDataContractSurrogate
    {
        private static List<Type> knownTypes = new List<Type>
                        {
                            typeof(Dictionary<string,string>)
                        };
        [Serializable]
        public class SDictionary : ISerializable
        {
            public Dictionary<string, string> dict;  
            public SDictionary()  
            {
                dict = new Dictionary<string, string>();  
            }
            public SDictionary(Dictionary<string, string> dict)
            {
                this.dict = dict;
            }

            // deserialize
            protected SDictionary(SerializationInfo info, StreamingContext context)  
            {
                dict = new Dictionary<string, string>();  
                foreach (var entry in info)  
                {  
                    dict.Add(entry.Name, entry.Value.ToString());  
                }  
            }  
            // serialize
            public void GetObjectData(SerializationInfo info, StreamingContext context)  
            {  
                foreach (string key in dict.Keys)  
                {
                    info.AddValue(key, dict[key]);
                }  
            }
        }

        private static bool IsKnownType(Type type)
        {
            if (knownTypes.Contains(type))
            {
                return true;
            }
            return false;
        }

        public Type GetDataContractType(Type type)
        {
            if (IsKnownType(type))
            {
                return typeof(SDictionary);
            }

            return type;
        }
        public object GetObjectToSerialize(object obj, Type targetType)
        {
            throw new NotImplementedException();
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj is SDictionary)
            {
                return ((SDictionary) obj).dict;
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        #region not implemented

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
            throw new NotImplementedException();
        }

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}