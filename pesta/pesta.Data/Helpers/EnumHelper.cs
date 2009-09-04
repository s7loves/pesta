#region License, Terms and Conditions
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
using System.ComponentModel;

namespace pesta.Data.Model.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Returns string specified by DescriptionAttribute, otherwise do the usual ToString()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum obj)
        {
            var attribs = (DescriptionAttribute[])obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
            return attribs.Length > 0 ? attribs[0].Description : obj.ToString();
        }

        public static int ToInt(this Enum obj)
        {
            return Convert.ToInt32(obj);
        }

        public static object ToEnum<T>(this string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return null;
            }
            try
            {
                return Enum.Parse(typeof(T), obj, true);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}