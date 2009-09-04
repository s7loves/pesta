/*
 * Copyright 2007 Netflix, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using System.Text;

namespace Pesta.Libraries.OAuth.signature
{
    public class RSA_SHA1 : OAuthSignatureMethod
    {
        readonly static public String PRIVATE_KEY = "RSA-SHA1.PrivateKey";
        readonly static public String PUBLIC_KEY = "RSA-SHA1.PublicKey";
        readonly static public String X509_CERTIFICATE = "RSA-SHA1.X509Certificate";
        readonly static public String X509_CERTIFICATE_PASS = "RSA-SHA1.X509Certificate.Pass";

        private X509Certificate2 certificate;

        protected override void initialize(string name, OAuthAccessor accessor)
        {
            base.initialize(name, accessor);
            // get pass
            string pass = accessor.consumer.getProperty(X509_CERTIFICATE_PASS) as string;
            // get certificate location
            string certloc = accessor.consumer.getProperty(X509_CERTIFICATE) as string;

            certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + certloc, pass);
        } 

        protected override String getSignature(String baseString)
        {
            try
            {
                byte[] signature = sign(Encoding.GetEncoding(OAuth.ENCODING).GetBytes(baseString));
                return base64Encode(signature);
            }
            catch (Exception e)
            {
                throw new OAuthException(e);
            }
        }

        protected override bool isValid(String signature, String baseString)
        {
            try
            {
                return verify(decodeBase64(signature), Encoding.GetEncoding(OAuth.ENCODING).GetBytes(baseString));
            }
            catch (Exception e)
            {
                throw new OAuthException(e);
            }
        }

        private byte[] sign(byte[] message)
        {
            if (certificate.PrivateKey == null)
            {
                throw new Exception("a private key is required when generating RSA-SHA1 signatures.");
            }
            using (HashAlgorithm hasher = HashAlgorithm.Create("SHA1"))
            {
                RSAPKCS1SignatureFormatter signatureFormatter = new RSAPKCS1SignatureFormatter();
                signatureFormatter.SetKey(certificate.PrivateKey);
                signatureFormatter.SetHashAlgorithm("SHA1");
                byte[] hash = hasher.ComputeHash(message);
                return signatureFormatter.CreateSignature(hash);
            }
        }

        private bool verify(byte[] signature, byte[] message)
        {
            if (certificate.PublicKey == null)
            {
                throw new Exception("a public key is required " +
                                    " OAuthConsumer.setProperty when " +
                                    "verifying RSA-SHA1 signatures.");
            }

            using (HashAlgorithm hasher = HashAlgorithm.Create("SHA1"))
            {
                RSAPKCS1SignatureDeformatter signatureDeformatter = new RSAPKCS1SignatureDeformatter(certificate.PrivateKey);
                signatureDeformatter.SetKey(certificate.PublicKey.Key);
                signatureDeformatter.SetHashAlgorithm("SHA1");
                byte[] hash = hasher.ComputeHash(message);
                return signatureDeformatter.VerifySignature(hash, signature);
            }
        }
    }
}