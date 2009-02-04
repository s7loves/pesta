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

using org.apache.shindig.gadgets.rewrite;
using pestaServer.Models.gadgets.http;

namespace pestaServer.Models.gadgets.rewrite
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface IContentRewriter
    {
        /**
        * Rewrite the original content located at source.
        * 
        * @param request Originating request, as context.
        * @param response Original HTTP response, for context.
        * @param content Original content.
        */
        RewriterResults rewrite(sRequest request, sResponse original, MutableContent content);

        /**
        * Rewrite the gadget. The Gadget object's manipulation methods are used
        * for the bulk of this.
        * 
        * @param gadget Gadget to rewrite.
        */
        RewriterResults rewrite(Gadget gadget, MutableContent content);
    }
}