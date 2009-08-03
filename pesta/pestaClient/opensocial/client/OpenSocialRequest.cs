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
 * An object which represents a single OpenSocial REST/JSON-RPC request, which
 * are collected and submitted using one or more OpenSocialBatch instances.
 * Instances should not be created directly but should instead be returned
 * from static OpenSocialClient methods which take care to set all
 * properties appropriately given the request type.
 *
 * @author Jason Cooper
 */
using System;
using System.Collections.Generic;
using Jayrock.Json;

public class OpenSocialRequest {

  private String id;
  private String rpcMethodName;
  private String restPathComponent;
  private Dictionary<String, String> parameters;

  public OpenSocialRequest(String pathComponent, String methodName) {
    this.parameters = new Dictionary<String, String>();

    this.restPathComponent = pathComponent;
    this.rpcMethodName = methodName;
    this.id = null;
  }

  /**
   * Returns instance variable: id.
   */
  public String getId() {
    return this.id;
  }

  /** 
   * Sets instance variable id to passed String.
   */
  public void setId(String id) {
    this.id = id;
  }

  /**
   * Creates a new entry in parameters Map with the passed key and value;
   * used for setting request-specific parameters such as appId, userId,
   * and groupId.
   */
  public void addParameter(String key, String value) {
    this.parameters.Add(key, value);
  }

  /**
   * Returns the value of the parameter with the given name or null if
   * no parameter with that name exists.
   */
  public String getParameter(String parameter) {
      if (!parameters.ContainsKey(parameter))
      {
          return null;
      }
    return this.parameters[parameter];
  }

  /**
   * Returns instance variable: restPathComponent. If path component does not
   * have a trailing backslash, one is added before being returned.
   */
  public String getRestPathComponent() {
    return this.restPathComponent;
  }

  /**
   * Returns a JSON-RPC serialization of the request including ID, RPC method,
   * and all added parameters. Used by other classes when preparing to submit
   * an RPC batch request.
   * 
   * @throws JSONException
   */
  public String getJsonEncoding() 
  {
    JsonObject o = new JsonObject();

    if (this.id != null) {
      o.Put("id", this.id);      
    }

    o.Put("method", this.rpcMethodName);
    o.Put("params", new JsonObject(this.parameters));

    return o.ToString();
  }
}
