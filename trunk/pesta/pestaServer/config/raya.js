/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
{
"gadgets.container" : ["raya"],
"gadgets.parent" : null,
"gadgets.lockedDomainRequired" : false,
"gadgets.lockedDomainSuffix" : "-pesta.my6solutions.com",
"gadgets.iframeBaseUri" : "/gadgets/ifr",
"gadgets.jsUriTemplate" : "http://pesta.my6solutions.com/gadgets/js/%js%",
"gadgets.features" : {
  "core.io" : {
    "proxyUrl" : "http://pesta.my6solutions.com/gadgets/proxy?refresh=%refresh%&url=%url%",
    "jsonProxyUrl" : "http://pesta.my6solutions.com/gadgets/makeRequest"
  },
  "views" : {
    "profile" : {
      "isOnlyVisible" : false,
      "urlTemplate" : "http://raya.my6solutions.com/profile/{var}",
      "aliases": ["DASHBOARD", "default"]
    },
    "canvas" : {
      "isOnlyVisible" : true,
      "urlTemplate" : "http://raya.my6solutions.com/profile/application/{var}",
      "aliases" : ["FULL_PAGE"]
    }
  },
  "rpc" : {
    "parentRelayUrl" : "/gadgets/files/container/rpc_relay.html",
    "useLegacyProtocol" : false
  },
  "skins" : {
    "properties" : {
      "BG_COLOR": "",
      "BG_IMAGE": "",
      "BG_POSITION": "",
      "BG_REPEAT": "",
      "FONT_COLOR": "",
      "ANCHOR_COLOR": ""
    }
  },
  "opensocial-0.8" : {
    "impl" : "rpc",  //Use "rpc" to enable JSON-RPC, "rest' for REST
    "path" : "http://%host%/social",
    "domain" : "pesta",
    "enableCaja" : false,
    "supportedFields" : {
       "person" : ["id", {"name" : ["familyName", "givenName", "unstructured"]}, "thumbnailUrl", "profileUrl"],
       "activity" : ["id", "title"]
    }
  },
  "osapi.base" : {
    // Use EL when available.
    "rpcUrl" : "http://%host%/social"
  }
}
}