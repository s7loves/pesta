/*
 * Copyright 2007 Netflix, Inc.
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
using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for OAuthProblemException
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class OAuthProblemException : OAuthException
{
    public static readonly String OAUTH_PROBLEM = "oauth_problem";

    public static readonly String HTTP_STATUS_CODE = "HTTP status";

    public OAuthProblemException() 
    {
    }

    public OAuthProblemException(String problem) 
        : base(problem)
    {
        if (problem != null) 
        {
            parameters.Add(OAUTH_PROBLEM, problem);
        }
    }

    private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

    public void setParameter(String name, Object value) 
    {
        getParameters().Add(name, value);
    }

    public Dictionary<String, Object> getParameters() 
    {
        return parameters;
    }

    public String getProblem() 
    {
        return (String) getParameters()[OAUTH_PROBLEM];
    }

    public int getHttpStatusCode() 
    {
        Object code = getParameters()[HTTP_STATUS_CODE];
        if (code == null) 
        {
            return 200;
        } 
        else 
        {
            return int.Parse(code.ToString());
        }
    }

    private static readonly long serialVersionUID = 1L;
}
