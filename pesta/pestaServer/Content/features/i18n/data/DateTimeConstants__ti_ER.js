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

var gadgets = gadgets || {};

gadgets.i18n = gadgets.i18n || {};

gadgets.i18n.DateTimeConstants = {
  ERAS:["\u12d3/\u12d3","\u12d3/\u121d"],
  ERANAMES:["\u12d3/\u12d3","\u12d3/\u121d"],
  NARROWMONTHS:["\u1303","\u134c","\u121b","\u12a4","\u121c","\u1301","\u1301","\u12a6","\u1234","\u12a6","\u1296","\u12f2"],
  MONTHS:["\u1325\u122a","\u1208\u12ab\u1272\u1275","\u1218\u130b\u1262\u1275","\u121a\u12eb\u12dd\u12eb","\u130d\u1295\u1266\u1275","\u1230\u1290","\u1213\u121d\u1208","\u1290\u1213\u1230","\u1218\u1235\u12a8\u1228\u121d","\u1325\u1245\u121d\u1272","\u1215\u12f3\u122d","\u1273\u1215\u1233\u1235"],
  SHORTMONTHS:["\u1325\u122a","\u1208\u12ab\u1272","\u1218\u130b\u1262","\u121a\u12eb\u12dd","\u130d\u1295\u1266","\u1230\u1290","\u1213\u121d\u1208","\u1290\u1213\u1230","\u1218\u1235\u12a8","\u1325\u1245\u121d","\u1215\u12f3\u122d","\u1273\u1215\u1233"],
  WEEKDAYS:["\u1230\u1295\u1260\u1275","\u1230\u1291\u12ed","\u1230\u1209\u1235","\u1228\u1261\u12d5","\u1213\u1219\u1235","\u12d3\u122d\u1262","\u1240\u12f3\u121d"],
  SHORTWEEKDAYS:["\u1230\u1295\u1260","\u1230\u1291\u12ed","\u1230\u1209\u1235","\u1228\u1261\u12d5","\u1213\u1219\u1235","\u12d3\u122d\u1262","\u1240\u12f3\u121d"],
  NARROWWEEKDAYS:["\u1230","\u1230","\u1220","\u1228","\u1283","\u12d3","\u1240"],
  SHORTQUARTERS:["Q1","Q2","Q3","Q4"],
  QUARTERS:["Q1","Q2","Q3","Q4"],
  AMPMS:["\u1295\u1309\u1206 \u1230\u12d3\u1270","\u12f5\u1215\u122d \u1230\u12d3\u1275"],
  DATEFORMATS:["EEEE\u1361 dd MMMM \u1218\u12d3\u120d\u1272 yyyy G","dd MMMM yyyy","dd-MMM-yyyy","dd/MM/yy"],
  TIMEFORMATS:["h:mm:ss a v","h:mm:ss a z","h:mm:ss a","h:mm a"],
  FIRSTDAYOFWEEK: 5,
  WEEKENDRANGE: [5, 6],
  FIRSTWEEKCUTOFFDAY: 4
};
gadgets.i18n.DateTimeConstants.STANDALONENARROWMONTHS = gadgets.i18n.DateTimeConstants.NARROWMONTHS;
gadgets.i18n.DateTimeConstants.STANDALONEMONTHS = gadgets.i18n.DateTimeConstants.MONTHS;
gadgets.i18n.DateTimeConstants.STANDALONESHORTMONTHS = gadgets.i18n.DateTimeConstants.SHORTMONTHS;
gadgets.i18n.DateTimeConstants.STANDALONEWEEKDAYS = gadgets.i18n.DateTimeConstants.WEEKDAYS;
gadgets.i18n.DateTimeConstants.STANDALONESHORTWEEKDAYS = gadgets.i18n.DateTimeConstants.SHORTWEEKDAYS;
gadgets.i18n.DateTimeConstants.STANDALONENARROWWEEKDAYS = gadgets.i18n.DateTimeConstants.NARROWWEEKDAYS;
