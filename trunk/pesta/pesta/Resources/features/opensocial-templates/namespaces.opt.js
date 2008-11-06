os.nsmap_={};
os.nsurls_={};
os.createNamespace=function(C,B){var A=os.nsmap_[C];
if(!A){A={};
os.nsmap_[C]=A;
os.nsurls_[C]=B
}else{if(os.nsurls_[C]!=B){throw ("Namespace "+C+" already defined with url "+os.nsurls_[C])
}}return A
};
os.getNamespace=function(A){return os.nsmap_[A]
};
os.addNamespace=function(C,B,A){if(os.nsmap_[C]){throw ("Namespace '"+C+"' already exists!")
}os.nsmap_[C]=A;
os.nsurls_[C]=B
};
os.getCustomTag=function(C,B){var A=os.nsmap_[C];
if(!A){return null
}if(A.getTag){return A.getTag(B)
}else{return A[B]
}};
os.getRequiredNamespaces=function(A){var C="";
for(var B in os.nsurls_){if(A.indexOf("<"+B+":")>=0&&A.indexOf("xmlns:"+B+":")<0){C+=" xmlns:"+B+'="'+os.nsurls_[B]+'"'
}}return C
};
os.checkCustom_=function(D){var B;
if((B=D.indexOf(":"))<0){return null
}var C=D.substring(0,B);
var A=D.substring(B+1);
if(os.getCustomTag(C,A)){return[C,A]
}return null
};
os.defineBuiltinTags=function(){var C=os.getNamespace("os")||os.createNamespace("os","http://opensocial.com/#template");
C.Render=function(E,I,D){var J=D.getVariable(os.VAR_parentnode);
var G=E.getAttribute("content")||"*";
var L=os.getValueFromNode_(J,G);
if(!L){return""
}else{if(typeof (L)=="string"){var F=document.createTextNode(L);
L=[];
L.push(F)
}else{if(!isArray(L)){var K=[];
for(var H=0;
H<L.childNodes.length;
H++){K.push(L.childNodes[H])
}L=K
}else{if(G!="*"&&L.length==1&&L[0].nodeType==DOM_ELEMENT_NODE){var K=[];
for(var H=0;
H<L[0].childNodes.length;
H++){K.push(L[0].childNodes[H])
}L=K
}}}}return L
};
C.render=C.RenderAll=C.renderAll=C.Render;
C.Html=function(E){var D=E.code?""+E.code:E.getAttribute("code")||"";
return D
};
function B(D,E){return function(){E.apply(D)
}
}function A(H,G,I,D){var F=D.getVariable(os.VAR_callbacks);
var E=new Function(G);
F.push(B(H,E))
}os.registerAttribute("onAttach",A)
};
os.defineBuiltinTags();