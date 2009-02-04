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
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using Pesta.Engine.auth;
using Pesta.Engine.common.crypto;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.social.service;


namespace pestaServer.Models.gadgets.oauth
{
    /// <summary>
    /// Summary description for OAuthResponseParams
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthResponseParams
    {

        // Finds the values of sensitive response params: oauth_token_secret and oauth_session_handle
        private static readonly Regex REMOVE_SECRETS = new Regex("(?<=(oauth_token_secret|oauth_session_handle)=)[^=& \t\r\n]*", RegexOptions.Compiled);

        // names for the JSON values we return to the client
        public static String CLIENT_STATE = "oauthState";
        public static String APPROVAL_URL = "oauthApprovalUrl";
        public static String ERROR_CODE = "oauthError";
        public static String ERROR_TEXT = "oauthErrorText";

        /**
        * Transient state we want to cache client side.
        */
        private readonly OAuthClientState newClientState;

        /**
        * Security token used to authenticate request.
        */
        private readonly ISecurityToken securityToken;

        /**
        * Original request from client.
        */
        private readonly sRequest originalRequest;

        /**
        * Request/response pairs we sent onward.
        */
        private readonly List<Pair> requestTrace = new List<Pair>();

        /**
        * Authorization URL for the client.
        */
        private String aznUrl;

        /**
        * Error code for the client.
        */
        private String error;

        /**
        * Error text for the client.
        */
        private String errorText;


        /**
         * Whether we should include the request trace in the response to the application.
         *
         * It might be nice to make this configurable based on options passed to makeRequest.  For now
         * we use some heuristics to figure it out.
         */
        private bool sendTraceToClient;

        /**
         * Create response parameters.
         */
        public OAuthResponseParams(ISecurityToken securityToken, sRequest originalRequest,
            BlobCrypter stateCrypter)
        {
            this.securityToken = securityToken;
            this.originalRequest = originalRequest;
            newClientState = new OAuthClientState(stateCrypter);
        }

      /**
       * Log a warning message that includes the details of the request.
       */
        public void logDetailedWarning(String note) 
        {
            Debug.WriteLine(note + '\n' + getDetails());
        }

        /**
        * Log a warning message that includes the details of the request and the thrown exception.
        */
        public void logDetailedWarning(String note, Exception cause) 
        {
            Debug.WriteLine(note + '\n' + getDetails() + '\n' + cause.Message);
        }

        /**
        * Add a request/response pair to our trace of actions associated with this request.
        */
        public void addRequestTrace(sRequest request, sResponse response)
        {
            requestTrace.Add(new Pair(request, response));
        }

        /**
        * @return true if the target server returned an error at some point during the request
        */
        public bool sawErrorResponse() 
        {
            foreach(var entry in requestTrace)
            {
                if (entry.Second == null || ((sResponse)(entry.Second)).isError())
                {
                    return true;
                }
            }
            return false;
        }

        private String getDetails() 
        {
        return "OAuth error [" + error + ", " + errorText + "] for application " +
        securityToken.getAppUrl() + ".  Request trace:" + getRequestTrace();
        }

        private String getRequestTrace() 
        {
            StringBuilder trace = new StringBuilder();
            trace.Append("\n==== Original request:\n");
            trace.Append(originalRequest);
            trace.Append("\n====");
            int i = 1;
            foreach(var entry in requestTrace) 
            {
                trace.Append("\n==== Sent request " + i + ":\n");
                if (entry.First != null) 
                {
                    trace.Append(filterSecrets(entry.First.ToString()));
                }
                trace.Append("\n==== Received response " + i + ":\n");
                if (entry.Second != null) 
                {
                    trace.Append(filterSecrets(entry.Second.ToString()));
                }
                trace.Append("\n====");
                ++i;
            }
            return trace.ToString();
        }

        /**
        * Removes security sensitive parameters from requests and responses.
        */
        static String filterSecrets(String _in) 
        {
            Match m = REMOVE_SECRETS.Match(_in);
            if (m.Success)
            {
                return m.Result("REMOVED");
            }
            return _in;
        }

        public void addToResponse(HttpResponseBuilder response)
        {
            if (!newClientState.isEmpty())
            {
                try
                {
                    response.setMetadata(CLIENT_STATE, newClientState.getEncryptedState());
                }
                catch (BlobCrypterException e)
                {
                    // Configuration error somewhere, this should never happen.
                    throw e;
                }
            }
            if (!String.IsNullOrEmpty(aznUrl))
            {
                response.setMetadata(APPROVAL_URL, aznUrl);
            }
            if (error != null)
            {
                response.setMetadata(ERROR_CODE, error);
            }
            if (errorText != null || sendTraceToClient)
            {
                StringBuilder verboseError = new StringBuilder();
                if (errorText != null)
                {
                    verboseError.Append(errorText);
                }
                if (sendTraceToClient)
                {
                    verboseError.Append('\n');
                    verboseError.Append(getRequestTrace());
                }
                response.setMetadata(ERROR_TEXT, verboseError.ToString());
            }
        }

        public OAuthClientState getNewClientState()
        {
            return newClientState;
        }

        public String getAznUrl()
        {
            return aznUrl;
        }

        public void setAznUrl(String _aznUrl)
        {
            aznUrl = _aznUrl;
        }

        public void setSendTraceToClient(bool _sendTraceToClient) 
        {
            sendTraceToClient = _sendTraceToClient;
        }

        public String getError() 
        {
            return error;
        }

        public OAuthRequestException oauthRequestException(OAuthError _error, String _errorText)
        {
            return oauthRequestException(_error.ToString(), _errorText);
        }

        public OAuthRequestException oauthRequestException(OAuthError _error, String _errorText,
                    Exception cause)
        {
            return oauthRequestException(_error.ToString(), _errorText, cause);
        }

        /**
        * Create an exception and record information about the exception to be returned to the gadget.
        */
        public OAuthRequestException oauthRequestException(String _error, String _errorText)
        {
            error = DataRequestHandler.Preconditions<string>.checkNotNull(_error);
            errorText = DataRequestHandler.Preconditions<string>.checkNotNull(_errorText);
            return new OAuthRequestException('[' + error + ',' + errorText + ']');
        }

        /**
        * Create an exception and record information about the exception to be returned to the gadget.
        */
        public OAuthRequestException oauthRequestException(String _error, String _errorText,
          Exception cause)
        {
            error = DataRequestHandler.Preconditions<string>.checkNotNull(_error);
            errorText = DataRequestHandler.Preconditions<string>.checkNotNull(_errorText);
            return new OAuthRequestException('[' + error + ',' + errorText + ']', cause);
        }

      /**
       * Superclass for all exceptions thrown from OAuthRequest and friends.
       *
       * The constructors are private, use OAuthResponseParams.oauthRequestException to create this
       * exception.  This makes sure that any exception thrown is also exposed to the calling gadget
       * in a useful way.
       */
        public class OAuthRequestException : Exception 
        {
            public OAuthRequestException(String message) 
            : base(message)
            {
            }

            public OAuthRequestException(String message, Exception cause)
            : base(message, cause)
            {
            }
        }
    }
}