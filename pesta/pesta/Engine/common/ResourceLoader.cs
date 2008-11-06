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
using System.Text;

namespace Pesta
{
    /// <summary> Handles loading contents from resource and file system files.</summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ResourceLoader
    {

        /// <summary> Opens a given path as either a resource or a file, depending on the path
        /// name.
        /// 
        /// If path starts with res://, we interpret it as a resource.
        /// Otherwise we attempt to load it as a file.
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <returns> The opened input stream
        /// </returns>
        public static Stream open(String path)
        {
            return openResource(path);
            /* if (path.StartsWith("res://"))
             {
                 return openResource(path.Substring(6));
             }
             FileInfo file = new FileInfo(path);
             return file.OpenRead();*/
        }

        /// <param name="resource">
        /// </param>
        /// <returns> An input stream for the given named resource
        /// </returns>
        /// <throws>  FileNotFoundException </throws>
        public static Stream openResource(String resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream s = assembly.GetManifestResourceStream(resource);
            if (s == null)
            {
                // try shindig.dll
                string location = "ikvm__" + resource.Replace('/', '!');
                s = AppDomain.CurrentDomain.Load("shindig").GetManifestResourceStream(location);
                if (s == null)
                {
                    throw new FileNotFoundException("Can not locate resource: " + resource);
                }
            }
            return s;
        }

        /// <summary> Reads the contents of a resource as a string.
        /// 
        /// </summary>
        /// <param name="resource">
        /// </param>
        /// <returns> Contents of the resource.
        /// </returns>
        /// <throws>  IOException </throws>
        public static String getContent(String resource)
        {
            string location = "";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream s = assembly.GetManifestResourceStream(resource);
            if (s == null)
            {
                // try shindig.dll
                location = "ikvm__" + resource.Replace('/', '!');
                s = AppDomain.CurrentDomain.Load("shindig").GetManifestResourceStream(location);
                if (s == null)
                {
                    throw new FileNotFoundException("Can not locate resource: " + resource);
                }
            }

            StreamReader sr = new StreamReader(s);
            String retval = sr.ReadToEnd();
            if (location != "")
            {
                return retval.Substring(retval.IndexOf("var"));
            }

            return retval;
            
        }

        /// <param name="file">
        /// </param>
        /// <returns> The contents of the file (assumed to be UTF-8).
        /// </returns>
        /// <throws>  IOException </throws>
        public static String getContent(FileInfo file)
        {
            return file.OpenText().ReadToEnd();
        }
    }   
}
