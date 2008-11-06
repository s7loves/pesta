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

namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class ChainedContentFetcher : HttpFetcher
    {
        /// <summary>
        /// next fetcher in the chain, may be null 
        /// </summary>

        protected internal HttpFetcher nextFetcher;

        protected internal ChainedContentFetcher(HttpFetcher nextFetcher_0)
        {
            this.nextFetcher = nextFetcher_0;
        }

        /// <summary>
        /// from org.apache.shindig.gadgets.http.HttpFetcher
        /// </summary>
        ///
        public abstract sResponse fetch(sRequest request);
    } 
}
