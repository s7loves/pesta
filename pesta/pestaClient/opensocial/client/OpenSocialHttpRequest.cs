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
 * An internal object which represents a single HTTP request and contains a
 * minimal set of methods for setting request properties, submitting the
 * request, and retrieving the response which can later be parsed into a
 * more meaningful object.
 *
 * @author Jason Cooper
 */
using System;
using System.IO;
using System.Net;
using System.Text;

public class OpenSocialHttpRequest {

  private String method;
  private String postBody;
  private OpenSocialUrl url;
  private HttpWebRequest connection;

  public OpenSocialHttpRequest(OpenSocialUrl url)
  {
    this.method = "GET";
    this.postBody = null;
    this.connection = null;    
    this.setUrl(url);
  }

  /**
   * Initializes connection if necessary before establishing the connection,
   * including writing POST data to the connection's output stream if
   * applicable.
   * 
   * @return HTTP status code of request, e.g. 200, 401, 500, etc.
   * @throws IOException
   */
  public int execute() 
  {
    if (this.connection == null) {
      this.connection = this.getConnection();
    }

    this.connection.Method = method;
    if (this.postBody != null) 
    {
      using (StreamWriter wr = new StreamWriter(this.connection.GetRequestStream()))
      {
          wr.Write(postBody);
          wr.Flush();
      }
    }
      HttpWebResponse resp = (HttpWebResponse)connection.GetResponse();
    return (int)resp.StatusCode;
  }

  /**
   * After executing the request, returns the server response as an InputStream
   * object.
   * 
   * @throws IOException
   */

    private Stream getResponseStream() 
  {
    return this.connection.GetResponse().GetResponseStream();
  }

  /**
   * After executing the request, transforms response output contained in the
   * connection's InputStream object into a string representation which can
   * later be parsed into a more meaningful object, e.g. OpenSocialObject. 
   *
   * @throws IOException if the open connection's input stream is not
   *         retrievable or accessible
   */
  public String getResponseString() 
  {
    Stream responseStream = getResponseStream();

    String sb;
    using (StreamReader reader = new StreamReader(responseStream))
    {
        sb = reader.ReadToEnd();
    }

    return sb;
  }

  /**
   * Returns instance variable: method.
   */
  public String getMethod() {
    return method;
  }

  /**
   * Returns instance variable: postBody.
   */
  public String getPostBody() {
    return postBody;
  }

  /**
   * Returns instance variable: url.
   */
  public OpenSocialUrl getUrl() {
    return url;
  }

  /**
   * Sets instance variable method to passed String.
   */
  public void setMethod(String method) {
    this.method = method;
  }

  /**
   * Sets instance variable postBody to passed String and automatically sets
   * request method to POST.
   */
  public void setPostBody(String postBody) {
    this.postBody = postBody;
    this.setMethod("POST");
  }

  /**
   * Sets instance variable url to passed OpenSocialUrl object.
   * 
   * @param requestUrl
   * @throws IOException
   */
  private void setUrl(OpenSocialUrl requestUrl) 
  {
    this.url = requestUrl;
  }

  /**
   * Opens a new HTTP connection for the URL associated with this object.
   * 
   * @return Opened connection
   * @throws IOException if URL protocol is not http
   */
  private HttpWebRequest getConnection() 
  {
    Uri uri = new Uri(this.url.toString());

    if (!uri.Scheme.StartsWith("http"))
    {
        throw new IOException("Unsupported scheme:" + uri.Scheme);
    }

    return (HttpWebRequest)WebRequest.Create(uri);
  }
}
