/* Copyright (c) 2008 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except _in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to _in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



/**
 * An object which exposes a number of static methods for parsing JSON strings
 * returned from RESTful or JSON-RPC requests into appropriate objects.
 *
 * @author Jason Cooper
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jayrock.Json;
using Jayrock.Json.Conversion;

public class OpenSocialJsonParser {

  /**
   * Parses the passed JSON string into an OpenSocialResponse object -- if the
   * passed string represents a JSON array, each object _in the array is added
   * to the returned object keyed on its "id" property.
   * 
   * @param  _in The complete JSON string returned from an OpenSocial container
   *            _in response to a request for data
   * @throws JSONException
   */
  public static OpenSocialResponse getResponse(String _in)
     {

    OpenSocialResponse r = null;

    if (_in[0] == '[') {
      JsonArray responseArray = new JsonArray(_in);
      r = new OpenSocialResponse();

      for (int i=0; i<responseArray.Length; i++) {
        JsonObject o = responseArray.GetObject(i);

        if (o.Contains("id")) {
          String id = o["id"].ToString();
          r.addItem(id, escape(o.ToString()));
        }
      }
    }


    return r;
  }

  /**
   * Parses the passed JSON string into an OpenSocialResponse object -- if the
   * passed string represents a JSON object, it is added to the returned
   * object keyed on the passed ID.
   * 
   * @param  _in The complete JSON string returned from an OpenSocial container
   *            _in response to a request for data
   * @param  id The string ID to tag the JSON object string with as it is added
   *            to the OpenSocialResponse object
   * @throws JSONException
   */
  public static OpenSocialResponse getResponse(String _in, String id)
    {

    OpenSocialResponse r = null;

    if (_in[0] == '{') {
      r = new OpenSocialResponse();
      r.addItem(id, escape(_in));
    } else if (_in[0] == '[') {
      return getResponse(_in);
    }

    return r;
  }
  
  /**
   * Transforms a raw JSON object string containing profile information for a
   * single user into an OpenSocialPerson instance with all profile details
   * abstracted as OpenSocialField objects associated with the instance.
   * 
   * @param  _in The JSON object string to parse as an OpenSocialPerson object
   * @throws OpenSocialRequestException 
   * @throws JSONException 
   * @throws IllegalAccessException 
   * @throws InstantiationException 
   */
  public static OpenSocialPerson parseAsPerson(String _in)
   {

    if (_in == null) {
      throw new OpenSocialRequestException(
          "Response item with given key not found");
    }

    JsonObject root = (JsonObject)JsonConvert.Import(_in);
    JsonObject entry = getEntryObject(root);

    OpenSocialPerson p =
        (OpenSocialPerson) parseAsObject(entry, typeof(OpenSocialPerson));

    return p;
  }

  /**
   * Transforms a raw JSON object string containing profile information for a
   * group of users into a list of OpenSocialPerson instances with all profile
   * details abstracted as OpenSocialField objects associated with the
   * instances. These instances are then added to a Java List which
   * gets returned.
   * 
   * @param  _in The JSON object string to parse as a List of OpenSocialPerson
   *         objects
   * @throws OpenSocialRequestException 
   * @throws JSONException 
   * @throws IllegalAccessException 
   * @throws InstantiationException 
   */
  public static List<OpenSocialPerson> parseAsPersonCollection(String _in)
  {

    if (_in == null) {
      throw new OpenSocialRequestException(
          "Response item with given key not found");
    }

    JsonObject root = (JsonObject)JsonConvert.Import(_in);
    JsonArray entries = getEntryArray(root);
    List<OpenSocialPerson> l = new List<OpenSocialPerson>(entries.Length);

    for (int i=0; i<entries.Length; i++) {
      JsonObject entry = entries.GetObject(i);

      OpenSocialPerson p =
          (OpenSocialPerson) parseAsObject(entry, typeof(OpenSocialPerson));

      l.Add(p);
    }

    return l;
  }

  /**
   * Transforms a raw JSON object string containing key-value pairs (i.e. App
   * Data) for one or more users into a specialized OpenSocialObject instance
   * with each key-value pair abstracted as OpenSocialField objects associated
   * with the instance.
   * 
   * @param  _in The JSON object string to parse as an OpenSocialAppData object
   * @throws JSONException 
   * @throws OpenSocialRequestException 
   */
  public static OpenSocialAppData parseAsAppData(String _in)
 {

    if (_in == null) {
      throw new OpenSocialRequestException(
          "Response item with given key not found");
    }

    JsonObject root = (JsonObject)JsonConvert.Import(_in);
    JsonObject entry = getEntryObject(root);

    OpenSocialAppData d =
        (OpenSocialAppData) parseAsObject(entry, typeof(OpenSocialAppData));

    return d;
  }

  /**
   * Inspects the passed object for one of several specific properties and, if
   * found, returns that property as a JsonArray object. All valid response
   * objects which contain a data collection (e.g. a collection of people)
   * must have this property.
   * 
   * @param  root JsonObject to query for the presence of the specific property
   * @throws OpenSocialRequestException if property is not found _in the passed
   *         object
   * @throws JSONException
   */
  private static JsonArray getEntryArray(JsonObject root)
    {

    JsonArray entry = new JsonArray();

    if (root.Contains("entry")) {
      entry = (JsonArray)root["entry"];
    } else if (root.Contains("data")) {
      entry = (JsonArray)root.getJSONObject("data")["list"];
    } else {
      throw new OpenSocialRequestException("Entry not found");
    }

    return entry;
  }

  /**
   * Inspects the passed object for one of several specific properties and, if
   * found, returns that property as a JsonObject object. All valid response
   * objects which encapsulate a single data item (e.g. a person) must have
   * this property.
   * 
   * @param  root JsonObject to query for the presence of the specific property
   * @throws OpenSocialRequestException if property is not found _in the passed
   *         object
   * @throws JSONException
   */
  private static JsonObject getEntryObject(JsonObject root)
    {

    JsonObject entry = new JsonObject();

    if (root.Contains("data")) {
      entry = root.getJSONObject("data");
    } else if (root.Contains("entry")) {
      entry = root.getJSONObject("entry");
    } else {
      throw new OpenSocialRequestException("Entry not found");
    }

    return entry;
  }

  /**
   * Calls a function to recursively iterates through the the properties of the
   * passed JsonObject object and returns an equivalent OpenSocialObject with
   * each property of the original object mapped to fields _in the returned
   * object.
   * 
   * @param  entryObject Object-oriented representation of JSON response
   *         string which is transformed into and returned as an
   *         OpenSocialObject
   * @param  clientClass Class of object to return, either OpenSocialObject
   *         or a subclass
   * @throws JSONException
   * @throws InstantiationException
   * @throws IllegalAccessException
   */
  private static OpenSocialObject parseAsObject(
      JsonObject entryObject, Type clientClass)
    {

        OpenSocialObject o = (OpenSocialObject)Activator.CreateInstance(clientClass);

    Dictionary<String,OpenSocialField> entryRepresentation = 
        createObjectRepresentation(entryObject);

    foreach(var e in entryRepresentation) {
      o.setField(e.Key, e.Value);
    }

    return o;
  }

  /**
   * Recursively iterates through the properties of the passed JsonObject
   * object and returns a Map of OpenSocialField objects keyed on Strings
   * representing the property values and names respectively.
   * 
   * @param  o Object-oriented representation of a JSON object which is
   *         transformed into and returned as a Map of OpenSocialField
   *         objects keyed on Strings
   * @throws JSONException
   */
  private static Dictionary<String, OpenSocialField> createObjectRepresentation(
      JsonObject o)
    {

    Dictionary<String,OpenSocialField> r = new Dictionary<String,OpenSocialField>();

    var keys = o.Names;

      foreach (string key in keys)
      {
        String property = o[key].ToString();

      if (property.Length > 0 && property[0] == '{') {
        JsonObject p = o.getJSONObject(key);
        OpenSocialField field = new OpenSocialField(true);

        field.addValue(new OpenSocialObject(createObjectRepresentation(p)));
        r.Add(key, field);
      } else if (property.Length > 0 && property[0] == '[') {
        JsonArray p = (JsonArray)o[key];
        var values = createArrayRepresentation(p);
        OpenSocialField field = new OpenSocialField(true);

        foreach(var v in values) {
          field.addValue(v);
        }

        r.Add(key, field);
      } else if (property.Length > 0) {
        OpenSocialField field = new OpenSocialField(false);
        field.addValue(unescape(property));
        r.Add(key, field);
      }      
    }

    return r;
  }

  /**
   * Iterates through the objects _in the passed JsonArray object, recursively
   * transforms each as needed, and returns a List of Java objects.
   * 
   * @param  a Object-oriented representation of a JSON array which is iterated
   *         through and returned as a List of Java objects
   * @throws JSONException
   */
  private static List<Object> createArrayRepresentation(JsonArray a)
    {

    List<Object> r = new List<Object>(a.Length);

    for (int i=0; i<a.Length; i++) {
      String member = a.GetString(i);

      if (member.Length > 0 && member[0] == '{') {
        JsonObject p = a.GetObject(i);
        r.Add(new OpenSocialObject(createObjectRepresentation(p)));
      } else if (member.Length > 0 && member[0] == '[') {
        JsonArray p = (JsonArray)a[i];
        List<Object> values = createArrayRepresentation(p);

        foreach(var v in values) {
          r.Add(v);
        }
      } else if (member.Length > 0) {
        r.Add(member);
      }
    }

    return r;
  }

  /**
   * Escapes "{ and }" as "%7B and "%7D respectively to prevent parsing errors
   * when property values begin with { or } tokens.
   * 
   * @param  _in String to escape
   * @return escaped String
   */
  private static String escape(String _in) {
    String _out = _in;

    _out = _out.Replace("\"\\{", "\"%7B");
    _out = _out.Replace("\\}\"", "%7D\"");

    return _out;
  }

  /**
   * Unescapes String objects previously returned from the escape method by
   * substituting { and } for %7B and %7D respectively. Called after
   * parsing to restore property values.
   * 
   * @param  _in String to unescape
   * @return unescaped String
   */
  private static String unescape(String _in) {
    String _out = _in;

    _out = _out.Replace("%7B", "{");
    _out = _out.Replace("%7D", "}");

    return _out;
  }
}
