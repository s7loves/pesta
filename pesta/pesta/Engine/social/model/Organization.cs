#region License, Terms and Conditions
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
/// Summary description for Organization
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
[ImplementedBy(typeof(OrganizationImpl))]
public abstract class Organization
{
    /**
    * Describes a current or past organizational affiliation of this contact. Service Providers that
    * support only a single Company Name and Job Title field should represent them with a single
    * organization element with name and title properties, respectively.
    *
    * see http://code.google.com/apis/opensocial/docs/0.7/reference/opensocial.Organization.Field.html
    *
    */

    /**
    * An Enumberation of field names for Organization.
    */
    public class Field : EnumBaseType<Field>
    {
        /// <summary>
        /// Initializes a new instance of the Field class.
        /// </summary>
        public Field()
        {
        }
        public Field(int key, string value) 
            : base(key,value)
        {
            
        }

    /** the name of the address field. */
        public static readonly Field ADDRESS = new Field(1, "address");
    /** the name of the description field. */
        public static readonly Field DESCRIPTION = new Field(2, "description");
    /** the name of the endDate field. */
        public static readonly Field END_DATE = new Field(3, "endDate");
    /** the name of the field field. */
        public static readonly Field FIELD = new Field(4, "field");
    /** the name of the name field. */
        public static readonly Field NAME = new Field(5, "name");
    /** the name of the salary field. */
        public static readonly Field SALARY = new Field(6, "salary");
    /** the name of the startDate field. */
        public static readonly Field START_DATE = new Field(7, "startDate");
    /** the name of the subField field. */
        public static readonly Field SUB_FIELD = new Field(8, "subField");
    /** the name of the title field. */
        public static readonly Field TITLE = new Field(9, "title");
    /** the name of the webpage field. */
        public static readonly Field WEBPAGE = new Field(10, "webpage");
    /**
    * the name of the type field, Should have the value of "job" or "school" to be put in the right
    * js fields.
    */
        public static readonly Field TYPE = new Field(11, "type");
    /** the name of the primary field. */
        public static readonly Field PRIMARY = new Field(12, "primary");

    /**
    * the name of this field.
    */
    private readonly String jsonString;

    /**
    * Construct a field based on the name of the field.
    *
    * @param jsonString the name of the field
    */
    private Field(String jsonString) 
    {
        this.jsonString = jsonString;
    }

    public override String ToString() 
    {
        return this.jsonString;
    }
    }

    /**
    * Get the address of the organization, specified as an Address. Container support for this field
    * is OPTIONAL.
    *
    * @return the Address of the organization
    */
    public abstract Address getAddress();

    /**
    * Set the address of the organization, specified as an Address. Container support for this field
    * is OPTIONAL.
    *
    * @param address the address of the organization
    */
    public abstract void setAddress(Address address);

    /**
    * Get a description or notes about the person's work in the organization, specified as a string.
    * This could be the courses taken by a student, or a more detailed description about a
    * Organization role. Container support for this field is OPTIONAL.
    *
    * @return a description about the persons work in the organization
    */
    public abstract String getDescription();

    /**
    * Set a description or notes about the person's work in the organization, specified as a string.
    * This could be the courses taken by a student, or a more detailed description about a
    * Organization role. Container support for this field is OPTIONAL.
    *
    * @param description a description about the persons work in the organization
    */
    public abstract void setDescription(String description);

    /**
    * Get the date the person stopped at the organization, specified as a Date. A null date indicates
    * that the person is still involved with the organization. Container support for this field is
    * OPTIONAL.
    *
    * @return the date the person stopped at the organization
    */
    public abstract DateTime? getEndDate();

    /**
    * Set the date the person stopped at the organization, specified as a Date. A null date indicates
    * that the person is still involved with the organization. Container support for this field is
    * OPTIONAL.
    *
    * @param endDate the date the person stopped at the organization
    */
    public abstract void setEndDate(DateTime? endDate);

    /**
    * Get the field the organization is in, specified as a string. This could be the degree pursued
    * if the organization is a school. Container support for this field is OPTIONAL.
    *
    * @return the field the organization is in
    */
    public abstract String getField();

    /**
    * Set the field the organization is in, specified as a string. This could be the degree pursued
    * if the organization is a school. Container support for this field is OPTIONAL.
    *
    * @param field the field the organization is in
    */
    public abstract void setField(String field);

    /**
    * Get the name of the organization, specified as a string. For example, could be a school name or
    * a job company. Container support for this field is OPTIONAL.
    *
    * @return the name of the organization
    */
    public abstract String getName();

    /**
    * Set the name of the organization, specified as a string. For example, could be a school name or
    * a job company. Container support for this field is OPTIONAL.
    *
    * @param name the name of the organization
    */
    public abstract void setName(String name);

    /**
    * Get the salary the person receives from the organization, specified as a string. Container
    * support for this field is OPTIONAL.
    *
    * @return the salary the person receives
    */
    public abstract String getSalary();

    /**
    * Set the salary the person receives from the organization, specified as a string. Container
    * support for this field is OPTIONAL.
    *
    * @param salary the salary the person receives
    */
    public abstract void setSalary(String salary);

    /**
    * Get the date the person started at the organization, specified as a Date. Container support for
    * this field is OPTIONAL.
    *
    * @return the start date at the organization
    */
    public abstract DateTime? getStartDate();

    /**
    * Set the date the person started at the organization, specified as a Date. Container support for
    * this field is OPTIONAL.
    *
    * @param startDate the start date at the organization
    */
    public abstract void setStartDate(DateTime? startDate);

    /**
    * Get the subfield the Organization is in, specified as a string. Container support for this
    * field is OPTIONAL.
    *
    * @return the subfield the Organization is in
    */
    public abstract String getSubField();

    /**
    * Set the subfield the Organization is in, specified as a string. Container support for this
    * field is OPTIONAL.
    *
    * @param subField the subfield the Organization is in
    */
    public abstract void setSubField(String subField);

    /**
    * Get the title or role the person has in the organization, specified as a string. This could be
    * graduate student, or software engineer. Container support for this field is OPTIONAL.
    *
    * @return the title or role the person has in the organization
    */
    public abstract String getTitle();

    /**
    * Set the title or role the person has in the organization, specified as a string. This could be
    * graduate student, or software engineer. Container support for this field is OPTIONAL.
    *
    * @param title the title or role the person has in the organization
    */
    public abstract void setTitle(String title);

    /**
    * Get a webpage related to the organization, specified as a string. Container support for this
    * field is OPTIONAL.
    *
    * @return the URL of a webpage related to the organization
    */
    public abstract String getWebpage();

    /**
    * Get a webpage related to the organization, specified as a string. Container support for this
    * field is OPTIONAL.
    *
    * @param webpage the URL of a webpage related to the organization
    */
    public abstract void setWebpage(String webpage);

    /**
    * Get the type of field for this instance, usually used to label the preferred function of the
    * given contact information. The type of organization, with Canonical Values <em>job</em> and
    * <em>school</em>.
    *
    * @return the type of the field
    */
    public abstract String getType();

    /**
    * Set the type of field for this instance, usually used to label the preferred function of the
    * given contact information. The type of organization, with Canonical Values <em>job</em> and
    * <em>school</em>.
    *
    * @param type the type of the field
    */
    public abstract void setType(String type);

    /**
    * Get Boolean value indicating whether this instance of the Plural Field is the primary or
    * preferred Organization.
    *
    * @return true if this is a primary or preferred value
    */
    public abstract bool getPrimary();

    /**
    * Set Boolean value indicating whether this instance of the Plural Field is the primary or
    * preferred Organization.
    *
    * @param primary true if this is a primary or preferred value
    */
    public abstract void setPrimary(bool primary);
}
