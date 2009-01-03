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

namespace Pesta.Interop
{
    public class Locale
    {
        private readonly string _language;
        private readonly string _country;
        public Locale(string language, string country)
        {
            _language = language;
            _country = country;
        }

        public string getLanguage()
        {
            return _language;
        }
        public string getCountry()
        {
            return _country;
        }

        public override bool Equals(object obj)
        {
            if (obj is Locale == false)
            {
                return base.Equals(obj);
            }
            Locale loc = (Locale) obj;

            return string.Compare(loc._country, _country, true) == 0 &&
                   string.Compare(loc._language, _language, true) == 0;
        }
        
        public override int GetHashCode()
        {
            return ((_language != null ? _language.GetHashCode() : 0) * 397) ^ (_country != null ? _country.GetHashCode() : 0);
        }
        
    }
}