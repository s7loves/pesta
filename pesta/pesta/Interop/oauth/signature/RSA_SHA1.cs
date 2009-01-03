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
using java.security;
using java.security.cert;
using java.security.spec;

using System.Text;

namespace Pesta.Interop.oauth.signature
{
    /// <summary>
    /// Summary description for RSA_SHA1
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class RSA_SHA1 : OAuthSignatureMethod
    {
        readonly static public String PRIVATE_KEY = "RSA-SHA1.PrivateKey";
        readonly static public String PUBLIC_KEY = "RSA-SHA1.PublicKey";
        readonly static public String X509_CERTIFICATE = "RSA-SHA1.X509Certificate";

        private PrivateKey privateKey = null;
        private PublicKey publicKey = null;

        protected override void initialize(String name, OAuthAccessor accessor)
        {
            base.initialize(name, accessor);

            Object privateKeyObject = accessor.consumer.getProperty(PRIVATE_KEY);
            try
            {
                if (privateKeyObject != null)
                {
                    if (privateKeyObject is PrivateKey)
                    {
                        privateKey = (PrivateKey)privateKeyObject;
                    }
                    else if (privateKeyObject is String)
                    {
                        privateKey = getPrivateKeyFromPem((String)privateKeyObject);
                    }
                    else if (privateKeyObject is byte[])
                    {
                        privateKey = getPrivateKeyFromDer((byte[])privateKeyObject);
                    }
                    else
                    {
                        throw new ArgumentException(
                                "Private key set through RSA_SHA1.PRIVATE_KEY must be of " +
                                "type PrivateKey, String, or byte[], and not " +
                                privateKeyObject.GetType().Name);
                    }
                }

                Object publicKeyObject = accessor.consumer.getProperty(PUBLIC_KEY);
                if (publicKeyObject != null)
                {
                    if (publicKeyObject is PublicKey)
                    {
                        publicKey = (PublicKey)publicKeyObject;
                    }
                    else if (publicKeyObject is String)
                    {
                        publicKey = getPublicKeyFromPem((String)publicKeyObject);
                    }
                    else if (publicKeyObject is byte[])
                    {
                        publicKey = getPublicKeyFromDer((byte[])publicKeyObject);
                    }
                    else
                    {
                        throw new ArgumentException(
                                "Public key set through RSA_SHA1.PRIVATE_KEY must be of " +
                                "type PublicKey, String, or byte[], and not " +
                                publicKeyObject.GetType().Name);
                    }
                }
                else
                {  // public key was null. perhaps they gave us a X509 cert.
                    Object certObject = accessor.consumer.getProperty(X509_CERTIFICATE);
                    if (certObject != null)
                    {
                        if (certObject is X509Certificate)
                        {
                            publicKey = ((X509Certificate)certObject).getPublicKey();
                        }
                        else if (certObject is String)
                        {
                            publicKey = getPublicKeyFromPemCert((String)certObject);
                        }
                        else if (certObject is byte[])
                        {
                            publicKey = getPublicKeyFromDerCert((byte[])certObject);
                        }
                        else
                        {
                            throw new ArgumentException(
                                    "X509Certificate set through RSA_SHA1.X509_CERTIFICATE" +
                                    " must be of type X509Certificate, String, or byte[]," +
                                    " and not " + certObject.GetType().Name);
                        }
                    }
                }
            }
            catch (GeneralSecurityException e)
            {
                throw new OAuthException(e);
            }
        }

        private PublicKey getPublicKeyFromPemCert(String certObject)
        {
            CertificateFactory fac = CertificateFactory.getInstance("X509");
            java.io.ByteArrayInputStream input = new java.io.ByteArrayInputStream(Encoding.Default.GetBytes(certObject));
            X509Certificate cert = (X509Certificate)fac.generateCertificate(input);
            return cert.getPublicKey();
        }

        private PublicKey getPublicKeyFromDerCert(byte[] certObject)
        {
            CertificateFactory fac = CertificateFactory.getInstance("X509");
            java.io.ByteArrayInputStream input = new java.io.ByteArrayInputStream(certObject);
            X509Certificate cert = (X509Certificate)fac.generateCertificate(input);
            return cert.getPublicKey();
        }

        private PublicKey getPublicKeyFromDer(byte[] publicKeyObject)
        {
            KeyFactory fac = KeyFactory.getInstance("RSA");
            EncodedKeySpec pubKeySpec = new X509EncodedKeySpec(publicKeyObject);
            return fac.generatePublic(pubKeySpec);
        }

        private PublicKey getPublicKeyFromPem(String publicKeyObject)
        {
            return getPublicKeyFromDer(decodeBase64(publicKeyObject));
        }

        private PrivateKey getPrivateKeyFromDer(byte[] privateKeyObject)
        {
            KeyFactory fac = KeyFactory.getInstance("RSA");
            EncodedKeySpec privKeySpec = new PKCS8EncodedKeySpec(privateKeyObject);
            return fac.generatePrivate(privKeySpec);
        }

        private PrivateKey getPrivateKeyFromPem(String privateKeyObject)
        {
            return getPrivateKeyFromDer(decodeBase64(privateKeyObject));
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
            if (privateKey == null)
            {
                throw new Exception("need to set private key with " +
                                                "OAuthConsumer.setProperty when " +
                                                "generating RSA-SHA1 signatures.");
            }
            Signature signer = Signature.getInstance("SHA1withRSA");
            signer.initSign(privateKey);
            signer.update(message);
            return signer.sign();
        }

        private bool verify(byte[] signature, byte[] message)
        {
            if (publicKey == null)
            {
                throw new Exception("need to set public key with " +
                                                " OAuthConsumer.setProperty when " +
                                                "verifying RSA-SHA1 signatures.");
            }
            Signature verifier = Signature.getInstance("SHA1withRSA");
            verifier.initVerify(publicKey);
            verifier.update(message);
            return verifier.verify(signature);
        }
    } 
}
