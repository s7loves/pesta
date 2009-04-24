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
using Pesta.Engine.social.core.model;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for Address
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(AddressImpl))]
    public abstract class Address
    {
        /**
       * The fields that represent the address object in json form.
       */
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field(int key, string value)
                : base(key, value)
            {

            }
            public static readonly Field COUNTRY = new Field(1, "country");
            /** the field name for latitude. */
            public static readonly Field LATITUDE = new Field(2, "latitude");
            /** the field name for locality. */
            public static readonly Field LOCALITY = new Field(3, "locality");
            /** the field name for longitude. */
            public static readonly Field LONGITUDE = new Field(4, "longitude");
            /** the field name for postalCode. */
            public static readonly Field POSTAL_CODE = new Field(5, "postalCode");
            /** the field name for region. */
            public static readonly Field REGION = new Field(6, "region");
            /** the feild name for streetAddress this field may be multiple lines. */
            public static readonly Field STREET_ADDRESS = new Field(7, "streetAddress");
            /** the field name for type. */
            public static readonly Field TYPE = new Field(8, "type");
            /** the field name for formatted. */
            public static readonly Field FORMATTED = new Field(9, "formatted");
            /** the field name for primary. */
            public static readonly Field PRIMARY = new Field(10, "primary");

            /**
            * The json field that the instance represents.
            */
            private readonly String jsonString;

            /**
            * create a field base on the a json element.
            *
            * @param jsonString the name of the element
            */
            protected Field(String jsonString)
            {
                this.jsonString = jsonString;
            }

            /**
            * emit the field as a json element.
            *
            * @return the field name
            */
            public override String ToString()
            {
                return jsonString;
            }
        }

        /**
        * Get the country.
        *
        * @return the country
        */
        public abstract String getCountry();

        /**
        * Set the country.
        *
        * @param country the country
        */
        public abstract void setCountry(String country);

        /**
        * Get the latitude.
        *
        * @return latitude
        */
        public abstract float? getLatitude();

        /**
        * Set the latitude.
        *
        * @param latitude latitude
        */
        public abstract void setLatitude(float? latitude);

        /**
        * Get the locality.
        *
        * @return the locality
        */
        public abstract String getLocality();

        /**
        * Set the locality.
        *
        * @param locality the locality
        */
        public abstract void setLocality(String locality);

        /**
        * Get the longitude of the address in degrees.
        *
        * @return the longitude of the address in degrees
        */
        public abstract float? getLongitude();

        /**
        * Set the longitude of the address in degrees.
        *
        * @param longitude the longitude of the address in degrees.
        */
        public abstract void setLongitude(float? longitude);

        /**
        * Get the Postal code for the address.
        *
        * @return the postal code for the address
        */
        public abstract String getPostalCode();

        /**
        * Set the postal code for the address.
        *
        * @param postalCode the postal code
        */
        public abstract void setPostalCode(String postalCode);

        /**
        * Get the region.
        *
        * @return the region
        */
        public abstract String getRegion();

        /**
        * Set the region.
        *
        * @param region the region
        */
        public abstract void setRegion(String region);

        /**
        * Get the street address.
        *
        * @return the street address
        */
        public abstract String getStreetAddress();

        /**
        * Set the street address.
        *
        * @param streetAddress the street address
        */
        public abstract void setStreetAddress(String streetAddress);

        /**
        * Get the type of label of the address.
        *
        * @return the type or label of the address
        */
        public abstract String getType();

        /**
        * Get the type of label of the address.
        *
        * @param type the type of label of the address.
        */
        public abstract void setType(String type);

        /**
        * Get the formatted address.
        *
        * @return the formatted address
        */
        public abstract String getFormatted();

        /**
        * Set the formatted address.
        *
        * @param formatted the formatted address
        */
        public abstract void setFormatted(String formatted);

        /**
        * <p>
        * Get a bool value indicating whether this instance of the Plural Field is the primary or
        * preferred value of for this field, e.g. the preferred mailing address. Service Providers MUST
        * NOT mark more than one instance of the same Plural Field as primary="true", and MAY choose not
        * to mark any fields as primary, if this information is not available. Introduced in v0.8.1
        * </p><p>
        * The service provider may wish to share the address instance between items and primary related
        * to the address from which this came, so if the address came from an Organization, primary
        * relates to the primary address of the organization, and not necessary the primary address of
        * all addresses.
        * </p><p>
        * If the address is not part of a list (eg Person.location ) primary has no meaning.
        * <p>
        * @return true if the instance if the primary instance.
        */
        public abstract bool? getPrimary();

        /**
        * @see Address.getPrimary()
        * @param primary set the Primary status of this Address.
        */
        public abstract void setPrimary(bool? primary);
    } 
}
