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
using Jayrock;
using Pesta.Libraries.OAuth.signature;

namespace Pesta.Libraries.OAuth
{
    public class SimpleOAuthValidator : OAuthValidator
    {
        // default window for timestamps is 5 minutes
        public static readonly long DEFAULT_TIMESTAMP_WINDOW = 5 * 60 * 1000L;

        /**
         * Construct a validator that rejects messages more than five minutes out
         * of date, or with a OAuth version other than 1.0, or with an invalid
         * signature.
         */
        public SimpleOAuthValidator()
        {
            timestampWindow = DEFAULT_TIMESTAMP_WINDOW;
            maxVersion = double.Parse(OAuth.VERSION_1_0);
        }

        /**
         * Public constructor.
         *
         * @param timestampWindowSec
         *            specifies, in seconds, the windows (into the past and
         *            into the future) in which we'll accept timestamps.
         * @param maxVersion
         *            the maximum acceptable oauth_version
         */
        public SimpleOAuthValidator(long timestampWindowMsec, double maxVersion)
        {
            timestampWindow = timestampWindowMsec;
            this.maxVersion = maxVersion;
        }

        protected readonly double minVersion = 1.0;
        protected readonly double maxVersion;
        protected readonly long timestampWindow;

        /** {@inherit} 
         * @throws URISyntaxException */
        public void validateMessage(OAuthMessage message, OAuthAccessor accessor)
        {
            validateVersion(message);
            validateTimestampAndNonce(message);
            validateSignature(message, accessor);
        }

        protected void validateVersion(OAuthMessage message)
        {
            String versionString = message.getParameter(OAuth.OAUTH_VERSION);
            if (versionString != null)
            {
                double version = double.Parse(versionString);
                if (version < minVersion || maxVersion < version)
                {
                    OAuthProblemException problem = new OAuthProblemException("version_rejected");
                    problem.setParameter("oauth_acceptable_versions", minVersion + "-" + maxVersion);
                    throw problem;
                }
            }
        }

        /** This implementation doesn't check the nonce value. */
        protected void validateTimestampAndNonce(OAuthMessage message)
        {
            message.requireParameters(new[] { OAuth.OAUTH_TIMESTAMP, OAuth.OAUTH_NONCE });
            DateTime timestamp = UnixTime.ToDateTime(double.Parse(message.getParameter(OAuth.OAUTH_TIMESTAMP)));
            DateTime now = DateTime.UtcNow;
            DateTime min = now.AddSeconds(0 - timestampWindow);
            DateTime max = now.AddSeconds(timestampWindow);
            if (timestamp < min || max < timestamp)
            {
                OAuthProblemException problem = new OAuthProblemException("timestamp_refused");
                problem.setParameter("oauth_acceptable_timestamps", min + "-" + max);
                throw problem;
            }
        }

        protected void validateSignature(OAuthMessage message, OAuthAccessor accessor)
        {
            message.requireParameters(new[]{OAuth.OAUTH_CONSUMER_KEY,
                                            OAuth.OAUTH_SIGNATURE_METHOD, OAuth.OAUTH_SIGNATURE});
            OAuthSignatureMethod.newSigner(message, accessor).validate(message);
        }
    }
}