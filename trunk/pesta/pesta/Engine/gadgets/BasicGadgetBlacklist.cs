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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using URI = System.Uri;

namespace Pesta.Engine.gadgets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    class BasicGadgetBlacklist : GadgetBlacklist
    {
        private static readonly char COMMENT_MARKER = '#';
        private static readonly String REGEXP_PREFIX = "REGEXP";

        private readonly HashSet<String> exactMatches;
        private readonly List<Regex> regexpMatches;

        /**
        * Constructs a new blacklist from the given file.
        *
        * @param blacklistFile file containing blacklist entries
        * @throws IOException if reading the file fails
        * @throws PatternSyntaxException if an invalid regular expression occurs in
        *    the file
        */
        public BasicGadgetBlacklist(String file)
        {
            exactMatches = new HashSet<String>();
            regexpMatches = new List<Regex>();
            if (file == "")
            {
                return;
            }
            FileInfo blacklistFile = new FileInfo(file);
            if (blacklistFile.Exists)
            {
                parseBlacklist(blacklistFile);
            }
        }

        private void parseBlacklist(FileInfo blacklistFile)
        {
            StreamReader reader = new StreamReader(blacklistFile.FullName);
            String line;
            while ((line = reader.ReadLine()) != null) 
            {
                line = line.Trim();
                if (line.Length == 0 || line[0] == COMMENT_MARKER) 
                {
                    continue;
                }

                String[] parts = Regex.Split(line,"\\s+");
                if (parts.Length == 1) 
                {
                    exactMatches.Add(line.ToLower());
                } 
                else if (parts.Length == 2 && parts[0].ToUpper().Equals(REGEXP_PREFIX)) 
                {
                    // compile will throw PatternSyntaxException on invalid patterns.
                    regexpMatches.Add(new Regex(parts[1], RegexOptions.Compiled | RegexOptions.IgnoreCase));
                }
            }
        }

        /** {@inheritDoc} */
        public bool isBlacklisted(URI gadgetUri) 
        {
            String uriString = gadgetUri.ToString().ToLower();
            if (exactMatches.Contains(uriString)) 
            {
                return true;
            }
            foreach(Regex pattern in regexpMatches) 
            {
                if (pattern.Match(uriString).Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}