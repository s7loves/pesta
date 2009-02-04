var gadgets=gadgets||{};
gadgets.rpc=function(){var R="__cb";
var P="";
var d="__g2c_rpc";
var F="__c2g_rpc";
var H="GRPC____NIXVBS_wrapper";
var B="GRPC____NIXVBS_get_wrapper";
var Y="GRPC____NIXVBS_handle_message";
var O="GRPC____NIXVBS_create_channel";
var J={};
var C={};
var W=[];
var D={};
var U={};
var K={};
var M=0;
var e={};
var T={};
var E={};
var c={};
if(gadgets.util){c=gadgets.util.getUrlParameters()
}K[".."]=c.rpctoken||c.ifpctok||0;
function Z(){return typeof window.postMessage==="function"?"wpm":typeof document.postMessage==="function"?"dpm":window.ActiveXObject?"nix":navigator.product==="Gecko"?"fe":"ifpc"
}function b(){if(I==="dpm"||I==="wpm"){window.addEventListener("message",function(i){S(gadgets.json.parse(i.data))
},false)
}if(I==="nix"){if(typeof window[B]!=="unknown"){window[Y]=function(i){S(gadgets.json.parse(i))
};
window[O]=function(i,k,j){if(K[i]==j){J[i]=k
}};
var g="Class "+H+"\n Private m_Intended\nPrivate m_Auth\nPublic Sub SetIntendedName(name)\n If isEmpty(m_Intended) Then\nm_Intended = name\nEnd If\nEnd Sub\nPublic Sub SetAuth(auth)\n If isEmpty(m_Auth) Then\nm_Auth = auth\nEnd If\nEnd Sub\nPublic Sub SendMessage(data)\n "+Y+"(data)\nEnd Sub\nPublic Function GetAuthToken()\n GetAuthToken = m_Auth\nEnd Function\nPublic Sub CreateChannel(channel, auth)\n Call "+O+"(m_Intended, channel, auth)\nEnd Sub\nEnd Class\nFunction "+B+"(name, auth)\nDim wrap\nSet wrap = New "+H+"\nwrap.SetIntendedName name\nwrap.SetAuth auth\nSet "+B+" = wrap\nEnd Function";
try{window.execScript(g,"vbscript")
}catch(h){I="ifpc"
}}}}var I=Z();
b();
C[P]=function(){throw new Error("Unknown RPC service: "+this.s)
};
C[R]=function(h,g){var i=e[h];
if(i){delete e[h];
i(g)
}};
function N(h,g){if(T[h]){return 
}if(I==="fe"){try{var j=document.getElementById(h);
j[d]=function(l){S(gadgets.json.parse(l))
}
}catch(i){}}if(I==="nix"){try{var j=document.getElementById(h);
var k=window[B](h,g);
j.contentWindow.opener=k
}catch(i){}}T[h]=true
}function V(k){var m=gadgets.json.stringify;
var g=[];
for(var l=0,h=k.length;
l<h;
++l){g.push(encodeURIComponent(m(k[l])))
}return g.join("&")
}function S(h){if(h&&typeof h.s==="string"&&typeof h.f==="string"&&h.a instanceof Array){if(K[h.f]){if(K[h.f]!=h.t){throw new Error("Invalid auth token.")
}}if(h.c){h.callback=function(i){gadgets.rpc.call(h.f,R,null,h.c,i)
}
}var g=(C[h.s]||C[P]).apply(h,h.a);
if(h.c&&typeof g!="undefined"){gadgets.rpc.call(h.f,R,null,h.c,g)
}}}function f(g,j,m,k){try{if(m!=".."){var i=J[".."];
if(!i&&window.opener&&"GetAuthToken" in window.opener){i=window.opener;
if(i.GetAuthToken()==K[".."]){var h=K[".."];
i.CreateChannel(window[B]("..",h),h);
J[".."]=i;
window.opener=null
}}if(i){i.SendMessage(k);
return 
}}else{if(J[g]){J[g].SendMessage(k);
return 
}}}catch(l){}a(g,j,m,k)
}function A(h,i,n,j,l){try{if(n!=".."){var g=window.frameElement;
if(typeof g[d]==="function"){if(typeof g[d][F]!=="function"){g[d][F]=function(o){S(gadgets.json.parse(o))
}
}g[d](j);
return 
}}else{var m=document.getElementById(h);
if(typeof m[d]==="function"&&typeof m[d][F]==="function"){m[d][F](j);
return 
}}}catch(k){}a(h,i,n,j,l)
}function a(g,h,m,i,j){var l=gadgets.rpc.getRelayUrl(g);
if(!l){throw new Error("No relay file assigned for IFPC")
}var k=null;
if(U[g]){k=[l,"#",V([m,M,1,0,V([m,h,"","",m].concat(j))])].join("")
}else{k=[l,"#",g,"&",m,"@",M,"&1&0&",encodeURIComponent(i)].join("")
}L(k)
}function L(k){var h;
for(var g=W.length-1;
g>=0;
--g){var l=W[g];
try{if(l&&(l.recyclable||l.readyState==="complete")){l.parentNode.removeChild(l);
if(window.ActiveXObject){W[g]=l=null;
W.splice(g,1)
}else{l.recyclable=false;
h=l;
break
}}}catch(j){}}if(!h){h=document.createElement("iframe");
h.style.border=h.style.width=h.style.height="0px";
h.style.visibility="hidden";
h.style.position="absolute";
h.onload=function(){this.recyclable=true
};
W.push(h)
}h.src=k;
setTimeout(function(){document.body.appendChild(h)
},0)
}function G(h,j){if(typeof E[h]==="undefined"){E[h]=false;
var i=null;
if(h===".."){i=parent
}else{i=frames[h]
}try{E[h]=i.gadgets.rpc.receiveSameDomain
}catch(g){}}if(typeof E[h]==="function"){E[h](j);
return true
}return false
}if(gadgets.config){function X(g){if(g.rpc.parentRelayUrl.substring(0,7)==="http://"){D[".."]=g.rpc.parentRelayUrl
}else{var l=document.location.search.substring(0).split("&");
var k="";
for(var h=0,j;
j=l[h];
++h){if(j.indexOf("parent=")===0){k=decodeURIComponent(j.substring(7));
break
}}D[".."]=k+g.rpc.parentRelayUrl
}U[".."]=!!g.rpc.useLegacyProtocol
}var Q={parentRelayUrl:gadgets.config.NonEmptyStringValidator};
gadgets.config.register("rpc",Q,X)
}return{register:function(h,g){if(h==R){throw new Error("Cannot overwrite callback service")
}if(h==P){throw new Error("Cannot overwrite default service: use registerDefault")
}C[h]=g
},unregister:function(g){if(g==R){throw new Error("Cannot delete callback service")
}if(g==P){throw new Error("Cannot delete default service: use unregisterDefault")
}delete C[g]
},registerDefault:function(g){C[""]=g
},unregisterDefault:function(){delete C[""]
},call:function(n,j,o,m){++M;
n=n||"..";
if(o){e[M]=o
}var l="..";
if(n===".."){l=window.name
}var i={s:j,f:l,c:o?M:0,a:Array.prototype.slice.call(arguments,3),t:K[n]};
if(G(n,i)){return 
}var g=gadgets.json.stringify(i);
var h=I;
if(U[n]){h="ifpc"
}switch(h){case"dpm":var p=n===".."?parent.document:frames[n].document;
p.postMessage(g);
break;
case"wpm":var k=n===".."?parent:frames[n];
k.postMessage(g,D[n]);
break;
case"nix":f(n,j,l,g);
break;
case"fe":A(n,j,l,g,i.a);
break;
default:a(n,j,l,g,i.a);
break
}},getRelayUrl:function(g){return D[g]
},setRelayUrl:function(h,g,i){D[h]=g;
U[h]=!!i
},setAuthToken:function(g,h){K[g]=h;
N(g,h)
},getRelayChannel:function(){return I
},receive:function(g){if(g.length>4){S(gadgets.json.parse(decodeURIComponent(g[g.length-1])))
}},receiveSameDomain:function(g){g.a=Array.prototype.slice.call(g.a);
window.setTimeout(function(){S(g)
},0)
}}
}();