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
using System.Net;
using Pesta.Utilities;

namespace Pesta.Engine.social
{
    /// <summary>
    /// Summary description for ResponseError
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ResponseError : EnumBaseType<ResponseError>
    {
        public ResponseError(int key, string value)
            : base(key, value)
        {
            this.jsonValue = value;
            this.httpErrorCode = key;
        }

        public readonly static ResponseError NOT_IMPLEMENTED = new ResponseError((int)HttpStatusCode.NotImplemented, "notImplemented");
        public readonly static ResponseError UNAUTHORIZED = new ResponseError((int)HttpStatusCode.Unauthorized, "unauthorized");
        public readonly static ResponseError FORBIDDEN = new ResponseError((int)HttpStatusCode.Forbidden, "forbidden");
        public readonly static ResponseError BAD_REQUEST = new ResponseError((int)HttpStatusCode.BadRequest, "badRequest");
        public readonly static ResponseError INTERNAL_ERROR = new ResponseError((int)HttpStatusCode.InternalServerError, "internalError");
        public readonly static ResponseError LIMIT_EXCEEDED = new ResponseError((int)HttpStatusCode.ExpectationFailed, "limitExceeded");
        public readonly static ResponseError NOT_FOUND = new ResponseError((int)HttpStatusCode.NotFound, "notFound");

        /**
       * The json value of the error.
       */
        private readonly String jsonValue;
        /**
         * The http error code associated with the error.
         */
        private int httpErrorCode;

        /**
         *
         * Converts the ResponseError to a String representation
         */
        public override String ToString()
        {
            return jsonValue;
        }

        /**
         * Get the HTTP error code.
         * @return the Http Error code.
         */
        public int getHttpErrorCode()
        {
            return httpErrorCode;
        }
    }
}