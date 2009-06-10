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
using System.Reflection;

namespace Pesta.Engine.common
{
    /// <summary> Handles loading contents from resource and file system files.</summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public static class ResourceLoader
    {
        public static String GetContent(String resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream s = assembly.GetManifestResourceStream(resource);
            if (s == null)
            {
                throw new FileNotFoundException("Can not locate resource: " + resource);
            }

            StreamReader sr = new StreamReader(s);
            String retval = sr.ReadToEnd();
            s.Close();
            return retval;
        }

        public static String GetContent(FileInfo file)
        {
            return file.OpenText().ReadToEnd();
        }
    }
}