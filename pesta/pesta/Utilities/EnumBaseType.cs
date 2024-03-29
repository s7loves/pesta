﻿#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
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

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Pesta.Utilities
{
    [IsJavaEnum]
    public abstract class EnumBaseType<T> where T : EnumBaseType<T>
    {
        private static readonly HashSet<T> enumValues = new HashSet<T>();

        public readonly int Key;
        public readonly string JsonValue;
        public readonly string Value;

        public EnumBaseType()
        {
            
        }
        public EnumBaseType(int key, string value)
        {
            Key = key;
            Value = value;
            enumValues.Add((T)this);
        }
        public EnumBaseType(string key, string value)
        {
            JsonValue = key;
            Value = value;
            enumValues.Add((T)this);
        }

        protected static ReadOnlyCollection<T> GetBaseValues()
        {
            return enumValues.ToList().AsReadOnly();
        }

        protected static ReadOnlyCollection<String> GetBaseValueStrings()
        {
            var list = new List<String>();
            foreach (T t in enumValues)
            {
                list.Add(t.Value);
            }
            return list.AsReadOnly();
        }

        public static T GetBaseByKey(int key)
        {
            foreach (T t in enumValues)
            {
                if (t.Key == key) return t;
            }
            return null;
        }
        public static T GetBaseByKey(string key)
        {
            foreach (T t in enumValues)
            {
                if (string.Compare(t.JsonValue, key, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return t;
            }
            return null;
        }

        public static T GetBaseByValue(string value)
        {
            foreach (T t in enumValues)
            {
                if (string.Compare(t.Value,value, StringComparison.InvariantCultureIgnoreCase) == 0) 
                    return t;
            }
            return null;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}