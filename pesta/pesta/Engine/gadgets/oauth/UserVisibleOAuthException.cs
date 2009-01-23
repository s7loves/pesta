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

namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Exceptions whose message text should be shown to gadget developers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [Serializable]
    public class UserVisibleOAuthException : GadgetException
    {
        private OAuthError oauthErrorCode;

        public UserVisibleOAuthException(String msg)
            : base(Code.INVALID_PARAMETER, msg)
        {
        }

        public UserVisibleOAuthException(String msg, Exception t)
            : base(Code.INVALID_PARAMETER, msg, t)
        {
        }

        public UserVisibleOAuthException(OAuthError oauthErrorCode, String msg)
            : base(Code.INVALID_PARAMETER, msg)
        {
            this.oauthErrorCode = oauthErrorCode;
        }
        /**
       * @return the OAuth error code, or null if no code was specified.
       */
        public OAuthError getOAuthErrorCode()
        {
            return oauthErrorCode;
        }
    }
}