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
using System.Collections.ObjectModel;
using System.Text;
using Pesta.Interop;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.variables
{
    /// <summary>
    /// Performs string substitutions for message bundles, user prefs, and bidi
    /// variables.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class Substitutions
    {
        /**
	 * Defines all of the valid types of message substitutions. Note: Order is
	 * important here, since the order of the enum is the order of evaluation.
	 * Don't change this unless you know what you're doing.
	 */
        public class Type : EnumBaseType<Type>
        {
            public Type(int key, string value)
                : base(key, value)
            {
                this.prefix = "__" + value + '_';
            }
            public static readonly Type MESSAGE = new Type(1, "MSG");
            public static readonly Type BIDI = new Type(2, "BIDI");
            public static readonly Type USER_PREF = new Type(3, "UP");
            public static readonly Type MODULE = new Type(4, "MODULE");

            private String prefix;
            public String getPrefix()
            {
                return prefix;
            }
            public static ReadOnlyCollection<Type> GetValues()
            {
                return GetBaseValues();
            }
        }

        private Dictionary<Substitutions.Type, Dictionary<String, String>> substitutions;

        /// <summary>
        /// Create a basic substitution coordinator.
        /// </summary>
        ///
        public Substitutions()
        {
            this.substitutions = new Dictionary<Type, Dictionary<string, string>>();
            /* foreach */
            foreach (Substitutions.Type type in Substitutions.Type.GetValues())
            {
                substitutions.Add(type, new Dictionary<String, String>());
            }
        }
        /// <summary>
        /// Adds a new substitution for the given type.
        /// </summary>
        ///
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void addSubstitution(Substitutions.Type type, String key, String value_ren)
        {
            substitutions[type].Add(key, value_ren);
        }

        /// <summary>
        /// Adds many substitutions of the same type at once.
        /// </summary>
        ///
        /// <param name="type"></param>
        /// <param name="entries"></param>
        public void addSubstitutions(Substitutions.Type type, Dictionary<string, string> entries)
        {
            foreach (var entry in entries)
            {
                substitutions[type].Add(entry.Key, entry.Value);
            }
        }


        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns>The substitution set under the given type / name, or null.</returns>
        public String getSubstitution(Substitutions.Type type, String name)
        {
            return substitutions[type][name];
        }

        /// <summary>
        /// Performs string substitution only for the specified type. If no
        /// substitution for {@code input} was provided or {@code input} is null, the
        /// output is left untouched.
        /// </summary>
        ///
        /// <param name="type">The type you wish to perform substitutions for.</param>
        /// <param name="input">The base string, with substitution markers.</param>
        /// <returns>The substituted string.</returns>
        public String substituteString(Substitutions.Type type, String input)
        {
            if (input == null)
            {
                return null;
            }

            if (type == null)
            {
                /* foreach */
                foreach (Substitutions.Type t in Substitutions.Type.GetValues())
                {
                    input = substituteString(t, input);
                }
                return input;
            }

            if (substitutions[type].Count == 0 || !input.Contains(type.getPrefix()))
            {
                return input;
            }

            StringBuilder output = new StringBuilder();
            string compareString;
            for (int i = 0, j = input.Length - 1; i < j; ++i)
            {
                if (i + type.getPrefix().Length > input.Length)
                {
                    compareString = input.Substring(i);
                }
                else
                {
                    compareString = input.Substring(i, type.getPrefix().Length);
                }
                if (compareString.CompareTo(type.getPrefix()) == 0)
                {
                    // Look for a trailing "__". If we don't find it, then this isn't a
                    // properly formed substitution.
                    int start = i + type.getPrefix().Length;
                    int end = input.IndexOf("__", start);
                    int length = end - start;
                    if (end != -1)
                    {
                        String name = input.Substring(start, length);
                        String replacement = (string)(substitutions[type][name]);
                        if (replacement != null)
                        {
                            output.Append(replacement);
                        }
                        else
                        {
                            output.Append(input.Substring(i, length + 2));
                        }
                        i = end + 1;
                    }
                    else
                    {
                        // If we didn't find any occurances of "__", then the rest of the
                        // string can't contain any more valid translations.
                        output.Append(input.Substring(i));
                        break;
                    }
                }
                else
                {
                    output.Append(input[i]);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Substitutes a uri
        /// </summary>
        ///
        /// <param name="type">The type to substitute, or null for all types.</param>
        /// <param name="uri"></param>
        /// <returns>The substituted uri, or a dummy value if the result is invalid.</returns>
        public Uri substituteUri(Type type, Uri uri)
        {
            if (uri == null)
            {
                return null;
            }
            try
            {
                return Uri.parse(substituteString(type, uri.ToString()));
            }
            catch (Exception e)
            {
                return Uri.parse("");
            }
        }
    }
}