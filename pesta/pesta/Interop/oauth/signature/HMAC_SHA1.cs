using System;
using System.Text;
using java.security;
using javax.crypto;

using javax.crypto.spec;

namespace Pesta.Interop.oauth.signature
{
    /// <summary>
    /// Summary description for HMAC_SHA1
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class HMAC_SHA1 : OAuthSignatureMethod
    {
        protected override String getSignature(String baseString)
        {
            try
            {
                String signature = base64Encode(computeSignature(baseString));
                return signature;
            }
            catch (GeneralSecurityException e)
            {
                throw new OAuthException(e);
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
                byte[] expected = computeSignature(baseString);
                byte[] actual = decodeBase64(signature);
                return Array.Equals(expected, actual);
            }
            catch (GeneralSecurityException e)
            {
                throw new OAuthException(e);
            }
            catch (Exception e)
            {
                throw new OAuthException(e);
            }
        }

        private byte[] computeSignature(String baseString)
        {
            SecretKey key = null;
            lock (this)
            {
                if (this.key == null)
                {
                    String keyString = OAuth.percentEncode(getConsumerSecret())
                                       + '&' + OAuth.percentEncode(getTokenSecret());
                    byte[] keyBytes = Encoding.GetEncoding(ENCODING).GetBytes(keyString);
                    this.key = new SecretKeySpec(keyBytes, MAC_NAME);
                }
                key = this.key;
            }
            Mac mac = Mac.getInstance(MAC_NAME);
            mac.init(key);
            byte[] text = Encoding.GetEncoding(ENCODING).GetBytes(baseString);
            return mac.doFinal(text);
        }

        /** ISO-8859-1 or US-ASCII would work, too. */
        private static readonly String ENCODING = OAuth.ENCODING;

        private static readonly String MAC_NAME = "HmacSHA1";

        private SecretKey key = null;

        public override void setConsumerSecret(String consumerSecret)
        {
            lock (this)
            {
                key = null;
            }
            base.setConsumerSecret(consumerSecret);
        }

        public override void setTokenSecret(String tokenSecret)
        {
            lock (this)
            {
                key = null;
            }
            base.setTokenSecret(tokenSecret);
        }
    }
}