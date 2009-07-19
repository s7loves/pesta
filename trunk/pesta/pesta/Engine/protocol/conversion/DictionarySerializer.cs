#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain [DataMember(EmitDefaultValue = false)] copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Pesta.Engine.protocol.conversion
{
    public class DictionarySerializer<K, V>
    {
        public struct SerializableKeyValuePair<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            public SerializableKeyValuePair(KeyValuePair<TKey, TValue> kvp)
            {
                Key = kvp.Key;
                Value = kvp.Value;
            }
        }

        private XmlSerializer Serializer = new XmlSerializer(typeof(List<SerializableKeyValuePair<K, V>>));

        public void Serialize(Dictionary<K, V> dictionary, XmlWriter serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }
        public void Serialize(Dictionary<K, V> dictionary, TextWriter serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }
        public void Serialize(Dictionary<K, V> dictionary, Stream serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }

        private List<SerializableKeyValuePair<K, V>> BuildItemList(Dictionary<K, V> dictionary)
        {
            List<SerializableKeyValuePair<K, V>> list = new List<SerializableKeyValuePair<K, V>>();
            foreach (KeyValuePair<K, V> nonSerializableKVP in dictionary)
            {
                list.Add(new SerializableKeyValuePair<K, V>(nonSerializableKVP));
            }

            return list;

        }


        public Dictionary<K, V> Deserialize(XmlReader serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }
        public Dictionary<K, V> Deserialize(TextReader serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }
        public Dictionary<K, V> Deserialize(Stream serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }

        private Dictionary<K, V> BuildDictionary(List<SerializableKeyValuePair<K, V>> dictionaryItems)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>(dictionaryItems.Count);
            foreach (SerializableKeyValuePair<K, V> item in dictionaryItems)
            {
                dictionary.Add(item.Key, item.Value);
            }

            return dictionary;
        }

    }
}
