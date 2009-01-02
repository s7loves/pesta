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

namespace Pesta.Engine.common.crypto
{
    /// <summary>
    /// Utility interface for managing signed, encrypted, and time stamped blobs.
    /// Blobs are made up of name/value pairs. Time stamps are automatically included
    /// and checked.
    /// Thread safe.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public interface BlobCrypter
    {

        /// <summary>
        /// Time stamps, encrypts, and signs a blob.
        /// </summary>
        ///
        /// <param name="in">name/value pairs to encrypt</param>
        /// <returns>a base64 encoded blob</returns>
        /// @throws BlobCrypterException
        String wrap(Dictionary<string, string> ins0);

        /// <summary>
        /// Unwraps a blob.
        /// </summary>
        ///
        /// <param name="in">blob</param>
        /// <param name="maxAgeSec">maximum age for the blob</param>
        /// <returns>the name/value pairs, including the origin timestamp.</returns>
        /// @throws BlobExpiredExceptionif the blob is too old to be accepted.
        /// @throws BlobCrypterExceptionif the blob can't be decoded.
        Dictionary<string, string> unwrap(String ins0, int maxAgeSec);

    }
}