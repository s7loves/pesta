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

using System.Collections.ObjectModel;
using Pesta.Utilities;

namespace Pesta.Engine.gadgets
{
    /// <summary>
    /// Summary description for RenderingContext
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class RenderingContext : EnumBaseType<RenderingContext>
    {
        /// <summary>
        /// Initializes a new instance of the RenderingContext class.
        /// </summary>
        public RenderingContext(int key, string value)
            : base(key, value)
        {
        }
        // Used when rendering gadgets (iframes or inline).
        // TODO: rename this to "RENDER_GADGET"?
        public readonly static RenderingContext GADGET = new RenderingContext(1, "GADGET");

        // Used when rendering container data (not a gadget render)
        public readonly static RenderingContext CONTAINER = new RenderingContext(2, "CONTAINER");

        // Used when retrieving metadata about a gadget. Processing is generally
        // identical to processing under GADGET, but some operations may be safely
        // skipped, such as preload processing.
        public readonly static RenderingContext METADATA = new RenderingContext(3, "METADATA");

        public static ReadOnlyCollection<RenderingContext> GetValues()
        {
            return GetBaseValues();
        }

    }
}