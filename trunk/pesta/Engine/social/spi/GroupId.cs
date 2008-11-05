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
using System.Collections.Generic;

/// <summary>
/// Summary description for GroupId
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class GroupId
{
    public enum Type 
    {
        all, 
        friends, 
        self, 
        deleted, 
        groupId
    }

    /** A map of JSON strings to Type objects */
    private static readonly Dictionary<String, Type> jsonTypeMap = 
        new Dictionary<string,Type>(){{"@all", Type.all},{"@friends", Type.friends},
                {"@self", Type.self},{"@deleted", Type.deleted},{"@groupId", Type.groupId}};

    
    /** Return the Type enum value given a specific jsonType **/
    public static Type? jsonValueOf(String jsonType) 
    {
        return jsonTypeMap[jsonType] as Type?;
    }


    private Type type;
    private String groupId;

    public GroupId(Type type, String groupId) 
    {
        this.groupId = groupId;
        this.type = type;
    }

    public Type getType() 
    {
        return type;
    }

    public String getGroupId() 
    {
        // Only valid for objects with type=groupId
        return groupId;
    }

    public static GroupId fromJson(String jsonId) 
    {
        Type? idSpecEnum = GroupId.jsonValueOf(jsonId);
        if (idSpecEnum != null) 
        {
            return new GroupId(idSpecEnum.Value, null);
        }

        return new GroupId(Type.groupId, jsonId);
    }

    // These are overriden so that EasyMock doesn't throw a fit
    public override bool Equals(Object o) 
    {
        if (!(o is GroupId)) 
        {
            return false;
        }

        GroupId actual = (GroupId) o;
        return this.type == actual.type
                    && this.groupId.Equals(actual.groupId);
    }

    public override int GetHashCode() 
    {
        int groupHashCode = 0;
        if (this.groupId != null) 
        {
            groupHashCode = this.groupId.GetHashCode();
        }
        return this.type.GetHashCode() + groupHashCode;
    }
}
