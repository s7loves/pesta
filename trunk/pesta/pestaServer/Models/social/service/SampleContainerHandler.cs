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
using System.IO;
using System.Net;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;


namespace pestaServer.Models.social.service
{
    /// <summary>
    /// Summary description for SampleContainerHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class SampleContainerHandler : DataRequestHandler
    {
        private readonly JsonDbOpensocialService service;

        private const String POST_PATH = "/samplecontainer/{type}/{doevil}";

        public SampleContainerHandler()
        {
            service = JsonDbOpensocialService.Instance;
        }

        /**
        * We don't support any delete methods right now.
        */
        protected override object handleDelete(RequestItem request)
        {
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, null);
        }

        /**
        * We don't distinguish between put and post for these urls.
        */
        protected override object handlePut(RequestItem request)
        {
            return handlePost(request);
        }

        /**
        * Handles /samplecontainer/setstate and /samplecontainer/setevilness/{doevil}. TODO(doll): These
        * urls aren't very resty. Consider changing the samplecontainer.html calls post.
        */
        protected override object handlePost(RequestItem request)
        {
            request.applyUrlTemplate(POST_PATH);
            String type = request.getParameter("type");
            if (type.Equals("setstate"))
            {
                try
                {
                    String stateFile = request.getParameter("fileurl");
                    service.SetDb(JsonConvert.Import(FetchStateDocument(stateFile)) as JsonObject);
                }
                catch (JsonException e)
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                                 "The json state file was not valid json", e);
                }
            }
            else if (type.Equals("setevilness"))
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED,
                                             "evil data has not been implemented yet");
            }
            return new JsonObject();
        }

        /**
        * Handles /samplecontainer/dumpstate
        */
        protected override object handleGet(RequestItem request)
        {
            return service.getDb();
        }

        private static String FetchStateDocument(String stateFileLocation)
        {
            String errorMessage = "The json state file " + stateFileLocation
                                  + " could not be fetched and parsed.";

            using (WebClient client = new WebClient())
            {
                try
                {
                    Stream data = client.OpenRead(stateFileLocation);
                    StreamReader reader = new StreamReader(data);
                    String retVal = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                    return retVal;
                }
                catch (Exception e)
                {
                    throw new Exception(errorMessage, e);
                }
            }

        }
    }
}