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
using System.IO;
using System.Security.Cryptography;
using System;
using System.Text;

namespace Pesta
{
    /// <summary>
    /// Cryptographic utility functions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class Crypto
    {
        /// <summary>
        /// HMAC algorithm to use
        /// </summary>
        ///
        private const String HMAC_TYPE = "HMACSHA1";

        /// <summary>
        /// minimum safe length for hmac keys (this is good practice, but not
        /// actually a requirement of the algorithm
        /// </summary>
        ///
        private const int MIN_HMAC_KEY_LEN = 8;

        /// <summary>
        /// Encryption algorithm to use
        /// </summary>
        ///
        private const String CIPHER_TYPE = "AES/CBC/PKCS5Padding";

        private const String CIPHER_KEY_TYPE = "AES";

        /// <summary>
        /// Use keys of this length for encryption operations
        /// </summary>
        ///
        public const int CIPHER_KEY_LEN = 16;

        private static int CIPHER_BLOCK_SIZE = 16;

        /// <summary>
        /// Length of HMAC SHA1 output
        /// </summary>
        ///
        public const int HMAC_SHA1_LEN = 20;

        public static Random rand = new Random();

        // everything is static, no instantiating this class
        private Crypto()
        {
        }

        /// <summary>
        /// Gets a hex encoded random string.
        /// </summary>
        ///
        /// <param name="numBytes">number of bytes of randomness.</param>
        public static String GetRandomString(int numBytes)
        {
            return String.Format("0:X", GetRandomBytes(numBytes));
        }

        /// <summary>
        /// Returns strong random bytes.
        /// </summary>
        ///
        /// <param name="numBytes">number of bytes of randomness</param>
        public static byte[] GetRandomBytes(int numBytes)
        {
            byte[] xout = new byte[numBytes];
            rand.NextBytes(xout);
            return xout;
        }

        /// <summary>
        /// HMAC sha1
        /// </summary>
        ///
        /// <param name="key">the key must be at least 8 bytes in length.</param>
        /// <param name="in">byte array to HMAC.</param>
        /// <returns>the hash</returns>
        /// @throws GeneralSecurityException
        public static byte[] HmacSha1(byte[] key, byte[] ins0)
        {
            if (key.Length < MIN_HMAC_KEY_LEN)
            {
                throw new Exception("HMAC key should be at least "
                        + MIN_HMAC_KEY_LEN + " bytes.");
            }
            HMACSHA1 hmac = new HMACSHA1(key);
            hmac.Initialize();
            return hmac.ComputeHash(ins0);
        }

        /// <summary>
        /// Verifies an HMAC SHA1 hash. Throws if the verification fails.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="in"></param>
        /// <param name="expected"></param>
        /// @throws GeneralSecurityException
        public static void HmacSha1Verify(byte[] key, byte[] ins0, byte[] expected)
        {

            HMACSHA1 hmac = new HMACSHA1(key);
            byte[] actual = hmac.ComputeHash(ins0);
            if (actual.Length != expected.Length)
            {
                throw new Exception("HMAC verification failure");
            }
            if (!Array.Equals(actual, expected))
            {
                throw new Exception("HMAC verification failure");
            }
        }

        /// <summary>
        /// AES-128-CBC encryption. The IV is returned as the first 16 bytes of the
        /// cipher text.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="plain"></param>
        /// <returns>the IV and cipher text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcEncrypt(byte[] key, byte[] plain)
        {
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.KeySize = 128;
            byte[] iv = symmetricKey.IV;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            cryptoStream.Write(plain, 0, plain.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherText = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Concat(iv, cipherText);
        }

        /// <summary>
        /// AES-128-CBC decryption. The IV is assumed to be the first 16 bytes of the
        /// cipher text.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="cipherText"></param>
        /// <returns>the plain text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcDecrypt(byte[] key, byte[] cipherText)
        {
            byte[] iv = new byte[CIPHER_BLOCK_SIZE];
            System.Array.Copy(cipherText, 0, iv, 0, iv.Length);
            return Aes128cbcDecryptWithIv(key, iv, cipherText, iv.Length);
        }

        /// <summary>
        /// AES-128-CBC decryption with a particular IV.
        /// </summary>
        ///
        /// <param name="key">decryption key</param>
        /// <param name="iv">initial vector for decryption</param>
        /// <param name="cipherText">cipher text to decrypt</param>
        /// <param name="offset">offset into cipher text to begin decryption</param>
        /// <returns>the plain text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcDecryptWithIv(byte[] key, byte[] iv, byte[] cipherText, int offset)
        {
            AesCryptoServiceProvider cipher = new AesCryptoServiceProvider();
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         decryptor,
                                                         CryptoStreamMode.Read);
            byte[] plain = new byte[cipherText.Length];
            int decryptedLength = cryptoStream.Read(plain, 0, plain.Length);
            memoryStream.Close();
            cryptoStream.Close();
            byte[] decrypted = new byte[decryptedLength];
            Array.Copy(plain, decrypted, decryptedLength);
            return decrypted;
        }

        /// <summary>
        /// Concatenate two byte arrays.
        /// </summary>
        ///
        public static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] xout = new byte[a.Length + b.Length];
            int cursor = 0;
            Array.Copy(a, 0, xout, cursor, a.Length);
            cursor += a.Length;
            Array.Copy(b, 0, xout, cursor, b.Length);
            return xout;
        }
    } 
}
