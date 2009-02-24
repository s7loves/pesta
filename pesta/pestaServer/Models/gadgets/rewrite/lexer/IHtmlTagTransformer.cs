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
using com.google.caja.lexer;

namespace pestaServer.Models.gadgets.rewrite.lexer
{
    /// <summary>
    /// Summary description for IHtmlTagTransformer
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface IHtmlTagTransformer
    {
        /**
       * Consume the token. Prior token supplied for context
       * @param token current token
       * @param lastToken 
       */
        void accept(Token token,
                    Token lastToken);

        /**
         * A new tag has begun, should this transformer continue to process
         * @param tagStart
         * @return true if continuing to process the new tag
         */
        bool acceptNextTag(Token tagStart);

        /**
         * Close the transformer, reset its state and return any content
         * for inclusion in the rewritten output
         * @return transformed content
         */
        String close();
    }
}