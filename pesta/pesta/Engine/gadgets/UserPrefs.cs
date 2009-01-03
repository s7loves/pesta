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
    public class UserPrefs
    {
        /**
        * Convenience representation of an empty pref set.
        */
        public static readonly UserPrefs EMPTY = new UserPrefs();
        private Dictionary<String, String> prefs;

        /**
        * @return An immutable reference to all prefs.
        */
        public Dictionary<String, String> getPrefs() 
        {
            return prefs;
        }

        /**
        * @param name The pref to fetch.
        * @return The pref specified by the given name.
        */
        public String getPref(String name)
        {
            String retVal = null;
            prefs.TryGetValue(name, out retVal);
            return retVal;
        }

        public override String ToString() 
        {
            return prefs.ToString();
        }

        /**
        * @param prefs The preferences to populate.
        */
        public UserPrefs(Dictionary<String, String> prefs) 
        {
            this.prefs = new Dictionary<String, String>(prefs);
        }

        /**
        * Creates an empty user prefs object.
        */
        private UserPrefs() 
        {
            // just use the empty map.
            this.prefs = new Dictionary<string,string>();
        }
    }
}