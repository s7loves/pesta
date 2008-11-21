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
/// Summary description for AddressImpl
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class AddressImpl : Address
{
    private String country;
    private float? latitude;
    private float? longitude;
    private String locality;
    private String postalCode;
    private String region;
    private String streetAddress;
    private String type;
    private String formatted;
    private bool? primary;

    public AddressImpl() { }

    public AddressImpl(String formatted)
    {
        this.formatted = formatted;
    }

    public override String getCountry()
    {
        return country;
    }

    public override void setCountry(String country)
    {
        this.country = country;
    }

    public override float? getLatitude()
    {
        return latitude;
    }

    public override void setLatitude(float? latitude)
    {
        this.latitude = latitude;
    }

    public override String getLocality()
    {
        return locality;
    }

    public override void setLocality(String locality)
    {
        this.locality = locality;
    }

    public override float? getLongitude()
    {
        return longitude;
    }

    public override void setLongitude(float? longitude)
    {
        this.longitude = longitude;
    }

    public override String getPostalCode()
    {
        return postalCode;
    }

    public override void setPostalCode(String postalCode)
    {
        this.postalCode = postalCode;
    }

    public override String getRegion()
    {
        return region;
    }

    public override void setRegion(String region)
    {
        this.region = region;
    }

    public override String getStreetAddress()
    {
        return streetAddress;
    }

    public override void setStreetAddress(String streetAddress)
    {
        this.streetAddress = streetAddress;
    }

    public override String getType()
    {
        return type;
    }

    public override void setType(String type)
    {
        this.type = type;
    }

    public override String getFormatted()
    {
        return formatted;
    }

    public override void setFormatted(String formatted)
    {
        this.formatted = formatted;
    }

    public override bool? getPrimary()
    {
        return primary;
    }

    public override void setPrimary(bool? primary)
    {
        this.primary = primary;
    }
}
