/*
 * Copyright 2007 Netflix, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace Pesta.Interop.oauth.signature
{
    /// <summary>
    /// Summary description for PLAINTEXT
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class PLAINTEXT : OAuthSignatureMethod
    {

        protected override String getSignature(String baseString)
        {
            return getSignature();
        }

        protected override bool isValid(String signature, String baseString)
        {
            return signature.Equals(getSignature());
        }

        private String getSignature()
        {
            if (signature == null)
            {
                signature = OAuth.percentEncode(getConsumerSecret()) + '&'
                            + OAuth.percentEncode(getTokenSecret());
            }
            return signature;
        }

        private String signature = null;

        public override void setConsumerSecret(String consumerSecret)
        {
            lock (this)
            {
                signature = null;
            }
            base.setConsumerSecret(consumerSecret);
        }

        public override void setTokenSecret(String tokenSecret)
        {
            lock (this)
            {
                signature = null;
            }
            base.setTokenSecret(tokenSecret);
        }
    }
}