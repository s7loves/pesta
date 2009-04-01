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
 * An object which represents a simple URL. Once an object is instantiated,
 * path components and parameters can be added. Once all of the elements
 * are in place, the object can be serialized into a string. This class
 * is used by internal classes and not by clients directly.
 *
 * @author Jason Cooper
 */
using System;
using System.Collections.Generic;
using System.Text;

public class OpenSocialUrl {

  private readonly String _base;
  private readonly List<String> components;
  private readonly Dictionary<String, String> queryStringParameters;

  public OpenSocialUrl(String _base) {
    this._base = _base;
    this.components = new List<String>();
    this.queryStringParameters = new Dictionary<String, String>();
  }

  /**
   * Adds passed String to the path component queue.
   * 
   * @param  component Path component to Add
   */
  public void addPathComponent(String component) {
    components.Add(component);
  }

  /**
   * Creates a new entry in queryStringParameters Map with the passed key and
   * value; used for adding URL parameters such as oauth_signature and the
   * various other OAuth parameters that are required in order to submit a
   * signed request.
   * 
   * @param  key Parameter name
   * @param  value Parameter value
   */
  public void addQueryStringParameter(String key, String value) {
    queryStringParameters.Add(key, value);
  }

  /**
   * Returns a String representing the serialized URL including the _base
   * followed by any path components added to the path component queue
   * and, last but not least, appending any query string parameters as
   * name-value pairs after the full path.
   */
  public String toString() {
    StringBuilder s = new StringBuilder(this._base);

    foreach(String pathComponent in this.components) {
      if (s[s.Length - 1] != '/') {
        s.Append("/");
      }
      s.Append(pathComponent);
    }

    String connector = "?";
    foreach(var e in this.queryStringParameters) {
      s.Append(connector);
      s.Append(e.Key);
      s.Append("=");
      s.Append(e.Value);
      connector = "&";
    }

    return s.ToString();
  }
}
