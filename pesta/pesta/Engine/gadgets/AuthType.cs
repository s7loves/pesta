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
/// Summary description for AuthType
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class AuthType : EnumBaseType<AuthType>
{
    public AuthType(int key, string value)
        : base(key,value)
    {
    }
    public readonly static AuthType NONE = new AuthType(1, "NONE");
    public readonly static AuthType SIGNED = new AuthType(2, "SIGNED");
    public readonly static AuthType OAUTH = new AuthType(3, "OAUTH");

    /**
   * @return The parsed value (defaults to NONE)
   */
    public static AuthType Parse(String value)
    {
        if (value != null) 
        {
            value = value.Trim();
            if (value.Length == 0)
            {
                return NONE;
            }
            try 
            {
                return GetBaseByValue(value);
            } 
            catch (ArgumentException iae) 
            {
                return NONE;
            }
        } 
        else 
        {
            return NONE;
        }
    }

  /**
   * Use lowercase as toString form
   * @return string value of Auth type
   */

    public override String ToString() 
    {
        return base.ToString().ToLower();
    }
}
