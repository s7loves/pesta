var opensocial=opensocial||{};opensocial.xmlutil=opensocial.xmlutil||{};opensocial.xmlutil.parser_=null;opensocial.xmlutil.parseXML=function(b){if(typeof(DOMParser)!="undefined"){opensocial.xmlutil.parser_=opensocial.xmlutil.parser_||new DOMParser();var a=opensocial.xmlutil.parser_.parseFromString(b,"text/xml");if(a.firstChild&&a.firstChild.tagName=="parsererror"){throw a.firstChild.firstChild.nodeValue}return a}else{if(typeof(ActiveXObject)!="undefined"){var a=new ActiveXObject("MSXML2.DomDocument");a.validateOnParse=false;a.loadXML(b);if(a.parseError&&a.parseError.errorCode){throw a.parseError.reason}return a}}throw"No XML parser found in this browser."};opensocial.xmlutil.NSMAP={os:"http://opensocial.org/"};opensocial.xmlutil.getRequiredNamespaces=function(a){var c=[];for(var b in opensocial.xmlutil.NSMAP){if(a.indexOf("<"+b+":")>=0&&a.indexOf("xmlns:"+b+":")<0){c.push(" xmlns:");c.push(b);c.push('="');c.push(opensocial.xmlutil.NSMAP[b]);c.push('"')}}return c.join("")};opensocial.xmlutil.ENTITIES='<!ENTITY nbsp "&#160;">';opensocial.xmlutil.prepareXML=function(a){var b=opensocial.xmlutil.getRequiredNamespaces(a);return"<!DOCTYPE root ["+opensocial.xmlutil.ENTITIES+']><root xml:space="preserve"'+b+">"+a+"</root>"};