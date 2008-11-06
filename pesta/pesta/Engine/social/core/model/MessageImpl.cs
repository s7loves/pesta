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
/// Summary description for MessageImpl
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class MessageImpl : Message
{
    private String body;
    private String title;
    private Type type;

    public MessageImpl()
    {
    }

    public MessageImpl(String initBody, String initTitle, Type initType)
    {
        this.body = initBody;
        this.title = initTitle;
        this.type = initType;
    }

    public override String getBody()
    {
        return this.body;
    }

    public override void setBody(String newBody)
    {
        this.body = newBody;
    }

    public override String getTitle()
    {
        return this.title;
    }

    public override void setTitle(String newTitle)
    {
        this.title = newTitle;
    }

    public override Type getType()
    {
        return type;
    }

    public override void setType(Type newType)
    {
        this.type = newType;
    }

    public override String sanitizeHTML(String htmlStr)
    {
        return htmlStr;
    }
}
