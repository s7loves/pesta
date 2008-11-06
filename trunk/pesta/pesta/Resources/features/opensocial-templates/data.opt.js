window.opensocial.data=window.opensocial.data||{};
var gadgets=window.gadgets;
os.data=window.opensocial.data;
os.data.requests_={};
os.data.registerRequestHandler=function(B,D){var A=B.split(":");
var C=os.getNamespace(A[0]);
if(!C){throw"Namespace "+A[0]+" is undefined."
}else{if(C[A[1]]){throw"Request handler "+A[1]+" is already defined."
}}C[A[1]]=D
};
os.data.loadRequests=function(A){if(typeof (A)!="string"){var B=A;
A=B.value||B.innerHTML
}A=os.prepareTemplateXML_(A);
var C=os.parseXML_(A);
var B=C.firstChild;
while(B.nodeType!=DOM_ELEMENT_NODE){B=B.nextSibling
}os.data.processDataNode_(B)
};
os.data.executeRequests=function(){for(var F in os.data.requests_){var B=os.data.requests_[F];
if(F=="os"){var D=opensocial.newDataRequest();
for(var C in B){var A=B[C];
var E=os.getCustomTag("os",A.tagParts[1]);
D.add(E(D,A),C)
}D.send(function(G){if(!G||G.hadError()){throw"Unexpected error with OpenSocial data request."
}else{for(var I in B){var H=G.get(I);
if(!H){throw"Request for "+I+" could not be loaded."
}else{if(H.hadError()){throw"Response error("+H.getErrorCode()+"): "+H.getErrorMessage()
}else{os.data.DataContext.putDataSet(I,G)
}}}}})
}else{for(var C in B){var A=B[C];
var E=os.getCustomTag(F,A.tagParts[1]);
E(C,A)
}}}delete os.data.requests_;
os.data.requests_={}
};
os.data.processDataNode_=function(A){for(var B=A.firstChild;
B;
B=B.nextSibling){if(B.nodeType==DOM_ELEMENT_NODE){if(B.tagName=="os:dataSet"){os.data.processDataSet_(B)
}}}};
os.data.processDataSet_=function(D){for(var E=D.firstChild;
E;
E=E.nextSibling){if(E.nodeType==DOM_ELEMENT_NODE){var C=D.getAttribute("key");
var A=new os.data.DataRequestTag(E);
if(!os.data.requests_[A.tagParts[0]]){os.data.requests_[A.tagParts[0]]={}
}var B=os.data.requests_[A.tagParts[0]];
B[C]=A;
break
}}};
os.data.listeners_=[];
os.data.checkListener_=function(B){for(var A in B.keys){if(os.data.DataContext[A]==null){return false
}}return true
};
os.data.DataRequestTag=function(C){this.tagName=C.tagName;
this.tagParts=this.tagName.split(":");
this.attributes={};
for(var B=0;
B<C.attributes.length;
++B){var A=C.attributes[B].nodeName;
if(A){var D=C.getAttribute(A);
if(A&&D){this.attributes[A]=D
}}}};
os.data.DataRequestTag.prototype.hasAttribute=function(A){return !!this.attributes[A]
};
os.data.DataRequestTag.prototype.getAttribute=function(B){var A=this.attributes[B];
if(!A){return A
}var C=os.parseAttribute_(A);
if(!C){return A
}return os.data.DataContext.evalExpression(C)
};
os.data.DataContext={};
os.data.DataContext.registerListener=function(B,D){var C={};
C.keys={};
if(typeof (B)=="object"){for(var A in B){C.keys[B[A]]=true
}}else{C.keys[B]=true
}C.callback=D;
os.data.listeners_.push(C);
if(os.data.checkListener_(C)){window.setTimeout(function(){C.callback()
},1)
}};
os.data.DataContext.getDataSet=function(A){return os.data.DataContext[A]
};
os.data.DataContext.putDataSet=function(B,D){var C=D;
if(typeof (C)=="undefined"||C===null){return 
}if(D.get){var A=D.get(B);
if(A&&A.getData){C=A.getData();
C=C.array_||C
}}os.data.DataContext[B]=C;
os.data.fireCallbacks_(B)
};
os.data.DataContext.putDataResult=function(A,B){B(function(C){os.data.DataContext.putDataSet(A,C)
})
};
os.data.DataContext.evalContext_=os.createContext(os.data.DataContext);
os.data.DataContext.evalExpression=function(A){return os.data.DataContext.evalContext_.evalExpression(A)
};
os.data.fireCallbacks_=function(B){for(var A=0;
A<os.data.listeners_.length;
++A){var C=os.data.listeners_[A];
if(C.keys[B]!=null){if(os.data.checkListener_(C)){C.callback()
}}}};
os.createRenderClosure=function(C,B,A){var D=function(){C.renderInto(B,A)
};
return D
};
os.createHideElementClosure=function(A){var B=function(){displayNone(A)
};
return B
};
os.data.newJsonPostRequestHandler=function(B,A){var C=function(E){if(!gadgets){return 
}var D={};
D[gadgets.io.RequestParameters.METHOD]=gadgets.io.MethodType.POST;
if(A){D[gadgets.io.RequestParameters.POST_DATA]=A
}D[gadgets.io.RequestParameters.CONTENT_TYPE]=gadgets.io.ContentType.JSON;
gadgets.io.makeRequest(B,function(F){E(F.data)
},D)
};
return C
};
(os.data.defineRequests_=function(){os.data.registerRequestHandler("os:personRequest",function(B,A){return B.newFetchPersonRequest(A.getAttribute("id"))
});
os.data.registerRequestHandler("os:peopleRequest",function(B,A){return B.newFetchPeopleRequest(A.getAttribute("group"))
});
if(gadgets){os.createNamespace("json","http://json.org");
os.data.registerRequestHandler("json:makeRequest",function(C,A){var B=A.getAttribute("url");
var D={};
D[gadgets.io.RequestParameters.CONTENT_TYPE]=gadgets.io.ContentType.JSON;
D[gadgets.io.RequestParameters.METHOD]=gadgets.io.MethodType.GET;
gadgets.io.makeRequest(B,function(E){os.data.DataContext.putDataSet(C,E.data)
},D)
})
}})();
(os.data.populateParams_=function(){var C={};
var E=document.location.search;
if(E){E=E.substring(1);
var B=E.split("&");
for(var A=0;
A<B.length;
A++){var D=B[A].split("=");
C[D[0]]=D[1]
}}os.data.DataContext.putDataSet("params",C)
})();