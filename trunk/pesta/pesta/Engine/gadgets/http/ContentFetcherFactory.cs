using Pesta.Engine.gadgets.oauth;


namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// Summary description for ContentFetcherFactory
    /// </summary>
    public class ContentFetcherFactory
    {
        private RemoteContentFetcherFactory remoteContentFetcherFactory;
        private OAuthFetcherFactory oauthFetcherFactory;
        public readonly static ContentFetcherFactory Instance = new ContentFetcherFactory();
        protected ContentFetcherFactory()
        {
            this.remoteContentFetcherFactory = RemoteContentFetcherFactory.Instance;
            this.oauthFetcherFactory = OAuthFetcherFactory.Instance;
        }

        public sResponse fetch(sRequest request)
        {
            if (request.AuthType == AuthType.NONE)
            {
                return remoteContentFetcherFactory.get().fetch(request);
            }
            else if (request.AuthType == AuthType.OAUTH || request.AuthType == AuthType.SIGNED)
            {
                return oauthFetcherFactory.getOAuthFetcher(remoteContentFetcherFactory.get(), request).fetch(request);
            }
            else
            {
                return null;
            }
        }
    }
}