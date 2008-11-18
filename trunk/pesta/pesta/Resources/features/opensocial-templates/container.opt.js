os.Container={};
os.Container.inlineTemplates_=[];
os.Container.domLoadCallbacks_=null;
os.Container.domLoaded_=false;
os.Container.registerDomLoadListener_=function(){var A=window.gadgets;
if(A&&A.util){A.util.registerOnLoadHandler(os.Container.onDomLoad_)
}else{if(navigator.product=="Gecko"){window.addEventListener("DOMContentLoaded",os.Container.onDomLoad_,false)
}}if(window.addEventListener){window.addEventListener("load",os.Container.onDomLoad_,false)
}else{if(!document.body){setTimeout(arguments.callee,0);
return 
}var B=window.onload||function(){};
window.onload=function(){B();
os.Container.onDomLoad_()
}
}};
os.Container.onDomLoad_=function(){if(os.Container.domLoaded_){return 
}while(os.Container.domLoadCallbacks_.length){os.Container.domLoadCallbacks_.pop()()
}os.Container.domLoaded_=true
};
os.Container.executeOnDomLoad=function(A){if(os.Container.domLoaded_){setTimeout(A,0)
}else{if(os.Container.domLoadCallbacks_==null){os.Container.domLoadCallbacks_=[];
os.Container.registerDomLoadListener_()
}os.Container.domLoadCallbacks_.push(A)
}};
os.Container.registerDocumentTemplates=function(E){var F=E||document;
var B=F.getElementsByTagName(os.Container.TAG_script_);
for(var C=0;
C<B.length;
++C){var D=B[C];
if(os.Container.isTemplateType_(D.type)){var A=D.getAttribute("tag");
if(A){os.Container.registerTagElement_(D,A)
}else{if(D.getAttribute("name")){os.Container.registerTemplateElement_(D,D.getAttribute("name"))
}}}}};
os.Container.executeOnDomLoad(os.Container.registerDocumentTemplates);
os.Container.compileInlineTemplates=function(A,G){var H=G||document;
var B=H.getElementsByTagName(os.Container.TAG_script_);
for(var D=0;
D<B.length;
++D){var F=B[D];
if(os.Container.isTemplateType_(F.type)){var C=F.getAttribute("name")||F.getAttribute("tag");
if(!C||C.length<0){var E=os.compileTemplate(F);
if(E){os.Container.inlineTemplates_.push({template:E,node:F})
}else{os.warn("Failed compiling inline template.")
}}}}};
os.Container.renderInlineTemplates=function(F,G){var I=G||document;
var C=os.Container.inlineTemplates_;
for(var E=0;
E<C.length;
++E){var J=C[E].template;
var D=C[E].node;
var A="_T_"+J.id;
var B=I.getElementById(A);
if(!B){B=I.createElement("div");
B.setAttribute("id",A);
D.parentNode.insertBefore(B,D)
}var K=D.getAttribute("beforeData");
if(K){var L=K.split(/[\, ]+/);
os.data.DataContext.registerListener(L,os.createHideElementClosure(B))
}var H=D.getAttribute("requireData");
if(H){var L=H.split(/[\, ]+/);
os.data.DataContext.registerListener(L,os.createRenderClosure(J,B,os.data.DataContext))
}else{J.renderInto(B,F)
}}};
os.Container.registerTemplate=function(A){var B=document.getElementById(A);
return os.Container.registerTemplateElement_(B)
};
os.Container.registerTag=function(A){var B=document.getElementById(A);
os.Container.registerTagElement_(B,A)
};
os.Container.renderElement=function(B,D,A){var E=os.getTemplate(D);
if(E){var C=document.getElementById(B);
if(C){E.renderInto(C,A)
}else{os.warn("Element ("+B+") not found to render into.")
}}else{os.warn("Template ("+D+") not registered.")
}};
os.Container.loadDataRequests=function(D){var E=D||document;
var A=E.getElementsByTagName(os.Container.TAG_script_);
for(var B=0;
B<A.length;
++B){var C=A[B];
if(C.type==os.Container.dataType_){os.data.loadRequests(C)
}}os.data.executeRequests()
};
os.Container.processInlineTemplates=function(A,C){var B=A||os.data.DataContext;
os.Container.compileInlineTemplates(C);
os.Container.renderInlineTemplates(B,C)
};
os.Container.processDocument=function(A,B){os.Container.loadDataRequests(B);
os.Container.registerDocumentTemplates(B);
os.Container.processInlineTemplates(A,B)
};
os.Container.TAG_script_="script";
os.Container.templateTypes_={};
os.Container.templateTypes_["text/os-template"]=true;
os.Container.templateTypes_["text/template"]=true;
os.Container.dataType_="text/os-data";
os.Container.isTemplateType_=function(A){return os.Container.templateTypes_[A]!=null
};
os.Container.registerTemplateElement_=function(A,C){var B=os.compileTemplate(A,C);
if(B){os.registerTemplate(B)
}else{os.warn("Could not compile template ("+A.id+")")
}return B
};
os.Container.registerTagElement_=function(D,C){var E=os.Container.registerTemplateElement_(D);
if(E){var B=C.split(":");
var A=os.getNamespace(B[0]);
if(A){A[B[1]]=os.createTemplateCustomTag(E)
}else{os.warn("Namespace "+B[0]+" is not registered.")
}}};