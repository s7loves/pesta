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
 * An object representing a single property of an OpenSocial entity. These
 * properties may have a single value or multiple values and these values
 * can either consist of simple character strings or complex entities;
 * for example, the "name" field of a Person instance can be modeled
 * as an OpenSocialObject with "givenName" and "familyName" fields.
 *
 * @author Jason Cooper
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OpenSocialField {

  private List<Object> values;
  private bool complex;

  public OpenSocialField(bool complex) {
    this.complex = complex;

    this.values = new List<Object>();
  }

  /**
   * Stores passed object as a value.
   */
  public void addValue(Object o) {
    this.values.Add(o);
  }

  /**
   * Returns the first stored value as a String. Returns null if no values
   * have been associated with the instance.
   */
  public String getStringValue() {
    if (this.values.Count == 0) {
      return null;
    }

    Object[] objectValues = this.values.ToArray();

    if (this.complex == true) {
      return objectValues[0].ToString();
    }

    return (String) objectValues[0];
  }

  /**
   * Returns the first stored value as an OpenSocialObject. Returns null if
   * no values have been associated with the instance.
   * 
   * @throws OpenSocialException if the complex instance variable is false,
   *         indicating that the values are stored as simple String objects.
   */
  public OpenSocialObject getValue() 
  {
    if (this.values.Count == 0) {
      return null;
    }
    if (this.complex == false) {
      throw new OpenSocialException(
          "String-based field cannot be returned as an OpenSocialObject");
    }

    Object[] objectValues = values.ToArray();
    return (OpenSocialObject) objectValues[0];
  }

  /**
   * Returns all stored values as a Java Collection of String objects.
   */
  public List<String> getStringValues() {
    var stringCollection = new List<String>(values.Count);

    foreach(Object o in values) {
      if (this.complex == true) {
        stringCollection.Add(o.ToString());
      } else {
        stringCollection.Add((String) o);
      }
    }

    return stringCollection;
  }

  /**
   * Returns all stored values as a Java Collection of OpenSocialObject
   * instances.
   *
   * @throws OpenSocialException if the complex instance variable is false,
   *         indicating that the values are stored as simple String objects.
   */
  public List<OpenSocialObject> getValues() 
  {
    if (this.complex == false) {
      throw new OpenSocialException(
          "String-based fields cannot be returned as an OpenSocialObject " +
          "collection");
    }

    var objectCollection = new List<OpenSocialObject>(this.values.Count);

    foreach(Object o in this.values) {
      objectCollection.Add((OpenSocialObject) o);
    }

    return objectCollection;
  }

  /**
   * Returns true if the number of stored values is greater than one, false
   * otherwise.
   */
  public bool isMultivalued() {
    return (values.Count > 1);
  }

  /**
   * Returns false if values are stored as simple String objects, true if
   * OpenSocialObject instances are stored instead.
   */
  public bool isComplex() {
    return this.complex;
  }
}
