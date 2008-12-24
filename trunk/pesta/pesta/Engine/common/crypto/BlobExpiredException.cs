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
using System.Collections;
using System;
using Pesta.Interop;

namespace Pesta
{
    /// <summary>
    /// Thrown when a blob has expired.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [Serializable]
    public class BlobExpiredException : BlobCrypterException
    {

        public readonly DateTime minDate;
        public readonly DateTime used;
        public readonly DateTime maxDate;

        public BlobExpiredException(long minTime, double now, long maxTime)
            : this(UnixTime.ConvertFromUnixTimestamp(minTime), UnixTime.ConvertFromUnixTimestamp(now), UnixTime.ConvertFromUnixTimestamp(maxTime))
        {
        }

        public BlobExpiredException(DateTime minTime, DateTime now, DateTime maxTime)
            : base("Blob expired, was valid from " + minTime + " to " + maxTime + ", attempted use at " + now)
        {
            this.minDate = minTime;
            this.used = now;
            this.maxDate = maxTime;
        }

    } 
}

