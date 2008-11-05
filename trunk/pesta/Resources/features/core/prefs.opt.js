var gadgets=gadgets||{};
(function(){var B=null;
var C={};
var E={};
var G="en";
var F="US";
var D=0;
function A(){var I=gadgets.util.getUrlParameters();
for(var H in I){if(I.hasOwnProperty(H)){if(H.indexOf("up_")===0&&H.length>3){C[H.substr(3)]=String(I[H])
}else{if(H==="country"){F=I[H]
}else{if(H==="lang"){G=I[H]
}else{if(H==="mid"){D=I[H]
}}}}}}}gadgets.Prefs=function(){if(!B){A();
B=this
}return B
};
gadgets.Prefs.setInternal_=function(I,J){if(typeof I==="string"){C[I]=J
}else{for(var H in I){if(I.hasOwnProperty(H)){C[H]=I[H]
}}}};
gadgets.Prefs.setMessages_=function(H){msgs=H
};
gadgets.Prefs.prototype.getString=function(H){return C[H]?gadgets.util.escapeString(C[H]):""
};
gadgets.Prefs.prototype.getInt=function(H){var I=parseInt(C[H],10);
return isNaN(I)?0:I
};
gadgets.Prefs.prototype.getFloat=function(H){var I=parseFloat(C[H]);
return isNaN(I)?0:I
};
gadgets.Prefs.prototype.getBool=function(H){var I=C[H];
if(I){return I==="true"||I===true||!!parseInt(I,10)
}return false
};
gadgets.Prefs.prototype.set=function(H,I){throw new Error("setprefs feature required to make this call.")
};
gadgets.Prefs.prototype.getArray=function(L){var M=C[L];
if(M){var H=M.split("|");
var I=gadgets.util.escapeString;
for(var K=0,J=H.length;
K<J;
++K){H[K]=I(H[K].replace(/%7C/g,"|"))
}return H
}return[]
};
gadgets.Prefs.prototype.setArray=function(H,I){throw new Error("setprefs feature required to make this call.")
};
gadgets.Prefs.prototype.getMsg=function(H){return msgs[H]||""
};
gadgets.Prefs.prototype.getCountry=function(){return F
};
gadgets.Prefs.prototype.getLang=function(){return G
};
gadgets.Prefs.prototype.getModuleId=function(){return D
}
})();