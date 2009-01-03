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
using System.Text;
using System.Security.Cryptography;

namespace Pesta.Engine.common.util
{
    /// <summary> Routines for producing hashes.</summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class HashUtil
    {
        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string checksum(string data)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(data);

            return checksum(b);
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string checksum(byte[] data)
        {
            // This is one implementation of the abstract class MD5.
            MD5 md5 = new MD5CryptoServiceProvider();

            return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", String.Empty);
        }

        /**
           * Produces a raw checksum for the given input data.
           *
           * @param data
           * @return The checksum.
           */
        public static String rawChecksum(byte[] data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return Encoding.Default.GetString(md5.ComputeHash(data));
        }
    }
}