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
 * A utility object containing static methods for verifying the signature of
 * requests signed by OpenSocial containers. All incoming requests should be
 * verified in case a malicious third party attempts to submit fraudulent
 * requests for user information.
 * 
 * @author Jason Cooper
 */
using System;
using System.Collections.Specialized;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Pesta.Libraries.OAuth;

public class OpenSocialRequestValidator {

  /**
   * Validates the passed request by reconstructing the original URL and
   * parameters and generating a signature following the OAuth HMAC-SHA1
   * specification and using the passed secret key.
   * 
   * @param  request Servlet request containing required information for
   *         reconstructing the signature such as the request's URL
   *         components and parameters
   * @param  consumerSecret Secret key shared between application owner and
   *         container. Used by containers when issuing signed makeRequests
   *         and by client applications to verify the source of these
   *         requests and the authenticity of its parameters.
   * @return {@code true} if the signature generated in this function matches
   *         the signature in the passed request, {@code false} otherwise
   * @throws IOException
   * @throws URISyntaxException
   */
  public static bool verifyHmacSignature(
      HttpWebRequest request, String consumerSecret)
    {

    String method = request.Method;
    String requestUrl = getRequestUrl(request);
    List<OAuth.Parameter> requestParameters = getRequestParameters(request);

    OAuthMessage message =
        new OAuthMessage(method, requestUrl, requestParameters);

    OAuthConsumer consumer =
        new OAuthConsumer(null, null, consumerSecret, null);
    OAuthAccessor accessor = new OAuthAccessor(consumer);

    try {
      message.validateMessage(accessor, new SimpleOAuthValidator());
    } catch (OAuthException e) {
      return false;
    }

    return true;
  }

  /**
   * Constructs and returns the full URL associated with the passed request
   * object.
   * 
   * @param  request Servlet request object with methods for retrieving the
   *         various components of the request URL
   */
  public static String getRequestUrl(HttpWebRequest request) {
    StringBuilder requestUrl = new StringBuilder();
    String scheme = request.RequestUri.Scheme;
    int port = request.RequestUri.Port;

    requestUrl.Append(scheme);
    requestUrl.Append("://");
    requestUrl.Append(request.RequestUri.Host);

    if ((scheme.Equals("http") && port != 80)
            || (scheme.Equals("https") && port != 443)) {
      requestUrl.Append(":");
      requestUrl.Append(port);
    }

    requestUrl.Append(request.RequestUri.AbsolutePath);
      // query string?

    return requestUrl.ToString();
  }

  /**
   * Constructs and returns a List of OAuth.Parameter objects, one per
   * parameter in the passed request.
   * 
   * @param  request Servlet request object with methods for retrieving the
   *         full set of parameters passed with the request
   */
  public static List<OAuth.Parameter> getRequestParameters(
      HttpWebRequest request) {

    List<OAuth.Parameter> parameters = new List<OAuth.Parameter>();

      for (int i = 0; i < request.Headers.Count; i++)
      {
          if (request.Headers.GetValues(i) == null) continue;
          foreach (String value in request.Headers.GetValues(i))
          {
              parameters.Add(new OAuth.Parameter(request.Headers.GetKey(i), value));
          }
      }

      var q = HttpUtility.ParseQueryString(request.RequestUri.Query);

      for (int i = 0; i < q.Count; i++)
      {
          if (q.GetValues(i) == null) continue;

          foreach (String value in q.GetValues(i))
          {
              parameters.Add(new OAuth.Parameter(q.GetKey(i), value));
          }
      }

      return parameters;
  }
}
