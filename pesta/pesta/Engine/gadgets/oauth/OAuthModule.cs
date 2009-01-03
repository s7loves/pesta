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

        public class OAuthStoreProvider
        {
            private BasicOAuthStore store;

            public OAuthStoreProvider(String signingKeyFile,String signingKeyName)
            {
                store = BasicOAuthStore.Instance;
                loadDefaultKey(signingKeyFile, signingKeyName);
                loadConsumers();
            }

            private void loadDefaultKey(String signingKeyFile, String signingKeyName) 
            {
                BasicOAuthStoreConsumerKeyAndSecret key = null;
                if (!String.IsNullOrEmpty(signingKeyFile))
                {
                    using (StreamReader reader = new StreamReader(ResourceLoader.open(signingKeyFile)))
                    {
                        String privateKey = reader.ReadToEnd();
                        privateKey = BasicOAuthStore.convertFromOpenSsl(privateKey);
                        key = new BasicOAuthStoreConsumerKeyAndSecret(null, privateKey, 
                                    BasicOAuthStoreConsumerKeyAndSecret.KeyType.RSA_PRIVATE,
                                    signingKeyName);
                    }
                }
                if (key != null)
                {
                    store.setDefaultKey(key);
                } 
                else 
                {
                
                }
            }

            private void loadConsumers()
            {
                String oauthConfigString = ResourceLoader.getContent(OAUTH_CONFIG);
                store.initFromConfigString(oauthConfigString);
            }

            public OAuthStore get() 
            {
                return store;
            }
        }
    }
}
