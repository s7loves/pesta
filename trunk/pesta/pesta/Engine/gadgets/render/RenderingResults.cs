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
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.render
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class RenderingResults
    {
        private readonly Status status;
        private readonly String content;
        private readonly String errorMessage;
        private readonly Uri redirect;

        private RenderingResults(Status status, String content, String errorMessage, Uri redirect) 
        {
            this.status = status;
            this.content = content;
            this.errorMessage = errorMessage;
            this.redirect = redirect;
        }

        public static RenderingResults ok(String content) 
        {
            return new RenderingResults(Status.OK, content, null, null);
        }

        public static RenderingResults error(String errorMessage)
        {
            return new RenderingResults(Status.ERROR, null, errorMessage, null);
        }

        public static RenderingResults mustRedirect(Uri redirect)
        {
            return new RenderingResults(Status.MUST_REDIRECT, null, null, redirect);
        }

        /**
        * @return The status of the rendering operation.
        */
        public Status getStatus() 
        {
            return status;
        }

        /**
        * @return The content to render. Only available when status is OK.
        */
        public String getContent()
        {
            Debug.Assert(status == Status.OK, "Only available when status is OK.");
            return content;
        }

        /**
        * @return The error message for rendering. Only available when status is ERROR.
        */
        public String getErrorMessage()
        {
            Debug.Assert(status == Status.ERROR, "Only available when status is ERROR.");
            return errorMessage;
        }

        /**
        * @return The error message for rendering. Only available when status is ERROR.
        */
        public Uri getRedirect()
        {
            Debug.Assert(status == Status.MUST_REDIRECT, "Only available when status is MUST_REDIRECT.");
            return redirect;
        }

        public enum Status 
        {
            OK, MUST_REDIRECT, ERROR
        }
    }
}