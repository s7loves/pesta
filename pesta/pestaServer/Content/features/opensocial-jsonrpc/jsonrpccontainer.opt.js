var JsonRpcContainer=function(e,g,f){opensocial.Container.call(this);var d={};for(var b in f){if(f.hasOwnProperty(b)){d[b]={};for(var c=0;c<f[b].length;c++){var a=f[b][c];d[b][a]=true}}}this.environment_=new opensocial.Environment(g,d);this.baseUrl_=e;this.securityToken_=shindig.auth.getSecurityToken();gadgets.rpc.register("shindig.requestShareApp_callback",JsonRpcContainer.requestShareAppCallback_)};(function(){var a={};JsonRpcContainer.inherits(opensocial.Container);JsonRpcContainer.prototype.getEnvironment=function(){return this.environment_};JsonRpcContainer.prototype.requestShareApp=function(f,h,c,d){var e="cId_"+Math.random();a[e]=c;var b=gadgets.util.unescapeString(h.getField(opensocial.Message.Field.BODY));if(!b||b.length===0){var g=gadgets.util.unescapeString(h.getField(opensocial.Message.Field.BODY_ID));b=gadgets.Prefs.getMsg(g)}gadgets.rpc.call("..","shindig.requestShareApp",null,e,f,b)};JsonRpcContainer.requestShareAppCallback_=function(f,g,c,e){callback=a[f];if(callback){a[f]=null;var d=null;if(e){d={recipientIds:e}}var b=new opensocial.ResponseItem(null,d,c);callback(b)}};JsonRpcContainer.prototype.requestCreateActivity=function(e,c,b){b=b||function(){};var d=opensocial.newDataRequest();var f=new opensocial.IdSpec({userId:"VIEWER"});d.add(this.newCreateActivityRequest(f,e),"key");d.send(function(g){b(g.get("key"))})};JsonRpcContainer.prototype.requestData=function(g,l){l=l||function(){};var e=g.getRequestObjects();var i=e.length;if(i===0){window.setTimeout(function(){l(new opensocial.DataResponse({},true))},0);return}var m=new Array(i);for(var f=0;f<i;f++){var k=e[f];m[f]=k.request.rpc;if(k.key){m[f].id=k.key}}var c=function(x){if(x.errors[0]){JsonRpcContainer.generateErrorResponse(x,e,l);return}x=x.data;var n=false;var w={};for(var r=0;r<x.length;r++){x[x[r].id]=x[r]}for(var o=0;o<e.length;o++){var q=e[o];var p=x[o];if(q.key&&p.id!=q.key){throw"Request key("+q.key+") and response id("+p.id+") do not match"}var j=p.data;var u=p.error;var t="";if(u){t=u.message}var s=q.request.processResponse(q.request,j,u,t);n=n||s.hadError();if(q.key){w[q.key]=s}}var v=new opensocial.DataResponse(w,n);l(v)};var h={CONTENT_TYPE:"JSON",METHOD:"POST",AUTHORIZATION:"SIGNED",POST_DATA:gadgets.json.stringify(m)};var b=[this.baseUrl_,"/rpc"];var d=shindig.auth.getSecurityToken();if(d){b.push("?st=",encodeURIComponent(d))}this.sendRequest(b.join(""),c,h,"application/json")};JsonRpcContainer.prototype.sendRequest=function(b,e,c,d){gadgets.io.makeNonProxiedRequest(b,e,c,d)};JsonRpcContainer.generateErrorResponse=function(b,e,g){var c=JsonRpcContainer.translateHttpError(b.errors[0]||b.data.error)||opensocial.ResponseItem.Error.INTERNAL_ERROR;var f={};for(var d=0;d<e.length;d++){f[e[d].key]=new opensocial.ResponseItem(e[d].request,null,c)}g(new opensocial.DataResponse(f,true))};JsonRpcContainer.translateHttpError=function(b){if(b=="Error 501"){return opensocial.ResponseItem.Error.NOT_IMPLEMENTED}else{if(b=="Error 401"){return opensocial.ResponseItem.Error.UNAUTHORIZED}else{if(b=="Error 403"){return opensocial.ResponseItem.Error.FORBIDDEN}else{if(b=="Error 400"){return opensocial.ResponseItem.Error.BAD_REQUEST}else{if(b=="Error 500"){return opensocial.ResponseItem.Error.INTERNAL_ERROR}else{if(b=="Error 404"){return opensocial.ResponseItem.Error.BAD_REQUEST}else{if(b=="Error 417"){return opensocial.ResponseItem.Error.LIMIT_EXCEEDED}}}}}}}};JsonRpcContainer.prototype.makeIdSpec=function(b){return new opensocial.IdSpec({userId:b})};JsonRpcContainer.prototype.translateIdSpec=function(b){var e=b.getField("userId");var d=b.getField("groupId");if(!opensocial.Container.isArray(e)){e=[e]}for(var c=0;c<e.length;c++){if(e[c]=="OWNER"){e[c]="@owner"}else{if(e[c]=="VIEWER"){e[c]="@viewer"}}}if(d=="FRIENDS"){d="@friends"}else{if(d=="SELF"||!d){d="@self"}}return{userId:e,groupId:d}};JsonRpcContainer.prototype.newFetchPersonRequest=function(e,d){var b=this.newFetchPeopleRequest(this.makeIdSpec(e),d);var c=this;return new JsonRpcRequestItem(b.rpc,function(f){return c.createPersonFromJson(f)})};JsonRpcContainer.prototype.newFetchPeopleRequest=function(b,d){var e={method:"people.get"};e.params=this.translateIdSpec(b);if(d.profileDetail){FieldTranslations.translateJsPersonFieldsToServerFields(d.profileDetail);e.params.fields=d.profileDetail}if(d.first){e.params.startIndex=d.first}if(d.max){e.params.count=d.max}if(d.sortOrder){e.params.sortBy=d.sortOrder}if(d.filter){e.params.filterBy=d.filter}if(b.getField("networkDistance")){e.params.networkDistance=b.getField("networkDistance")}var c=this;return new JsonRpcRequestItem(e,function(j){var h;if(j.list){h=j.list}else{h=[j]}var g=[];for(var f=0;f<h.length;f++){g.push(c.createPersonFromJson(h[f]))}return new opensocial.Collection(g,j.startIndex,j.totalResults)})};JsonRpcContainer.prototype.createPersonFromJson=function(b){FieldTranslations.translateServerPersonToJsPerson(b);return new JsonPerson(b)};JsonRpcContainer.prototype.getFieldsList=function(b){if(this.hasNoKeys(b)||this.isWildcardKey(b[0])){return[]}else{return b}};JsonRpcContainer.prototype.hasNoKeys=function(b){return !b||b.length===0};JsonRpcContainer.prototype.isWildcardKey=function(b){return b=="*"};JsonRpcContainer.prototype.newFetchPersonAppDataRequest=function(b,d,c){var e={method:"appdata.get"};e.params=this.translateIdSpec(b);e.params.appId="@app";e.params.fields=this.getFieldsList(d);if(b.getField("networkDistance")){e.params.networkDistance=b.getField("networkDistance")}return new JsonRpcRequestItem(e,function(f){return opensocial.Container.escape(f,c,true)})};JsonRpcContainer.prototype.newUpdatePersonAppDataRequest=function(e,b,c){var d={method:"appdata.update"};d.params=this.translateIdSpec(this.makeIdSpec(e));d.params.appId="@app";d.params.data={};d.params.data[b]=c;d.params.fields=b;return new JsonRpcRequestItem(d)};JsonRpcContainer.prototype.newRemovePersonAppDataRequest=function(d,b){var c={method:"appdata.delete"};c.params=this.translateIdSpec(this.makeIdSpec(d));c.params.appId="@app";c.params.fields=this.getFieldsList(b);return new JsonRpcRequestItem(c)};JsonRpcContainer.prototype.newFetchActivitiesRequest=function(b,c){var d={method:"activities.get"};d.params=this.translateIdSpec(b);d.params.appId="@app";if(b.getField("networkDistance")){d.params.networkDistance=b.getField("networkDistance")}return new JsonRpcRequestItem(d,function(f){f=f.list;var g=[];for(var e=0;e<f.length;e++){g.push(new JsonActivity(f[e]))}return new opensocial.Collection(g)})};JsonRpcContainer.prototype.newActivity=function(b){return new JsonActivity(b,true)};JsonRpcContainer.prototype.newMediaItem=function(d,b,c){c=c||{};c.mimeType=d;c.url=b;return new JsonMediaItem(c)};JsonRpcContainer.prototype.newCreateActivityRequest=function(b,c){var d={method:"activities.create"};d.params=this.translateIdSpec(b);d.params.appId="@app";if(b.getField("networkDistance")){d.params.networkDistance=b.getField("networkDistance")}d.params.activity=c.toJsonObject();return new JsonRpcRequestItem(d)}})();JsonRpcContainer.prototype.newMessage=function(a,b){return new JsonMessage(a,b)};JsonRpcContainer.prototype.newMessageCollection=function(a){return new JsonMessageCollection(a)};JsonRpcContainer.prototype.newFetchMessageCollectionsRequest=function(a,b){var c={method:"messages.get"};c.params=this.translateIdSpec(a);return new JsonRpcRequestItem(c,function(e){e=e.list;var f=[];for(var d=0;d<e.length;d++){f.push(new JsonMessageCollection(e[d]))}return new opensocial.Collection(f)})};JsonRpcContainer.prototype.newFetchMessagesRequest=function(a,c,b){var d={method:"messages.get"};d.params=this.translateIdSpec(a);d.params.msgCollId=c;return new JsonRpcRequestItem(d,function(g){g=g.list;var f=[];for(var e=0;e<g.length;e++){f.push(new JsonMessage(g[e]))}return new opensocial.Collection(f)})};var JsonRpcRequestItem=function(b,a){this.rpc=b;this.processData=a||function(c){return c};this.processResponse=function(c,f,e,d){var g=e?JsonRpcContainer.translateHttpError("Error "+e.code):null;return new opensocial.ResponseItem(c,e?null:this.processData(f),g,d)}};