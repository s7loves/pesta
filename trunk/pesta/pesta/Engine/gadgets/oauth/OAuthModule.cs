using System;
using System.IO;
using Pesta.Engine.common;
using Pesta.Engine.common.crypto;

namespace Pesta.Engine.gadgets.oauth
{
    public class OAuthModule
    {
        private static readonly String OAUTH_CONFIG = "config/oauth.json";
        private static readonly String OAUTH_SIGNING_KEY_FILE = "shindig.signing.key-file";
        private static readonly String OAUTH_SIGNING_KEY_NAME = "shindig.signing.key-name";

        public class OAuthCrypterProvider 
        {
            private readonly BlobCrypter crypter;

            public OAuthCrypterProvider(String stateCrypterPath)
            {
                if (String.IsNullOrEmpty(stateCrypterPath)) 
                {
                    crypter = new BasicBlobCrypter(Crypto.getRandomBytes(BasicBlobCrypter.MASTER_KEY_MIN_LEN));
                } 
                else 
                {
                    crypter = new BasicBlobCrypter(stateCrypterPath);
                }
            }

            public BlobCrypter get() 
            {
                return crypter;
            }
        }
    }
}
