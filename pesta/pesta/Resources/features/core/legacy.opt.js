var JSON=gadgets.json;
var _IG_Prefs=gadgets.Prefs;
_IG_Prefs._parseURL=gadgets.Prefs.parseUrl;
function _IG_Fetch_wrapper(B,A){B(A.data)
}function _IG_FetchContent(B,E,C){var D=C||{};
if(D.refreshInterval){D.REFRESH_INTERVAL=D.refreshInterval
}else{D.REFRESH_INTERVAL=3600
}var A=gadgets.util.makeClosure(null,_IG_Fetch_wrapper,E);
gadgets.io.makeRequest(B,A,D)
}function _IG_FetchXmlContent(B,E,C){var D=C||{};
if(D.refreshInterval){D.REFRESH_INTERVAL=D.refreshInterval
}else{D.REFRESH_INTERVAL=3600
}D.CONTENT_TYPE="DOM";
var A=gadgets.util.makeClosure(null,_IG_Fetch_wrapper,E);
gadgets.io.makeRequest(B,A,D)
}function _IG_FetchFeedAsJSON(B,F,C,A,D){var E=D||{};
E.CONTENT_TYPE="FEED";
E.NUM_ENTRIES=C;
E.GET_SUMMARIES=A;
gadgets.io.makeRequest(B,function(G){if(G.errors){G.data=G.data||{};
if(G.errors&&G.errors.length>0){G.data.ErrorMsg=G.errors[0]
}}F(G.data)
},E)
}function _IG_GetCachedUrl(A){return gadgets.io.getProxyUrl(A)
}function _IG_GetImageUrl(A){return gadgets.io.getProxyUrl(A)
}function _IG_RegisterOnloadHandler(A){gadgets.util.registerOnLoadHandler(A)
}function _IG_Callback(B,C){var A=arguments;
return function(){var D=Array.prototype.slice.call(arguments);
B.apply(null,D.concat(Array.prototype.slice.call(A,1)))
}
}var _args=gadgets.util.getUrlParameters;
function _gel(A){return document.getElementById?document.getElementById(A):null
}function _gelstn(A){if(A==="*"&&document.all){return document.all
}return document.getElementsByTagName?document.getElementsByTagName(A):[]
}function _gelsbyregex(D,F){var C=_gelstn(D);
var E=[];
for(var B=0,A=C.length;
B<A;
++B){if(F.test(C[B].id)){E.push(C[B])
}}return E
}function _esc(A){return window.encodeURIComponent?encodeURIComponent(A):escape(A)
}function _unesc(A){return window.decodeURIComponent?decodeURIComponent(A):unescape(A)
}function _hesc(A){return gadgets.util.escapeString(A)
}function _striptags(A){return A.replace(/<\/?[^>]+>/g,"")
}function _trim(A){return A.replace(/^\s+|\s+$/g,"")
}function _toggle(A){A=_gel(A);
if(A!==null){if(A.style.display.length===0||A.style.display==="block"){A.style.display="none"
}else{if(A.style.display==="none"){A.style.display="block"
}}}}var _global_legacy_uidCounter=0;
function _uid(){return _global_legacy_uidCounter++
}function _min(B,A){return(B<A?B:A)
}function _max(B,A){return(B>A?B:A)
}function _exportSymbols(A,B){var H={};
for(var I=0,F=B.length;
I<F;
I+=2){H[B[I]]=B[I+1]
}var E=A.split(".");
var J=window;
for(var D=0,C=E.length-1;
D<C;
++D){var G={};
J[E[D]]=G;
J=G
}J[E[E.length-1]]=H
};