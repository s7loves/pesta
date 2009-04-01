/* Copyright (c) 2008 Google Inc.
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

/**
 * A utility object containing several static methods for signing requests in
 * accordance with the OAuth specification -- methods generate a signature,
 * and append it and other required parameters to the URL associated with
 * the OpenSocialHttpRequest argument. The signatures are then validated
 * by the container to verify that they were submitted from a trusted
 * application.
 *
 * @author Jason Cooper
 */
using System;
using System.Collections.Generic;
using Pesta.Interop.oauth;

public class OpenSocialRequestSigner {

  /**
   * Extracts pertinent properties from passed OpenSocialClient object and
   * passes these along with OpenSocialHttpRequest object to a separate
   * method which does the actual signing.
   * 
   * @param  request OpenSocialHttpRequest object which contains both the URL
   *         to sign as well as the POST body which must be included as a
   *         parameter when signing POST requests
   * @param  client OpenSocialClient object with various properties, both
   *         optional and required, used during the signing process
   * @throws OAuthException
   * @throws IOException
   * @throws URISyntaxException
   */
  public static void signRequest(
      OpenSocialHttpRequest request, OpenSocialClient client)
    {

    String token =
      client.getProperty(OpenSocialClient.Properties.TOKEN);
    String viewerId =
      client.getProperty(OpenSocialClient.Properties.VIEWER_ID);
    String consumerKey =
      client.getProperty(OpenSocialClient.Properties.CONSUMER_KEY);
    String consumerSecret =
      client.getProperty(OpenSocialClient.Properties.CONSUMER_SECRET);

    signRequest(request, token, viewerId, consumerKey, consumerSecret);
  }

  /**
   * Adds optional query string parameters to request URL if present, then
   * passes the passed OpenSocialHttpRequest to a separate method which
   * does the actual signing.
   * 
   * @param  request OpenSocialHttpRequest object which contains both the URL
   *         to sign as well as the POST body which must be included as a
   *         parameter when signing POST requests
   * @param  token Security token which can be borrowed from a running gadget
   *         and appended to the URL as a query string parameter instead of
   *         signing the request. These types of tokens are typically short-
   *         lived and must be refreshed often, making signing much more
   *         practical.
   * @param  viewerId ID of the user currently using the application (or for
   *         whom the application is making requests on behalf of). The ID
   *         is appended to the URL as a query string parameter so it can
   *         be signed with the rest of the URL.
   * @param  consumerKey Application key assigned and used by containers to
   *         uniquely identify applications
   * @param  consumerSecret Secret key shared between application owner and
   *         container. Used to generate the signature which is attached to
   *         the request so containers can verify the authenticity of the
   *         requests made by the client application.
   * @throws OAuthException
   * @throws IOException
   * @throws URISyntaxException
   */
  public static void signRequest(
      OpenSocialHttpRequest request, String token, String viewerId,
      String consumerKey, String consumerSecret)
    {

    OpenSocialUrl requestUrl = request.getUrl();

    if (!String.IsNullOrEmpty(viewerId))
    {
      requestUrl.addQueryStringParameter("xoauth_requestor_id", viewerId);
    }
    if (!String.IsNullOrEmpty(token))
    {
      requestUrl.addQueryStringParameter("st", token);
    }

    signRequest(request, consumerKey, consumerSecret);
  }

  /**
   * Signs the URL associated with the passed request object using the passed
   * consumer key and secret in accordance with the OAuth specification and
   * appends signature and other required parameters to the URL as query
   * string parameters.
   * 
   * @param  request OpenSocialHttpRequest object which contains both the URL
   *         to sign as well as the POST body which must be included as a
   *         parameter when signing POST requests
   * @param  consumerKey Application key assigned and used by containers to
   *         uniquely identify applications
   * @param  consumerSecret Secret key shared between application owner and
   *         container. Used to generate the signature which is attached to
   *         the request so containers can verify the authenticity of the
   *         requests made by the client application.
   * @throws OAuthException
   * @throws IOException
   * @throws URISyntaxException
   */
  public static void signRequest(
      OpenSocialHttpRequest request, String consumerKey, String consumerSecret)
    {

    String postBody = request.getPostBody();
    String requestMethod = request.getMethod();
    OpenSocialUrl requestUrl = request.getUrl();

    if (!String.IsNullOrEmpty(consumerKey) && !String.IsNullOrEmpty(consumerSecret))
    {
      OAuthMessage message =
          new OAuthMessage(requestMethod, requestUrl.toString(), null);

      if (!String.IsNullOrEmpty(postBody))
      {
        message.addParameter(postBody, "");        
      }

      OAuthConsumer consumer =
          new OAuthConsumer(null, consumerKey, consumerSecret, null);
      consumer.setProperty(OAuth.OAUTH_SIGNATURE_METHOD, OAuth.HMAC_SHA1);

      OAuthAccessor accessor = new OAuthAccessor(consumer);
      accessor.accessToken = "";      

      message.addRequiredParameters(accessor);

      foreach(var p in message.getParameters()) {
        if (!p.Key.Equals(postBody)) {
          requestUrl.addQueryStringParameter(
              OAuth.percentEncode(new List<string> { p.Key }),
              OAuth.percentEncode(new List<string> {p.Value}));          
        }
      }
    }
  }
}
