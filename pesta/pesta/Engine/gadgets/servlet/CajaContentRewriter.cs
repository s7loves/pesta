using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using org.apache.shindig.gadgets.rewrite;
using URI = System.Uri;
using com.google.caja.opensocial;
using com.google.caja.reporting;
using com.google.caja.lexer;
using Reader = java.io.Reader;

namespace Pesta
{
    public class CajaContentRewriter : ContentRewriter
    {
        private class UriCallback2 : UriCallback
        {
            readonly Uri retrievedUri;
            /// <summary>
            /// Initializes a new instance of the UriCallback2 class.
            /// </summary>
            public UriCallback2(URI retrievedUri)
            {
                this.retrievedUri = Uri.fromJavaUri(retrievedUri);
            }
            public UriCallbackOption getOption(ExternalReference externalReference, String _string)
            {
                return UriCallbackOption.REWRITE;
            }

            public Reader retrieve(ExternalReference externalReference, String _string)
            {
                try
                {
                    Reader _in = new java.io.InputStreamReader(externalReference.getUri().toURL().openConnection().getInputStream(), "UTF-8");
                    char[] buf = new char[4096];
                    StringBuilder sb = new StringBuilder();
                    for (int n; (n = _in.read(buf)) > 0; )
                    {
                        sb.Append(buf, 0, n);
                    }
                    return new java.io.StringReader(sb.ToString());
                }
                catch (java.net.MalformedURLException ex)
                {
                    throw new UriCallbackException(externalReference, ex);
                }
                catch (IOException ex)
                {
                    throw new UriCallbackException(externalReference, ex);
                }
            }

            public java.net.URI rewrite(ExternalReference externalReference, String _string)
            {
                return new java.net.URI(retrievedUri.resolve(Uri.parse(externalReference.getUri().toString())).ToString());
            }
        }
        public RewriterResults rewrite(sRequest req, sResponse resp, MutableContent content)
        {
            return RewriterResults.cacheableIndefinitely();
        }

        public RewriterResults rewrite(Gadget gadget, MutableContent content)
        {
            if (gadget.getSpec().getModulePrefs().getFeatures().ContainsKey("caja") ||
                        "1".Equals(gadget.getContext().getParameter("caja"))) 
            {
                URI retrievedUri = gadget.getContext().getUrl();
                UriCallback2 cb = new UriCallback2(retrievedUri);

                MessageQueue mq = new SimpleMessageQueue();
                DefaultGadgetRewriter rw = new DefaultGadgetRewriter(mq);
                CharProducer input = CharProducer.Factory.create(
                                    new java.io.StringReader(content.getContent()),
                                    FilePosition.instance(new InputSource(new java.net.URI(retrievedUri.ToString())), 2, 2, 1, 1));
                java.lang.StringBuilder output = new java.lang.StringBuilder();

                try 
                {
                    rw.rewriteContent(new java.net.URI(retrievedUri.ToString()), input, cb, output);
                }
                catch (GadgetRewriteException e)
                {
                    throwCajolingException(e, mq);
                    return RewriterResults.notCacheable();
                }
                catch (IOException e) 
                {
                    throwCajolingException(e, mq);
                    return RewriterResults.notCacheable();
                }      
                content.setContent(output.ToString());
            }
            return RewriterResults.notCacheable();
        }

        private void throwCajolingException(Exception cause, MessageQueue mq) 
        {
            StringBuilder errbuilder = new StringBuilder();
            MessageContext mc = new MessageContext();

            if (cause != null)
            {
                errbuilder.Append(cause).Append('\n');
            }

            for (java.util.Iterator iter = mq.getMessages().iterator(); iter.hasNext(); )
            {
                com.google.caja.reporting.Message m = iter.next() as com.google.caja.reporting.Message;
                errbuilder.Append(m.format(mc)).Append('\n');
            }
        }
    }
}
