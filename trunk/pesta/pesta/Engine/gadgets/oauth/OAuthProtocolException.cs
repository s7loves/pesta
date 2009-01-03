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
using System.Collections.Generic;
using System;
using Pesta.Interop.oauth;


namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Implements the
    /// <a href="http://wiki.oauth.net/ProblemReporting">
    /// OAuth problem reporting extension</a>
    /// We divide problems into two categories:
    /// - problems that cause us to abort the protocol.  For example, if we don't
    /// have a consumer key that the service provider accepts, we give up.
    /// - problems that cause us to ask for the user's permission again.  For
    /// example, if the service provider reports that an access token has been
    /// revoked, we throw away the token and start over.
    /// By default we assume most service provider errors fall into the second
    /// category: we should ask for the user's permission again.
    /// TODO: add a third category to cope with reauthorization per the ScalableOAuth
    /// extension.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    [Serializable]
    internal class OAuthProtocolException : Exception
    {

        static OAuthProtocolException()
        {
            fatalProblems = new List<String>();
            fatalProblems.Add("version_rejected");
            fatalProblems.Add("signature_method_rejected");
            fatalProblems.Add("consumer_key_unknown");
            fatalProblems.Add("consumer_key_rejected");
            fatalProblems.Add("timestamp_refused");
            temporaryProblems = new List<String>();
            temporaryProblems.Add("consumer_key_refused");
        }

        /// <summary>
        /// Problems that should force us to abort the protocol right away,
        /// and next time the user visits ask them for permission again.
        /// </summary>
        ///
        private static List<string> fatalProblems;

        /// <summary>
        /// Problems that should force us to abort the protocol right away,
        /// but we can still try to use the access token again later.
        /// </summary>
        ///
        private static List<String> temporaryProblems;

        private readonly String problemCode;
        private readonly String problemText;

        public readonly bool canRetry;

        public readonly bool startFromScratch;

        internal OAuthProtocolException(bool canRetry_0)
        {
            this.problemCode = null;
            this.problemText = null;
            this.canRetry = canRetry_0;
            this.startFromScratch = false;
        }

        public OAuthProtocolException(OAuthMessage reply)
        {
            String problem = reply.getParameter(OAuthProblemException.OAUTH_PROBLEM);
            if (problem == null)
            {
                throw new ArgumentException(
                    "No problem reported for OAuthProtocolException");
            }
            this.problemCode = problem;
            this.problemText = reply.getParameter("oauth_problem_advice");
            if (fatalProblems.Contains(problem))
            {
                startFromScratch = true;
                canRetry = false;
            }
            else if (temporaryProblems.Contains(problem))
            {
                startFromScratch = false;
                canRetry = false;
            }
            else
            {
                startFromScratch = true;
                canRetry = true;
            }
        }

        /// <summary>
        /// Handle OAuth protocol errors for SPs that don't support the problem
        /// reporting extension
        /// </summary>
        ///
        /// <param name="status"> HTTP status code, assumed to be between 400 and 499 inclusive</param>
        public OAuthProtocolException(int status)
        {
            problemCode = status.ToString();
            problemText = null;
            if (status == 401)
            {
                startFromScratch = true;
                canRetry = true;
            }
            else
            {
                startFromScratch = true;
                canRetry = false;
            }
        }
    }
}