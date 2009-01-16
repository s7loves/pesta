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
    /// Summary description for BodyType
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(BodyTypeImpl))]
    public abstract class BodyType
    {
        /**
        * The fields that represent the person object in serialized form.
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
                : base(key, value)
            {

            }
            /** the field name for build. */
            public static readonly Field BUILD = new Field(1, "build");
            /** the field name for build. */
            public static readonly Field EYE_COLOR = new Field(2, "eyeColor");
            /** the field name for hairColor. */
            public static readonly Field HAIR_COLOR = new Field(3, "hairColor");
            /** the field name for height. */
            public static readonly Field HEIGHT = new Field(4, "height");
            /** the field name for weight. */
            public static readonly Field WEIGHT = new Field(5, "weight");

            /**
            * The field name that the instance represents.
            */
            private readonly String jsonString;

            /**
            * create a field base on the a json element.
            *
            * @param jsonString the name of the element
            */
            private Field(String jsonString)
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
                return this.jsonString;
            }
        }

        /**
        * The build of the person's body, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @return the build of the person's body
        */
        public abstract String getBuild();

        /**
        * The build of the person's body, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @param build the build of the person's body
        */
        public abstract void setBuild(String build);

        /**
        * The eye color of the person, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @return the eye color of the person
        */
        public abstract String getEyeColor();

        /**
        * The eye color of the person, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @param eyeColor the eye color of the person
        */
        public abstract void setEyeColor(String eyeColor);

        /**
        * The hair color of the person, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @return the hair color of the person
        */
        public abstract String getHairColor();

        /**
        * The hair color of the person, specified as a string. Container support for this field is
        * OPTIONAL.
        *
        * @param hairColor the hair color of the person
        */
        public abstract void setHairColor(String hairColor);

        /**
        * The height of the person in meters, specified as a number. Container support for this field is
        * OPTIONAL.
        *
        * @return the height of the person in meters
        */
        public abstract float? getHeight();

        /**
        * The height of the person in meters, specified as a number. Container support for this field is
        * OPTIONAL.
        *
        * @param height the height of the person in meters
        */
        public abstract void setHeight(float height);

        /**
        * The weight of the person in kilograms, specified as a number. Container support for this field
        * is OPTIONAL.
        *
        * @return the weight of the person in kilograms
        */
        public abstract float? getWeight();

        /**
        * The weight of the person in kilograms, specified as a number. Container support for this field
        * is OPTIONAL.
        *
        * @param weight weight of the person in kilograms
        */
        public abstract void setWeight(float weight);
    } 
}
