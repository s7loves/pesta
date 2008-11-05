var gadgets=gadgets||{};
gadgets.flash=gadgets.flash||{};
gadgets.flash.getMajorVersion=function(){var C=0;
if(navigator.plugins&&navigator.mimeTypes&&navigator.mimeTypes.length){var B=navigator.plugins["Shockwave Flash"];
if(B&&B.description){C=parseInt(B.description.match(/[0-9]+/)[0],10)
}}else{for(var A=9;
A>0;
A--){try{new ActiveXObject("ShockwaveFlash.ShockwaveFlash."+A);
return A
}catch(D){}}}return C
};
gadgets.flash.embedFlash=function(D,J,I,C){switch(typeof J){case"string":J=document.getElementById(J);
case"object":if(J&&(typeof J.innerHTML=="string")){break
}default:return false
}switch(typeof C){case"undefined":C={};
case"object":break;
default:return false
}var F=gadgets.flash.getMajorVersion();
if(F){var K=parseInt(I,10);
if(isNaN(K)){K=0
}if(F>=K){if(!C.width){C.width="100%"
}if(!C.height){C.height="100%"
}if(typeof C.base!="string"){C.base=D.match(/^[^?#]+\//)[0]
}if(typeof C.wmode!="string"){C.wmode="opaque"
}var E;
if(navigator.plugins&&navigator.mimeTypes&&navigator.mimeTypes.length){C.type="application/x-shockwave-flash";
C.src=D;
E="<embed";
for(var B in C){if(!/^swf_/.test(B)){E+=" "+B+'="'+C[B]+'"'
}}E+=" /></embed>"
}else{C.movie=D;
var G={width:C.width,height:C.height,classid:"clsid:D27CDB6E-AE6D-11CF-96B8-444553540000"};
if(C.id){G.id=C.id
}E="<object";
for(var H in G){E+=" "+H+'="'+G[H]+'"'
}E+=">";
for(var A in C){if(!/^swf_/.test(A)&&!G[A]){E+='<param name="'+A+'" value="'+C[A]+'" />'
}}E+="</object>"
}J.innerHTML=E;
return true
}}return false
};
gadgets.flash.embedCachedFlash=function(){var A=Array.prototype.slice.call(arguments);
A[0]=gadgets.io.getProxyUrl(A[0]);
return gadgets.flash.embedFlash.apply(this,A)
};
var _IG_GetFlashMajorVersion=gadgets.flash.getMajorVersion;
var _IG_EmbedFlash=function(C,B,A){return gadgets.flash.embedFlash(C,B,A.swf_version,A)
};
var _IG_EmbedCachedFlash=function(C,B,A){return gadgets.flash.embedCachedFlash(C,B,A.swf_version,A)
};