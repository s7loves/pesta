var MAPS_DEBUG=false;function log(a){}var STRING_empty="";var CSS_display="display";var CSS_position="position";var TYPE_boolean="boolean";var TYPE_number="number";var TYPE_object="object";var TYPE_string="string";var TYPE_function="function";var TYPE_undefined="undefined";function jsEval(expr){try{return eval("["+expr+"][0]")}catch(e){log("EVAL FAILED "+expr+": "+e);return null}}function jsLength(a){return a.length}function assert(a){}function copyProperties(c,b){for(var a in b){c[a]=b[a]}}function getDefaultObject(b,a){if(typeof b!=TYPE_undefined&&b!=null){return(b)}else{return a}}function isArray(a){return a!=null&&typeof a==TYPE_object&&typeof a.length==TYPE_number}function arraySlice(c,b,a){return Function.prototype.call.apply(Array.prototype.slice,arguments)}function parseInt10(a){return parseInt(a,10)}function arrayClear(a){a.length=0}function bindFully(b,d,c){var a=arraySlice(arguments,2);return function(){return d.apply(b,a)}}var DOM_ELEMENT_NODE=1;var DOM_ATTRIBUTE_NODE=2;var DOM_TEXT_NODE=3;var DOM_CDATA_SECTION_NODE=4;var DOM_ENTITY_REFERENCE_NODE=5;var DOM_ENTITY_NODE=6;var DOM_PROCESSING_INSTRUCTION_NODE=7;var DOM_COMMENT_NODE=8;var DOM_DOCUMENT_NODE=9;var DOM_DOCUMENT_TYPE_NODE=10;var DOM_DOCUMENT_FRAGMENT_NODE=11;var DOM_NOTATION_NODE=12;function domGetElementById(a,b){return a.getElementById(b)}function domCreateElement(b,a){return b.createElement(a)}function domTraverseElements(b,c){var a=new DomTraverser(c);a.run(b)}function DomTraverser(a){this.callback_=a}DomTraverser.prototype.run=function(a){var b=this;b.queue_=[a];while(jsLength(b.queue_)){b.process_(b.queue_.shift())}};DomTraverser.prototype.process_=function(b){var a=this;a.callback_(b);for(var d=b.firstChild;d;d=d.nextSibling){if(d.nodeType==DOM_ELEMENT_NODE){a.queue_.push(d)}}};function domGetAttribute(b,a){return b.getAttribute(a)}function domSetAttribute(b,a,c){b.setAttribute(a,c)}function domRemoveAttribute(b,a){b.removeAttribute(a)}function domCloneNode(a){return a.cloneNode(true)}function domCloneElement(a){return(domCloneNode(a))}function ownerDocument(a){if(!a){return document}else{if(a.nodeType==DOM_DOCUMENT_NODE){return(a)}else{return a.ownerDocument||document}}}function domCreateTextNode(a,b){return a.createTextNode(b)}function domAppendChild(a,b){return a.appendChild(b)}function displayDefault(a){a.style[CSS_display]=""}function displayNone(a){a.style[CSS_display]="none"}function positionAbsolute(a){a.style[CSS_position]="absolute"}function domInsertBefore(a,b){return b.parentNode.insertBefore(a,b)}function domReplaceChild(a,b){return b.parentNode.replaceChild(a,b)}function domRemoveNode(a){return domRemoveChild(a.parentNode,a)}function domRemoveChild(a,b){return a.removeChild(b)}function stringTrim(a){return stringTrimRight(stringTrimLeft(a))}function stringTrimLeft(a){return a.replace(/^\s+/,"")}function stringTrimRight(a){return a.replace(/\s+$/,"")};