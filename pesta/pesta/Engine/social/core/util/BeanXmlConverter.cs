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
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Xml;

/// <summary>
/// Summary description for BeanXmlConverter
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class BeanXmlConverter : BeanConverter
{
    public String convertToString(Object pojo) 
    {
        return convertToXml(pojo);
    }

    public String convertToXml(Object obj) 
    {
        XmlSerializer serial = new XmlSerializer(obj.GetType());
        MemoryStream stream = new MemoryStream();
        serial.Serialize(stream, obj);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public object convertToObject(String xml, Type className) 
    {
        XmlSerializer serial = new XmlSerializer(className);
        TextReader reader = new StringReader(xml);
        return serial.Deserialize(reader);
    }
}
