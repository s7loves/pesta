﻿#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion
using System;


/// <summary>
/// Summary description for ListField
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
[ImplementedBy(typeof(ListFieldImpl))]
public interface ListField 
{
    /**
    * The fields that represent the ListField object in serialized form.
    */
    

    /**
    * The type of field for this instance, usually used to label the preferred function of the given
    * contact information. Unless otherwise specified, this string value specifies Canonical Values
    * of <em>work</em>, <em>home</em>, and <em>other</em>.
    *
    * @return the type of the field
    */
    String getType();

    /**
    * Set the type of the field.
    * @param type the type of the field
    */
    void setType(String type);

    /**
    * Get the primary value of this field, e.g. the actual e-mail address, phone number, or URL. When
    * specifying a sortBy field in the Query Parameters for a Plural Field, the default meaning is to
    * sort based on this value sub-field. Each non-empty Plural Field value MUST contain at least the
    * value sub-field, but all other sub-fields are optional.
    *
    * @return the value of the field
    */
    String getValue();

    /**
    * @see ListField.getValue()
    * @param value the value of the field
    */
    void setValue(String value);

    /**
    * Get Boolean value indicating whether this instance of the Plural Field is the primary or
    * preferred value of for this field, e.g. the preferred mailing address or primary e-mail
    * address. Service Providers MUST NOT mark more than one instance of the same Plural Field as
    * primary="true", and MAY choose not to mark any fields as primary, if this information is not
    * available. For efficiency, Service Providers SHOULD NOT mark all non-primary fields with
    * primary="false", but should instead omit this sub-field for all non-primary instances.
    *
    * @return true if this is a primary or preferred value
    */
    bool getPrimary();

    /**
    * @see ListField.getPrimary()
    * @param primary set to true if a primary or preferred value
    */
    void setPrimary(bool primary);
}
