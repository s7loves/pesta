var JsonRpcContainer=function(E,G,F){opensocial.Container.call(this);
var D={};
for(var B in F){if(F.hasOwnProperty(B)){D[B]={};
for(var C=0;
C<F[B].length;
C++){var A=F[B][C];
D[B][A]=true
}}}this.environment_=new opensocial.Environment(G,D);
this.baseUrl_=E;
this.securityToken_=shindig.auth.getSecurityToken()
};
JsonRpcContainer.inherits(opensocial.Container);
JsonRpcContainer.prototype.getEnvironment=function(){return this.environment_
};
JsonRpcContainer.prototype.requestCreateActivity=function(D,B,A){A=A||function(){};
var C=opensocial.newDataRequest();
var E=new opensocial.IdSpec({userId:"VIEWER"});
C.add(this.newCreateActivityRequest(E,D),"key");
C.send(function(F){A(F.get("key"))
})
};
JsonRpcContainer.prototype.requestData=function(D,H){H=H||function(){};
var B=D.getRequestObjects();
var F=B.length;
if(F==0){window.setTimeout(function(){H(new opensocial.DataResponse({},true))
},0);
return 
}var I=new Array(F);
for(var C=0;
C<F;
C++){var G=B[C];
I[C]=G.request.rpc;
if(G.key){I[C].id=G.key
}}var A=function(U){if(U.errors[0]){JsonRpcContainer.generateErrorResponse(U,B,H);
return 
}U=U.data;
var K=false;
var T={};
for(var O=0;
O<U.length;
O++){U[U[O].id]=U[O]
}for(var L=0;
L<B.length;
L++){var N=B[L];
var M=U[L];
if(N.key&&M.id!=N.key){throw"Request key("+N.key+") and response id("+M.id+") do not match"
}var J=M.data;
var R=M.error;
var Q="";
if(R){Q=R.message
}var P=N.request.processResponse(N.request,J,R,Q);
K=K||P.hadError();
if(N.key){T[N.key]=P
}}var S=new opensocial.DataResponse(T,K);
H(S)
};
var E={CONTENT_TYPE:"JSON",METHOD:"POST",AUTHORIZATION:"SIGNED",POST_DATA:gadgets.json.stringify(I)};
this.sendRequest(this.baseUrl_+"/rpc?st="+encodeURIComponent(shindig.auth.getSecurityToken()),A,E,"application/json")
};
JsonRpcContainer.prototype.sendRequest=function(A,D,B,C){gadgets.io.makeNonProxiedRequest(A,D,B,C)
};
JsonRpcContainer.generateErrorResponse=function(A,D,F){var B=JsonRpcContainer.translateHttpError(A.errors[0]||A.data.error)||opensocial.ResponseItem.Error.INTERNAL_ERROR;
var E={};
for(var C=0;
C<D.length;
C++){E[D[C].key]=new opensocial.ResponseItem(D[C].request,null,B)
}F(new opensocial.DataResponse(E,true))
};
JsonRpcContainer.translateHttpError=function(A){if(A=="Error 501"){return opensocial.ResponseItem.Error.NOT_IMPLEMENTED
}else{if(A=="Error 401"){return opensocial.ResponseItem.Error.UNAUTHORIZED
}else{if(A=="Error 403"){return opensocial.ResponseItem.Error.FORBIDDEN
}else{if(A=="Error 400"){return opensocial.ResponseItem.Error.BAD_REQUEST
}else{if(A=="Error 500"){return opensocial.ResponseItem.Error.INTERNAL_ERROR
}else{if(A=="Error 404"){return opensocial.ResponseItem.Error.BAD_REQUEST
}else{if(A=="Error 417"){return opensocial.ResponseItem.Error.LIMIT_EXCEEDED
}}}}}}}};
JsonRpcContainer.prototype.makeIdSpec=function(A){return new opensocial.IdSpec({userId:A})
};
JsonRpcContainer.prototype.translateIdSpec=function(A){var D=A.getField("userId");
var C=A.getField("groupId");
if(!opensocial.Container.isArray(D)){D=[D]
}for(var B=0;
B<D.length;
B++){if(D[B]=="OWNER"){D[B]="@owner"
}else{if(D[B]=="VIEWER"){D[B]="@viewer"
}}}if(C=="FRIENDS"){C="@friends"
}else{if(C=="SELF"||!C){C="@self"
}}return{userId:D,groupId:C}
};
JsonRpcContainer.prototype.newFetchPersonRequest=function(D,C){var A=this.newFetchPeopleRequest(this.makeIdSpec(D),C);
var B=this;
return new JsonRpcRequestItem(A.rpc,function(E){return B.createPersonFromJson(E)
})
};
JsonRpcContainer.prototype.newFetchPeopleRequest=function(A,C){var D={method:"people.get"};
D.params=this.translateIdSpec(A);
if(C.profileDetail){FieldTranslations.translateJsPersonFieldsToServerFields(C.profileDetail);
D.params.fields=C.profileDetail
}if(C.first){D.params.startIndex=C.first
}if(C.max){D.params.count=C.max
}if(C.sortOrder){D.params.sortBy=C.sortOrder
}if(C.filter){D.params.filterBy=C.filter
}if(A.getField("networkDistance")){D.params.networkDistance=A.getField("networkDistance")
}var B=this;
return new JsonRpcRequestItem(D,function(H){var G;
if(H.list){G=H.list
}else{G=[H]
}var F=[];
for(var E=0;
E<G.length;
E++){F.push(B.createPersonFromJson(G[E]))
}return new opensocial.Collection(F,H.startIndex,H.totalResults)
})
};
JsonRpcContainer.prototype.createPersonFromJson=function(A){FieldTranslations.translateServerPersonToJsPerson(A);
return new JsonPerson(A)
};
JsonRpcContainer.prototype.getFieldsList=function(A){if(this.hasNoKeys(A)||this.isWildcardKey(A[0])){return[]
}else{return A
}};
JsonRpcContainer.prototype.hasNoKeys=function(A){return !A||A.length==0
};
JsonRpcContainer.prototype.isWildcardKey=function(A){return A=="*"
};
JsonRpcContainer.prototype.newFetchPersonAppDataRequest=function(A,C,B){var D={method:"appdata.get"};
D.params=this.translateIdSpec(A);
D.params.appId="@app";
D.params.fields=this.getFieldsList(C);
if(A.getField("networkDistance")){D.params.networkDistance=A.getField("networkDistance")
}return new JsonRpcRequestItem(D,function(E){return opensocial.Container.escape(E,B,true)
})
};
JsonRpcContainer.prototype.newUpdatePersonAppDataRequest=function(D,A,B){var C={method:"appdata.update"};
C.params=this.translateIdSpec(this.makeIdSpec(D));
C.params.appId="@app";
C.params.data={};
C.params.data[A]=B;
C.params.fields=A;
return new JsonRpcRequestItem(C)
};
JsonRpcContainer.prototype.newRemovePersonAppDataRequest=function(C,A){var B={method:"appdata.delete"};
B.params=this.translateIdSpec(this.makeIdSpec(C));
B.params.appId="@app";
B.params.fields=this.getFieldsList(A);
return new JsonRpcRequestItem(B)
};
JsonRpcContainer.prototype.newFetchActivitiesRequest=function(A,B){var C={method:"activities.get"};
C.params=this.translateIdSpec(A);
C.params.appId="@app";
if(A.getField("networkDistance")){C.params.networkDistance=A.getField("networkDistance")
}return new JsonRpcRequestItem(C,function(E){E=E.list;
var F=[];
for(var D=0;
D<E.length;
D++){F.push(new JsonActivity(E[D]))
}return new opensocial.Collection(F)
})
};
JsonRpcContainer.prototype.newActivity=function(A){return new JsonActivity(A,true)
};
JsonRpcContainer.prototype.newMediaItem=function(C,A,B){B=B||{};
B.mimeType=C;
B.url=A;
return new JsonMediaItem(B)
};
JsonRpcContainer.prototype.newCreateActivityRequest=function(A,B){var C={method:"activities.create"};
C.params=this.translateIdSpec(A);
C.params.appId="@app";
if(A.getField("networkDistance")){C.params.networkDistance=A.getField("networkDistance")
}C.params.activity=B.toJsonObject();
return new JsonRpcRequestItem(C)
};
var JsonRpcRequestItem=function(B,A){this.rpc=B;
this.processData=A||function(C){return C
};
this.processResponse=function(C,F,E,D){var G=E?JsonRpcContainer.translateHttpError("Error "+E.code):null;
return new opensocial.ResponseItem(C,E?null:this.processData(F),G,D)
}
};