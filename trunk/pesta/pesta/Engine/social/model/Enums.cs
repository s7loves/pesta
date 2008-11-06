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
using System.Data;
using System.Configuration;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public interface EnumKey
{
    String getDisplayValue();
}

public abstract class Enums<T> where T : EnumKey
{

    public class Field : EnumBaseType<Field>
    {
        public Field(int key, string value)
            : base(key, value)
        {

        }
        public static readonly Field VALUE = new Field(1,"value");
        public static readonly Field DISPLAY_VALUE = new Field(2,"displayValue");

        private readonly String jsonString;

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
* Gets the value of this Enum. This is the string displayed to the user. If the container
* supports localization, the string should be localized.
*
* @return the Enum's user visible value
*/
    public abstract String getDisplayValue();

    /**
    * Sets the value of this Enum. This is the string displayed to the user. If the container
    * supports localization, the string should be localized.
    *
    * @param displayValue The value to set.
    */
    public abstract void setDisplayValue(String displayValue);

    /**
    * Gets the key for this Enum. Use this for logic within your gadget.
    *
    * @return java.lang.Enum key object for this Enum.
    */
    public abstract T getValue();

    /**
    * Sets the key for this Enum. Use this for logic within your gadget.
    *
    * @param value The value to set.
    */
    public abstract void setValue(T value);


    
}

public class EnumTypes
{
    public class Drinker : EnumBaseType<Drinker>, EnumKey
    {
        public Drinker(int key, string value)
            : base(key, value)
        {

        }
        public static readonly Drinker HEAVILY = new Drinker("HEAVILY", "Heavily");
        public static readonly Drinker NO = new Drinker("NO", "No");
        public static readonly Drinker OCCASIONALLY = new Drinker("OCCASIONALLY", "Occasionally");
        public static readonly Drinker QUIT = new Drinker("QUIT", "Quit");
        public static readonly Drinker QUITTING = new Drinker("QUITTING", "Quitting");
        public static readonly Drinker REGULARLY = new Drinker("REGULARLY", "Regularly");
        public static readonly Drinker SOCIALLY = new Drinker("SOCIALLY", "Socially");
        public static readonly Drinker YES = new Drinker("YES", "Yes");

        private readonly String jsonString;

        private readonly String displayValue;

        private Drinker(String jsonString, String displayValue)
        {
            this.jsonString = jsonString;
            this.displayValue = displayValue;
        }

        public override String ToString()
        {
            return this.jsonString;
        }

        public String getDisplayValue()
        {
            return displayValue;
        }
    }

    public class Smoker : EnumBaseType<Smoker>, EnumKey
    {
        public Smoker(int key, string value)
            : base(key, value)
        {

        }
        public static readonly Smoker HEAVILY = new Smoker("HEAVILY", "Heavily");
        public static readonly Smoker NO = new Smoker("NO", "No");
        public static readonly Smoker OCCASIONALLY = new Smoker("OCCASIONALLY", "Ocasionally");
        public static readonly Smoker QUIT = new Smoker("QUIT", "Quit");
        public static readonly Smoker QUITTING = new Smoker("QUITTING", "Quitting");
        public static readonly Smoker REGULARLY = new Smoker("REGULARLY", "Regularly");
        public static readonly Smoker SOCIALLY = new Smoker("SOCIALLY", "Socially");
        public static readonly Smoker YES = new Smoker("YES", "Yes");

        private readonly String jsonString;

        private readonly String displayValue;

        private Smoker(String jsonString, String displayValue)
        {
            this.jsonString = jsonString;
            this.displayValue = displayValue;
        }

        public override String ToString()
        {
            return this.jsonString;
        }

        public String getDisplayValue()
        {
            return displayValue;
        }
    }


    public class NetworkPresence : EnumBaseType<NetworkPresence>, EnumKey
    {
        public NetworkPresence(int key, string value)
            : base(key, value)
        {

        }
        public static readonly NetworkPresence ONLINE = new NetworkPresence("ONLINE", "Online");
        public static readonly NetworkPresence OFFLINE = new NetworkPresence("OFFLINE", "Offline");
        public static readonly NetworkPresence AWAY = new NetworkPresence("AWAY", "Away");
        public static readonly NetworkPresence CHAT = new NetworkPresence("CHAT", "Chat");
        public static readonly NetworkPresence DND = new NetworkPresence("DND", "Do Not Disturb");
        public static readonly NetworkPresence XA = new NetworkPresence("XA", "Extended Away");

        private readonly String jsonString;

        private readonly String displayValue;

        private NetworkPresence(String jsonString, String displayValue)
        {
            this.jsonString = jsonString;
            this.displayValue = displayValue;
        }

        public override String ToString()
        {
            return this.jsonString;
        }

        public String getDisplayValue()
        {
            return displayValue;
        }
    }

    public class LookingFor : EnumBaseType<LookingFor>, EnumKey
    {
        public LookingFor(int key, string value)
            : base(key, value)
        {

        }
        public static readonly LookingFor DATING = new LookingFor("DATING", "Dating");
        public static readonly LookingFor FRIENDS = new LookingFor("FRIENDS", "Friends");
        public static readonly LookingFor RELATIONSHIP = new LookingFor("RELATIONSHIP", "Relationship");
        public static readonly LookingFor NETWORKING = new LookingFor("NETWORKING", "Networking");
        public static readonly LookingFor ACTIVITY_PARTNERS = new LookingFor("ACTIVITY_PARTNERS", "Activity partners");
        public static readonly LookingFor RANDOM = new LookingFor("RANDOM", "Random");

        private readonly String jsonString;

        private readonly String displayValue;

        private LookingFor(String jsonString, String displayValue)
        {
            this.jsonString = jsonString;
            this.displayValue = displayValue;
        }

        public override String ToString()
        {
            return this.jsonString;
        }

        public String getDisplayValue()
        {
            return displayValue;
        }
    }
}