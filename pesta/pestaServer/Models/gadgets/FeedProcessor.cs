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
using System.ServiceModel.Syndication;
using System.Xml;
using Jayrock.Json;
using pestaServer.Models.gadgets.http;

namespace pestaServer.Models.gadgets
{
    
    public class FeedProcessor
    {
        private BasicHttpFetcher fetcher = BasicHttpFetcher.Instance;
        public JsonObject process(String feedUrl, String feedXml,
                                  bool getSummaries, int numEntries)
        {
            JsonObject json = new JsonObject();
            XmlReader reader = XmlReader.Create(feedUrl);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            if (feed == null)
            {
                throw new GadgetException(GadgetException.Code.FAILED_TO_RETRIEVE_CONTENT,
                                          "Unable to retrieve feed xml.");
            }
            json.Put("Title", feed.Title);
            json.Put("URL", feedUrl);
            json.Put("Description", feed.Description);
            json.Put("Link", feed.Links);

            var authors = feed.Authors;
            String jsonAuthor = null;
            if (authors.Count != 0)
            {
                SyndicationPerson author = authors[0];
                if (!String.IsNullOrEmpty(author.Name))
                {
                    jsonAuthor = author.Name;
                }
                else if (!String.IsNullOrEmpty(author.Email))
                {
                    jsonAuthor = author.Email;
                }
            }

            JsonArray entries = new JsonArray();
            json.Put("Entry", entries);

            int entryCnt = 0;
            foreach(var obj in feed.Items) 
            {
                SyndicationItem e = obj;
                if (entryCnt >= numEntries)
                {
                    break;
                }
                entryCnt++;

                JsonObject entry = new JsonObject();
                entry.Put("Title", e.Title);
                entry.Put("Link", e.Links);
                if (getSummaries) 
                {
                    entry.Put("Summary", e.Summary);
                }
                if (e.LastUpdatedTime == e.PublishDate)
                {
                    entry.Put("Date", e.PublishDate.LocalDateTime.ToShortDateString());
                }
                else
                {
                    entry.Put("Date", e.LastUpdatedTime.LocalDateTime.ToShortDateString());
                }

                // if no author at feed level, use the first entry author
                if (jsonAuthor == null && e.Authors.Count != 0) 
                {
                    jsonAuthor = e.Authors[0].Name;
                }

                entries.Put(entry);
            }

            json.Put("Author", jsonAuthor ?? "");
            reader.Close();
            return json;
        }
    }
}