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

using System.Web;

/// <summary>
/// Summary description for ResponseItem
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class ResponseItem
{
    /**
    * The ResponseError associated with the item.
    */
    private ResponseError error;

    /**
    * The error message.
    */
    private String errorMessage;

    /**
    * The response value.
    */
    private Object response;

    /**
    * Create a ResponseItem specifying the ResponseError and error Message.
    * @param error a ResponseError
    * @param errorMessage the Error Message
    */
    public ResponseItem(ResponseError error, String errorMessage) 
    {
        this.error = error;
        this.errorMessage = errorMessage;
        this.response = null;
    }

    /**
    * Create a ResponseItem specifying a value.
    */
    public ResponseItem(Object response) 
    {
        this.error = null;
        this.errorMessage = null;
        this.response = response;
    }

    /**
    * Get the response value.
    */
    public Object getResponse() 
    {
        return response;
    }

    /**
    * Get the ResponseError associated with this ResponseItem.
    * @return the ResponseError associated with this ResponseItem
    */
    public ResponseError getError() 
    {
        return error;
    }

    /**
    * Get the Error Message associated with this Response Item.
    * @return the Error Message
    */
    public String getErrorMessage() 
    {
        return errorMessage;
    }


    public override bool Equals(Object o) 
    {
        if (this == o) 
        {
            return true;
        }

        if (!(o is ResponseItem)) 
        {
            return false;
        }

        ResponseItem that = (ResponseItem) o;
        return (error == that.error)
                && Object.Equals(errorMessage, that.errorMessage)
                && Object.Equals(response, that.response);
    }

    public override int GetHashCode() 
    {
        int result = (error != null ? error.GetHashCode() : 0);
        result = 31 * result + (errorMessage != null ? errorMessage.GetHashCode() : 0);
        result = 31 * result + (response != null ? response.GetHashCode() : 0);
        return result;
    }
}
