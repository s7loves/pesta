using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pesta.Interop.oauth.signature
{
    /// <summary>
    /// Summary description for HMAC_SHA1
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
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
                return Enumerable.SequenceEqual(expected, actual);
            }
            catch (Exception e)
            {
                throw new OAuthException(e);
            }
        }

        private byte[] computeSignature(String baseString)
        {
            byte[] _key;
            lock (this)
            {
                if (key == null)
                {
                    String keyString = Rfc3986.Encode(getConsumerSecret())
                                         + "&" + Rfc3986.Encode(getTokenSecret());
                    key = Encoding.GetEncoding(ENCODING).GetBytes(keyString);
                }
                _key = key;
            }
            using (HMACSHA1 crypto = new HMACSHA1())
            {
                crypto.Key = _key;
                byte[] hash = crypto.ComputeHash(Encoding.GetEncoding(ENCODING).GetBytes(baseString));
                crypto.Clear();
                return hash;
            }
        }

        /** ISO-8859-1 or US-ASCII would work, too. */
        private static readonly String ENCODING = OAuth.ENCODING;

        private byte[] key;

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