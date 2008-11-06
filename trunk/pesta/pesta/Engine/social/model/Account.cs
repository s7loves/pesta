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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


/// <summary>
/// Summary description for Account
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
[ImplementedBy(typeof(AccountImpl))]
public abstract class Account
{
    /**
   * The fields that represent the account object in json form.
   *
   * <p>
   * All of the fields that an account can have, all fields are required
   * </p>
   *
   */
    public class Field : EnumBaseType<Field>
    {
        public Field(int key, string value) 
            : base(key,value)
        {

        }
        public static readonly Field DOMAIN = new Field(1, "domain");
        public static readonly Field USER_ID = new Field(2, "userId");
        public static readonly Field USERNAME = new Field(3, "username");

        /**
        * The json field that the instance represents.
        */
        private readonly String jsonString;

        /**
        * create a field base on the a json element.
        *
        * @param jsonString the name of the element
        */
        private Field(String jsonString) 
        {
            this.jsonString = jsonString;
        }

        /**
        * emit the field as a json element.
        *
        * @return the field name
        */
        public override String ToString() 
        {
            return this.jsonString;
        }
    }

    /**
    * The top-most authoritative domain for this account, e.g. "twitter.com". This is the Primary
    * Sub-Field for this field, for the purposes of sorting and filtering.
    *
    * @return the domain
    */
    public abstract String getDomain();

    /**
    * The top-most authoritative domain for this account, e.g. "twitter.com". This is the Primary
    * Sub-Field for this field, for the purposes of sorting and filtering. *
    *
    * @param domain the domain
    */
    public abstract void setDomain(String domain);

    /**
    * A user ID number, usually chosen automatically, and usually numeric but sometimes alphanumeric,
    * e.g. "12345" or "1Z425A".
    *
    * @return the userId
    */
    public abstract String getUserId();

    /**
    * A user ID number, usually chosen automatically, and usually numeric but sometimes alphanumeric,
    * e.g. "12345" or "1Z425A".
    *
    * @param userId the userId
    */
    public abstract void setUserId(String userId);

    /**
    * An alphanumeric user name, usually chosen by the user, e.g. "jsmarr".
    *
    * @return the username
    */
    public abstract String getUsername();

    /**
    * An alphanumeric user name, usually chosen by the user, e.g. "jsmarr".
    *
    * @param username the username
    */
    public abstract void setUsername(String username);
}
