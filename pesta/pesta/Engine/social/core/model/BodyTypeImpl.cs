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
/// Summary description for BodyTypeImpl
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class BodyTypeImpl : BodyType
{
    private String build;
    private String eyeColor;
    private String hairColor;
    private float? height;
    private float? weight;


    public override String getBuild()
    {
        return build;
    }

    public override void setBuild(String build)
    {
        this.build = build;
    }

    public override String getEyeColor()
    {
        return eyeColor;
    }

    public override void setEyeColor(String eyeColor)
    {
        this.eyeColor = eyeColor;
    }

    public override String getHairColor()
    {
        return hairColor;
    }

    public override void setHairColor(String _hairColor)
    {
        this.hairColor = _hairColor;
    }

    public override float? getHeight()
    {
        return height;
    }

    public override void setHeight(float _height)
    {
        this.height = _height;
    }

    public override float? getWeight()
    {
        return weight;
    }

    public override void setWeight(float _weight)
    {
        this.weight = _weight;
    }
}
