attachDocumentStub = (function() {
    function arrayRemove(array, from, to) { var rest = array.slice((to || from) + 1 || array.length); array.length = from < 0 ? array.length + from : from; return array.push.apply(array, rest); }
    var tameNodeTrademark = {}; var tameEventTrademark = {}; function Html(htmlFragment) { this.html___ = String(htmlFragment || ''); }
    Html.prototype.valueOf = Html.prototype.toString = function() { return this.html___; }; function safeHtml(htmlFragment) { return (htmlFragment instanceof Html) ? htmlFragment.html___ : html.escapeAttrib(String(htmlFragment || '')); }
    function blessHtml(htmlFragment) { return (htmlFragment instanceof Html) ? htmlFragment : new Html(htmlFragment); }
    var XML_SPACE = '\t\n\r '; var XML_NAME_PATTERN = new RegExp('^[' + unicode.LETTER + '_:][' + unicode.LETTER + unicode.DIGIT + '.\\-_:'
+ unicode.COMBINING_CHAR + unicode.EXTENDER + ']*$'); var XML_NMTOKEN_PATTERN = new RegExp('^[' + unicode.LETTER + unicode.DIGIT + '.\\-_:'
+ unicode.COMBINING_CHAR + unicode.EXTENDER + ']+$'); var XML_NMTOKENS_PATTERN = new RegExp('^(?:[' + XML_SPACE + ']*[' + unicode.LETTER + unicode.DIGIT + '.\\-_:'
+ unicode.COMBINING_CHAR + unicode.EXTENDER + ']+)+[' + XML_SPACE + ']*$'); var JS_SPACE = '\t\n\r '; var JS_IDENT = '(?:[a-zA-Z_][a-zA-Z0-9$_]*[a-zA-Z0-9$]|[a-zA-Z])_?'; var SIMPLE_HANDLER_PATTERN = new RegExp('^[' + JS_SPACE + ']*'
+ '(return[' + JS_SPACE + ']+)?'
+ '(' + JS_IDENT + ')[' + JS_SPACE + ']*'
+ '\\((?:this'
+ '(?:[' + JS_SPACE + ']*,[' + JS_SPACE + ']*event)?'
+ '[' + JS_SPACE + ']*)?\\)'
+ '[' + JS_SPACE + ']*(?:;?[' + JS_SPACE + ']*)$'); function isXmlName(s) { return XML_NAME_PATTERN.test(s); }
    function isXmlNmTokens(s) { return XML_NMTOKENS_PATTERN.test(s); }
    function trimCssSpaces(input) { return input.replace(/^[ \t\r\n\f]+|[ \t\r\n\f]+$/g, ''); }
    function sanitizeStyleAttrValue(styleAttrValue) {
        var sanitizedDeclarations = []; var declarations = styleAttrValue.split(/;/g); for (var i = 0; declarations && i < declarations.length; i++) { var parts = declarations[i].split(':'); var property = trimCssSpaces(parts[0]).toLowerCase(); var value = trimCssSpaces(parts.slice(1).join(":")); var stylePropertyName = property.replace(/-[a-z]/g, function(m) { return m.substring(1).toUpperCase(); }); if (css.properties.hasOwnProperty(stylePropertyName) && css.properties[stylePropertyName].test(value + ' ')) { sanitizedDeclarations.push(property + ': ' + value); } }
        return sanitizedDeclarations.join(' ; ');
    }
    function mimeTypeForAttr(tagName, attribName) {
        if (tagName === 'img' && attribName === 'src') { return 'image/*'; }
        return '*/*';
    }
    functionassert(cond) { if (!cond) { (typeof console !== 'undefined') && (console.log('domita assertion failed'), console.trace()); throw new Error(); } }
    function exportFields(object, fields) {
        for (var i = fields.length; --i >= 0; ) {
            var field = fields[i]; var fieldUCamel = field.charAt(0).toUpperCase() + field.substring(1); var getterName = 'get' + fieldUCamel; var setterName = 'set' + fieldUCamel; var count = 0; if (object[getterName]) { ++count; ___.useGetHandler(object, field, object[getterName]); }
            if (object[setterName]) { ++count; ___.useSetHandler(object, field, object[setterName]); }
            if (!count) { throw new Error('Failed to export field ' + field + ' on ' + object); } 
        } 
    }
    function extend(subClass, baseClass) { var noop = function() { }; noop.prototype = baseClass.prototype; subClass.prototype = new noop(); subClass.prototype.constructor = subClass; }
    var cssSealerUnsealerPair = cajita.makeSealerUnsealerPair(); var timeoutIdTrademark = {}; function tameSetTimeout(timeout, delayMillis) { var timeoutId = setTimeout(function() { ___.callPub(timeout, 'call', [___.USELESS]); }, delayMillis | 0); return ___.freeze(___.stamp(timeoutIdTrademark, { timeoutId___: timeoutId })); }
    ___.frozenFunc(tameSetTimeout); function tameClearTimeout(timeoutId) { ___.guard(timeoutIdTrademark, timeoutId); clearTimeout(timeoutId.timeoutId___); }
    ___.frozenFunc(tameClearTimeout); var intervalIdTrademark = {}; function tameSetInterval(interval, delayMillis) { var intervalId = setInterval(function() { ___.callPub(interval, 'call', [___.USELESS]); }, delayMillis | 0); return ___.freeze(___.stamp(intervalIdTrademark, { intervalId___: intervalId })); }
    ___.frozenFunc(tameSetInterval); function tameClearInterval(intervalId) { ___.guard(intervalIdTrademark, intervalId); clearInterval(intervalId.intervalId___); }
    ___.frozenFunc(tameClearInterval); function attachDocumentStub(idSuffix, uriCallback, imports) {
        var elementPolicies = {}; elementPolicies.form = function(attribs) {
            var sawHandler = false; for (var i = 0, n = attribs.length; i < n; i += 2) { if (attribs[i] === 'onsubmit') { sawHandler = true; } }
            if (!sawHandler) { attribs.push('onsubmit', 'return false'); }
            return attribs;
        }; elementPolicies.a = elementPolicies.area = function(attribs) { attribs.push('target', '_blank'); return attribs; }; function sanitizeHtml(htmlText) { var out = []; htmlSanitizer(htmlText, out); return out.join(''); }
        var htmlSanitizer = html.makeHtmlSanitizer(function sanitizeAttributes(tagName, attribs) {
            for (var i = 0; i < attribs.length; i += 2) {
                var attribName = attribs[i]; var value = attribs[i + 1]; var atype = null, attribKey; if ((attribKey = tagName + ':' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey))) { atype = html4.ATTRIBS[attribKey]; value = rewriteAttribute(tagName, attribName, atype, value); } else { value = null; }
                if (value !== null && value !== void 0) { attribs[i + 1] = value; } else { attribs.splice(i, 2); i -= 2; } 
            }
            var policy = elementPolicies[tagName]; if (policy && elementPolicies.hasOwnProperty(tagName)) { return policy(attribs); }
            return attribs;
        }); function tameInnerHtml(htmlText) { var out = []; innerHtmlTamer(htmlText, out); return out.join(''); }
        var innerHtmlTamer = html.makeSaxParser({ startTag: function(tagName, attribs, out) {
            out.push('<', tagName); for (var i = 0; i < attribs.length; i += 2) {
                var attribName = attribs[i]; if (attribName === 'target') { continue; }
                var attribKey; var atype; if ((attribKey = tagName + ':' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey))) { atype = html4.ATTRIBS[attribKey]; } else { return ''; }
                var value = attribs[i + 1]; switch (atype) {
                    case html4.atype.ID: case html4.atype.IDREF: case html4.atype.IDREFS: if (value.length <= idSuffix.length || (idSuffix !== value.substring(value.length - idSuffix.length))) { continue; }
                        value = value.substring(0, value.length - idSuffix.length); break;
                }
                if (value !== null) { out.push(' ', attribName, '="', html.escapeAttrib(value), '"'); } 
            }
            out.push('>');
        }, endTag: function(name, out) { out.push('</', name, '>'); }, pcdata: function(text, out) { out.push(text); }, rcdata: function(text, out) { out.push(text); }, cdata: function(text, out) { out.push(text); } 
        }); var illegalSuffix = /__(?:\s|$)/; function rewriteAttribute(tagName, attribName, type, value) {
            switch (type) {
                case html4.atype.ID: case html4.atype.IDREF: case html4.atype.IDREFS: value = String(value); if (value && !illegalSuffix.test(value) && isXmlName(value)) { return value + idSuffix; }
                    return null; case html4.atype.CLASSES: case html4.atype.GLOBAL_NAME: case html4.atype.LOCAL_NAME: value = String(value); if (value && !illegalSuffix.test(value) && isXmlNmTokens(value)) { return value; }
                    return null; case html4.atype.SCRIPT: value = String(value); var match = value.match(SIMPLE_HANDLER_PATTERN); if (!match) { return null; }
                    var doesReturn = match[1]; var fnName = match[2]; var pluginId = ___.getId(imports); value = (doesReturn ? 'return ' : '') + 'plugin_dispatchEvent___('
+ 'this, event, ' + pluginId + ', "'
+ fnName + '");'; if (attribName === 'onsubmit') { value = 'try { ' + value + ' } finally { return false; }'; }
                    return value; case html4.atype.URI: value = String(value); if (!uriCallback) { return null; }
                    return uriCallback.rewrite(value, mimeTypeForAttr(tagName, attribName)) || null; case html4.atype.STYLE: if ('function' !== typeof value) { return sanitizeStyleAttrValue(String(value)); }
                    var cssPropertiesAndValues = cssSealerUnsealerPair.unseal(value); if (!cssPropertiesAndValues) { return null; }
                    var css = []; for (var i = 0; i < cssPropertiesAndValues.length; i += 2) {
                        var propName = cssPropertiesAndValues[i]; var propValue = cssPropertiesAndValues[i + 1]; var semi = propName.indexOf(';'); if (semi >= 0) { propName = propName.substring(0, semi); }
                        css.push(propName + ' : ' + propValue);
                    }
                    return css.join(' ; '); case html4.atype.FRAME_TARGET: return null; default: return String(value);
            } 
        }
        function tameNode(node, editable) {
            if (node === null || node === void 0) { return null; }
            if (node.tamed___) { return node.tamed___; }
            var tamed; switch (node.nodeType) {
                case 1: var tagName = node.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName) || (html4.ELEMENTS[tagName] & html4.eflags.UNSAFE)) { tamed = new TameOpaqueNode(node, editable); break; }
                    switch (tagName) { case 'a': tamed = new TameAElement(node, editable); break; case 'form': tamed = new TameFormElement(node, editable); break; case 'select': case 'button': case 'option': case 'textarea': case 'input': tamed = new TameInputElement(node, editable); break; case 'img': tamed = new TameImageElement(node, editable); break; case 'td': case 'tr': case 'thead': case 'tfoot': case 'tbody': case 'th': tamed = new TameTableCompElement(node, editable); break; case 'table': tamed = new TameTableElement(node, editable); break; default: tamed = new TameElement(node, editable); break; }
                    break; case 3: tamed = new TameTextNode(node, editable); break; case 8: tamed = new TameCommentNode(node, editable); break; default: tamed = new TameOpaqueNode(node, editable); break;
            }
            node.tamed___ = tamed; return tamed;
        }
        function tameRelatedNode(node, editable) {
            if (node === null || node === void 0) { return null; }
            try { for (var ancestor = node; ancestor; ancestor = ancestor.parentNode) { if (idClass === ancestor.className) { return tameNode(node, editable); } } } catch (e) { }
            return null;
        }
        function tameNodeList(nodeList, editable, opt_keyAttrib) {
            var tamed = []; for (var i = nodeList.length; --i >= 0; ) { var node = tameNode(nodeList.item(i), editable); tamed[i] = node; var key = opt_keyAttrib && node.getAttribute(opt_keyAttrib); if (key && !(key.charAt(key.length - 1) === '_' || (key in tamed) || key === String(key & 0x7fffffff))) { tamed[key] = node; } }
            node = nodeList = null; tamed.item = ___.frozenFunc(function(k) {
                k &= 0x7fffffff; if (isNaN(k)) { throw new Error(); }
                return tamed[k] || null;
            }); return cajita.freeze(tamed);
        }
        function makeEventHandlerWrapper(thisNode, listener) {
            if ('function' !== typeof listener && !('object' === (typeof listener) && listener !== null && ___.canCallPub(listener, 'call'))) { throw new Error('Expected function not ' + typeof listener); }
            function wrapper(event) { return plugin_dispatchEvent___(thisNode, event, ___.getId(imports), listener); }
            wrapper.originalListener___ = listener; return wrapper;
        }
        var NOT_EDITABLE = "Node not editable."; function tameAddEventListener(name, listener, useCapture) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            if (!this.wrappedListeners___) { this.wrappedListeners___ = []; }
            name = String(name); useCapture = Boolean(useCapture); var wrappedListener = makeEventHandlerWrapper(this.node___, listener); this.wrappedListeners___.push(wrappedListener); bridal.addEventListener(this.node___, name, wrappedListener, useCapture);
        }
        function tameRemoveEventListener(name, listener, useCapture) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            if (!this.wrappedListeners___) { return; }
            var wrappedListener; for (var i = this.wrappedListeners___.length; --i >= 0; ) { if (this.wrappedListeners___[i].originalListener___ === listener) { wrappedListener = this.wrappedListeners___[i]; this.wrappedListeners___ = arrayRemove(this.wrappedListeners___, i, i); break; } }
            if (!wrappedListener) { return; }
            name = String(name); bridal.removeEventListener(this.node___, name, wrappedListener, useCapture);
        }
        function tameDispatchEvent(evt) { cajita.guard(tameEventTrademark, evt); }
        var nodeClasses = {}; var tameNodeFields = ['nodeType', 'nodeValue', 'nodeName', 'firstChild', 'lastChild', 'nextSibling', 'previousSibling', 'parentNode', 'ownerDocument', 'childNodes']; function TameNode(node, editable) {
            if (!node) { throw new Error('Creating tame node with undefined native delegate'); }
            this.node___ = node; this.editable___ = editable; ___.stamp(tameNodeTrademark, this, true); exportFields(this, tameNodeFields);
        }
        nodeClasses.Node = TameNode; TameNode.prototype.getNodeType = function() { return this.node___.nodeType; }; TameNode.prototype.getNodeName = function() { return this.node___.nodeName; }; TameNode.prototype.getNodeValue = function() { return this.node___.nodeValue; }; TameNode.prototype.appendChild = function(child) {
            cajita.guard(tameNodeTrademark, child); if (!this.editable___ || !child.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.appendChild(child.node___);
        }; TameNode.prototype.insertBefore = function(toInsert, child) {
            cajita.guard(tameNodeTrademark, toInsert); if (child === void 0) { child = null; }
            if (child !== null) { cajita.guard(tameNodeTrademark, child); }
            if (!this.editable___ || !toInsert.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.insertBefore(toInsert.node___, child !== null ? child.node___ : null);
        }; TameNode.prototype.removeChild = function(child) {
            cajita.guard(tameNodeTrademark, child); if (!this.editable___ || !child.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.removeChild(child.node___);
        }; TameNode.prototype.replaceChild = function(child, replacement) {
            cajita.guard(tameNodeTrademark, child); cajita.guard(tameNodeTrademark, replacement); if (!this.editable___ || !replacement.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.replaceChild(child.node___, replacement.node___);
        }; TameNode.prototype.getFirstChild = function() { return tameNode(this.node___.firstChild, this.editable___); }; TameNode.prototype.getLastChild = function() { return tameNode(this.node___.lastChild, this.editable___); }; TameNode.prototype.getNextSibling = function() { return tameNode(this.node___.nextSibling, this.editable___); }; TameNode.prototype.getPreviousSibling = function() { return tameNode(this.node___.previousSibling, this.editable___); }; TameNode.prototype.getParentNode = function() { return tameRelatedNode(this.node___.parentNode, this.editable___); }; TameNode.prototype.getElementsByTagName = function(tagName) { return tameNodeList(this.node___.getElementsByTagName(String(tagName)), this.editable___); }; TameNode.prototype.getOwnerDocument = function() { return imports.document; }; TameNode.prototype.getElementsByClassName = function(className) { return tameNodeList(this.node___.getElementsByClassName(String(className), this.editable___)); }; TameNode.prototype.getChildNodes = function() { return tameNodeList(this.node___.childNodes, this.editable___); }; TameNode.prototype.handleRead___ = function(name) {
            var handlerName = name + '_getter___'; if (this[handlerName]) { return this[handlerName](); }
            if (this.node___ !== void 0 && this.node___ !== null && this.node___.properties___) { return this.node___.properties___[name]; } else { return void 0; } 
        }; TameNode.prototype.handleCall___ = function(name, args) {
            var handlerName = name + '_handler___'; if (this[handlerName]) { return this[handlerName].call(this, args); }
            if (this.node___ !== void 0 && this.node___ !== null && this.node___.properties___) { return this.node___.properties___[name].call(this, args); } else { throw new TypeError(name + " is not a function."); } 
        }; TameNode.prototype.handleSet___ = function(name, val) {
            var handlerName = name + '_setter___'; if (this[handlerName]) { return this[handlerName](val); }
            if (this.node___ === void 0 || this.node___ === null) { this.node___ = {}; }
            if (!this.node___.properties___) { this.node___.properties___ = {}; }
            this[name + "_canEnum___"] = true; return this.node___.properties___[name] = val;
        }; TameNode.prototype.handleDelete___ = function(name) {
            var handlerName = name + '_deleter___'; if (this[handlerName]) { return this[handlerName](); }
            if (this.node___ !== void 0 && this.node___ !== null && this.node___.properties___) { return (delete this.node___.properties___[name] && delete this[name + "_canEnum___"]); } else { return true; } 
        }; TameNode.prototype.handleEnum___ = function(ownFlag) {
            var result = []; if (this.node___ && this.node___.properties___) { result = cajita.allKeys(this.node___.properties___); }
            return result;
        }; TameNode.prototype.hasChildNodes = function() { return !!this.node___.hasChildNodes(); }; ___.ctor(TameNode, void 0, 'TameNode'); var tameNodeMembers = ['getNodeType', 'getNodeValue', 'getNodeName', 'appendChild', 'insertBefore', 'removeChild', 'replaceChild', 'getFirstChild', 'getLastChild', 'getNextSibling', 'getPreviousSibling', 'getElementsByTagName', 'getOwnerDocument', 'hasChildNodes']; ___.all2(___.grantTypedGeneric, TameNode.prototype, tameNodeMembers); function TameOpaqueNode(node, editable) { TameNode.call(this, node, editable); }
        extend(TameOpaqueNode, TameNode); TameOpaqueNode.prototype.getNodeValue = TameNode.prototype.getNodeValue; TameOpaqueNode.prototype.getNodeType = TameNode.prototype.getNodeType; TameOpaqueNode.prototype.getNodeName = TameNode.prototype.getNodeName; TameOpaqueNode.prototype.getNextSibling = TameNode.prototype.getNextSibling; TameOpaqueNode.prototype.getPreviousSibling = TameNode.prototype.getPreviousSibling; TameOpaqueNode.prototype.getFirstChild = TameNode.prototype.getFirstChild; TameOpaqueNode.prototype.getLastChild = TameNode.prototype.getLastChild; TameOpaqueNode.prototype.getParentNode = TameNode.prototype.getParentNode; TameOpaqueNode.prototype.getChildNodes = TameNode.prototype.getChildNodes; for (var i = tameNodeMembers.length; --i >= 0; ) { var k = tameNodeMembers[i]; if (!TameOpaqueNode.prototype.hasOwnProperty(k)) { TameOpaqueNode.prototype[k] = ___.frozenFunc(function() { throw new Error('Node is opaque'); }); } }
        ___.all2(___.grantTypedGeneric, TameOpaqueNode.prototype, tameNodeMembers); function TameTextNode(node, editable) { assert(node.nodeType === 3); TameNode.call(this, node, editable); exportFields(this, ['nodeValue', 'data']); }
        extend(TameTextNode, TameNode); nodeClasses.TextNode = TameTextNode; TameTextNode.prototype.setNodeValue = function(value) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.nodeValue = String(value || ''); return value;
        }; TameTextNode.prototype.getData = TameTextNode.prototype.getNodeValue; TameTextNode.prototype.setData = TameTextNode.prototype.setNodeValue; TameTextNode.prototype.toString = function() { return '#text'; }; ___.ctor(TameTextNode, void 0, 'TameNode'); ___.all2(___.grantTypedGeneric, TameTextNode.prototype, ['setNodeValue', 'getData', 'setData']); function TameCommentNode(node, editable) { assert(node.nodeType === 8); TameNode.call(this, node, editable); }
        extend(TameCommentNode, TameNode); nodeClasses.CommentNode = TameCommentNode; TameCommentNode.prototype.toString = function() { return '#comment'; }; ___.ctor(TameCommentNode, void 0, 'TameNode'); function TameElement(node, editable) { assert(node.nodeType === 1); TameNode.call(this, node, editable); exportFields(this, ['className', 'id', 'innerHTML', 'tagName', 'style', 'offsetLeft', 'offsetTop', 'offsetWidth', 'offsetHeight', 'offsetParent', 'scrollLeft', 'scrollTop', 'scrollWidth', 'scrollHeight', 'title', 'dir']); }
        extend(TameElement, TameNode); nodeClasses.Element = TameElement; nodeClasses.HTMLElement = TameElement; TameElement.prototype.getId = function() { return this.getAttribute('id') || ''; }; TameElement.prototype.setId = function(newId) { return this.setAttribute('id', newId); }; TameElement.prototype.getAttribute = function(attribName) {
            attribName = String(attribName).toLowerCase(); var tagName = this.node___.tagName.toLowerCase(); var attribKey; var atype; if ((attribKey = tagName + ':' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey))) { atype = html4.ATTRIBS[attribKey]; } else { return ''; }
            var value = this.node___.getAttribute(attribName); if ('string' !== typeof value) { return value; }
            switch (atype) {
                case html4.atype.ID: case html4.atype.IDREF: case html4.atype.IDREFS: if (!value) { return ''; }
                    var n = idSuffix.length; var len = value.length; var end = len - n; if (end > 0 && idSuffix === value.substring(end, len)) { return value.substring(0, end); }
                    return ''; default: return value;
            } 
        }; TameElement.prototype.hasAttribute = function(name) {
            name = String(name).toLowerCase(); var type = html4.ATTRIBS[name]; if (type === undefined || !html4.ATTRIBS.hasOwnProperty(name)) { return false; }
            return this.node___.hasAttribute(name);
        }; TameElement.prototype.setAttribute = function(attribName, value) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            attribName = String(attribName).toLowerCase(); var tagName = this.node___.tagName.toLowerCase(); var attribKey; var atype; if ((attribKey = tagName + ':' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey))) { atype = html4.ATTRIBS[attribKey]; } else { throw new Error(); }
            var sanitizedValue = rewriteAttribute(tagName, attribName, atype, value); if (sanitizedValue !== null) { bridal.setAttribute(this.node___, attribName, sanitizedValue); }
            return value;
        }; TameElement.prototype.removeAttribute = function(name) {
            if (!this.editable___) { throw new Error(); }
            name = String(name).toLowerCase(); var type = html4.ATTRIBS[name]; if (type === void 0 || !html4.ATTRIBS.hasOwnProperty(name)) { return; }
            this.node___.removeAttribute(name);
        }; TameElement.prototype.getClassName = function() { return this.getAttribute('class') || ''; }; TameElement.prototype.setClassName = function(classes) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            return this.setAttribute('class', String(classes));
        }; TameElement.prototype.getTitle = function() { return this.getAttribute('title') || ''; }; TameElement.prototype.setTitle = function(classes) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('title', String(classes));
        }; TameElement.prototype.getDir = function() { return this.getAttribute('dir') || ''; }; TameElement.prototype.setDir = function(classes) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('dir', String(classes));
        }; TameElement.prototype.getTagName = TameNode.prototype.getNodeName; TameElement.prototype.getInnerHTML = function() {
            var tagName = this.node___.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) { return ''; }
            var flags = html4.ELEMENTS[tagName]; var innerHtml = this.node___.innerHTML; if (flags & html4.eflags.CDATA) { innerHtml = html.escapeAttrib(innerHtml); } else if (flags & html4.eflags.RCDATA) { innerHtml = html.normalizeRCData(innerHtml); } else { innerHtml = tameInnerHtml(innerHtml); }
            return innerHtml;
        }; TameElement.prototype.setInnerHTML = function(htmlFragment) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            var tagName = this.node___.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) { throw new Error(); }
            var flags = html4.ELEMENTS[tagName]; if (flags & html4.eflags.UNSAFE) { throw new Error(); }
            var sanitizedHtml; if (flags & html4.eflags.RCDATA) { sanitizedHtml = html.normalizeRCData(String(htmlFragment || '')); } else { sanitizedHtml = (htmlFragment instanceof Html ? safeHtml(htmlFragment) : sanitizeHtml(String(htmlFragment || ''))); }
            this.node___.innerHTML = sanitizedHtml; return htmlFragment;
        }; TameElement.prototype.setStyle = function(style) { this.setAttribute('style', style); return this.getStyle(); }; TameElement.prototype.getStyle = function() { return new TameStyle(this.node___.style, this.editable___); }; TameElement.prototype.updateStyle = function(style) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            var cssPropertiesAndValues = cssSealerUnsealerPair.unseal(style); if (!cssPropertiesAndValues) { throw new Error(); }
            var styleNode = this.node___.style; for (var i = 0; i < cssPropertiesAndValues.length; i += 2) {
                var propName = cssPropertiesAndValues[i]; var propValue = cssPropertiesAndValues[i + 1]; var semi = propName.indexOf(';'); if (semi >= 0) { propName = propName.substring(semi + 1); }
                styleNode[propName] = propValue;
            } 
        }; TameElement.prototype.getOffsetLeft = function() { return this.node___.offsetLeft; }; TameElement.prototype.getOffsetTop = function() { return this.node___.offsetTop; }; TameElement.prototype.getOffsetWidth = function() { return this.node___.offsetWidth; }; TameElement.prototype.getOffsetHeight = function() { return this.node___.offsetHeight; }; TameElement.prototype.getOffsetParent = function() { return tameRelatedNode(this.node___.offsetParent, this.editable___); }; TameElement.prototype.getScrollLeft = function() { return this.node___.scrollLeft; }; TameElement.prototype.getScrollTop = function() { return this.node___.scrollTop; }; TameElement.prototype.getScrollWidth = function() { return this.node___.scrollWidth; }; TameElement.prototype.getScrollHeight = function() { return this.node___.scrollHeight; }; TameElement.prototype.toString = function() { return '<' + this.node___.tagName + '>'; }; TameElement.prototype.addEventListener = tameAddEventListener; TameElement.prototype.removeEventListener = tameRemoveEventListener; TameElement.prototype.dispatchEvent = tameDispatchEvent; ___.ctor(TameElement, TameNode, 'TameElement'); ___.all2(___.grantTypedGeneric, TameElement.prototype, ['addEventListener', 'removeEventListener', 'dispatchEvent', 'getAttribute', 'setAttribute', 'removeAttribute', 'hasAttribute', 'getClassName', 'setClassName', 'getId', 'setId', 'getInnerHTML', 'setInnerHTML', 'updateStyle', 'getStyle', 'setStyle', 'getTagName', 'getOffsetLeft', 'getOffsetTop', 'getOffsetWidth', 'getOffsetHeight']); (function() {
            var attrNameRe = /:(.*)/; for (var html4Attrib in html4.ATTRIBS) {
                if (html4.atype.SCRIPT === html4.ATTRIBS[html4Attrib]) {
                    (function(attribName) {
                        ___.useSetHandler(TameElement.prototype, attribName, function eventHandlerSetter(listener) {
                            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
                            if (!listener) { this.node___[attribName] = null; } else { this.node___[attribName] = makeEventHandlerWrapper(this.node___, listener); return listener; } 
                        });
                    })(html4Attrib.match(attrNameRe)[1]);
                } 
            } 
        })(); function TameAElement(node, editable) { TameElement.call(this, node, editable); exportFields(this, ['href']); }
        extend(TameAElement, TameElement); nodeClasses.HTMLAnchorElement = TameAElement; TameAElement.prototype.focus = function() { this.node___.focus(); }; TameAElement.prototype.getHref = function() { return this.node___.href; }; TameAElement.prototype.setHref = function(href) { this.setAttribute('href', href); return href; }; ___.ctor(TameAElement, TameElement, 'TameAElement'); ___.all2(___.grantTypedGeneric, TameAElement.prototype, ['getHref', 'setHref', 'focus']); function TameFormElement(node, editable) { TameElement.call(this, node, editable); exportFields(this, ['action', 'elements', 'enctype', 'method', 'target']); }
        extend(TameFormElement, TameElement); nodeClasses.HTMLFormElement = TameFormElement; TameFormElement.prototype.submit = function() { return this.node___.submit(); }; TameFormElement.prototype.reset = function() { return this.node___.reset(); }; TameFormElement.prototype.getAction = function() { return this.getAttribute('action'); }; TameFormElement.prototype.setAction = function(newVal) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('action', String(newVal));
        }; TameFormElement.prototype.getElements = function() { return tameNodeList(this.node___.elements, this.editable___, 'name'); }; TameFormElement.prototype.getEnctype = function() { return this.getAttribute('enctype') || ''; }; TameFormElement.prototype.setEnctype = function(newVal) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('enctype', String(newVal));
        }; TameFormElement.prototype.getMethod = function() { return this.getAttribute('method') || ''; }; TameFormElement.prototype.setMethod = function(newVal) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('method', String(newVal));
        }; TameFormElement.prototype.getTarget = function() { return this.getAttribute('target') || ''; }; TameFormElement.prototype.setTarget = function(newVal) {
            if (!this.editable___) { throw new Error(); }
            return this.setAttribute('target', String(newVal));
        }; TameFormElement.prototype.reset = function() {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.reset();
        }; TameFormElement.prototype.submit = function() {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.submit();
        }; ___.ctor(TameFormElement, TameElement, 'TameFormElement'); ___.all2(___.grantTypedGeneric, TameFormElement.prototype, ['getElements', 'reset', 'submit']); function TameInputElement(node, editable) { TameElement.call(this, node, editable); exportFields(this, ['form', 'value', 'checked', 'disabled', 'readOnly', 'options', 'selected', 'selectedIndex', 'name', 'accessKey', 'tabIndex', 'text', 'defaultChecked', 'defaultSelected', 'maxLength', 'size', 'type', 'index', 'label', 'multiple', 'cols', 'rows']); }
        extend(TameInputElement, TameElement); nodeClasses.HTMLInputElement = TameInputElement; TameInputElement.prototype.getChecked = function() { return this.node___.checked; }; TameInputElement.prototype.setChecked = function(checked) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            return (this.node___.checked = !!checked);
        }; TameInputElement.prototype.getValue = function() { var value = this.node___.value; return value === null || value === void 0 ? null : String(value); }; TameInputElement.prototype.setValue = function(newValue) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            this.node___.value = (newValue === null || newValue === void 0 ? '' : '' + newValue); return newValue;
        }; TameInputElement.prototype.focus = function() { this.node___.focus(); }; TameInputElement.prototype.blur = function() { this.node___.blur(); }; TameInputElement.prototype.select = function() { this.node___.select(); }; TameInputElement.prototype.getForm = function() { return tameRelatedNode(this.node___.form, this.editable___); }; TameInputElement.prototype.getChecked = function() { return this.node___.checked; }; TameInputElement.prototype.setChecked = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.checked = newValue; return newValue;
        }; TameInputElement.prototype.getDisabled = function() { return this.node___.disabled; }; TameInputElement.prototype.setDisabled = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.disabled = newValue; return newValue;
        }; TameInputElement.prototype.getReadOnly = function() { return this.node___.readOnly; }; TameInputElement.prototype.setReadOnly = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.readOnly = newValue; return newValue;
        }; TameInputElement.prototype.getOptions = function() { return tameNodeList(this.node___.options, this.editable___, 'name'); }; TameInputElement.prototype.getDefaultSelected = function() { return !!this.node___.defaultSelected; }; TameInputElement.prototype.getSelected = function() { return this.node___.selected; }; TameInputElement.prototype.setSelected = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.selected = newValue; return newValue;
        }; TameInputElement.prototype.getSelectedIndex = function() { return this.node___.selectedIndex; }; TameInputElement.prototype.setSelectedIndex = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.selectedIndex = newValue; return newValue;
        }; TameInputElement.prototype.getName = function() { return this.node___.name; }; TameInputElement.prototype.setName = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.name = newValue; return newValue;
        }; TameInputElement.prototype.getAccessKey = function() { return this.node___.accessKey; }; TameInputElement.prototype.setAccessKey = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.accessKey = newValue; return newValue;
        }; TameInputElement.prototype.getTabIndex = function() { return this.node___.tabIndex; }; TameInputElement.prototype.getText = function() { return String(this.node___.text); }; TameInputElement.prototype.setTabIndex = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.tabIndex = newValue; return newValue;
        }; TameInputElement.prototype.getDefaultChecked = function() { return this.node___.defaultChecked; }; TameInputElement.prototype.setDefaultChecked = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.defaultChecked = newValue; return newValue;
        }; TameInputElement.prototype.getMaxLength = function() { return this.node___.maxLength; }; TameInputElement.prototype.setMaxLength = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.maxLength = newValue; return newValue;
        }; TameInputElement.prototype.getSize = function() { return this.node___.size; }; TameInputElement.prototype.setSize = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.size = newValue; return newValue;
        }; TameInputElement.prototype.getType = function() { return String(this.node___.type); }; TameInputElement.prototype.setType = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.type = newValue; return newValue;
        }; TameInputElement.prototype.getIndex = function() { return this.node___.index; }; TameInputElement.prototype.setIndex = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.index = newValue; return newValue;
        }; TameInputElement.prototype.getLabel = function() { return this.node___.label; }; TameInputElement.prototype.setLabel = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.label = newValue; return newValue;
        }; TameInputElement.prototype.getMultiple = function() { return this.node___.multiple; }; TameInputElement.prototype.setMultiple = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.multiple = newValue; return newValue;
        }; TameInputElement.prototype.getCols = function() { return this.node___.cols; }; TameInputElement.prototype.setCols = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.cols = newValue; return newValue;
        }; TameInputElement.prototype.getRows = function() { return this.node___.rows; }; TameInputElement.prototype.setRows = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.rows = newValue; return newValue;
        }; ___.ctor(TameInputElement, TameElement, 'TameInputElement'); ___.all2(___.grantTypedGeneric, TameInputElement.prototype, ['getValue', 'setValue', 'focus', 'getForm', 'getType', 'select']); function TameImageElement(node, editable) { TameElement.call(this, node, editable); exportFields(this, ['src', 'alt']); }
        extend(TameImageElement, TameElement); nodeClasses.HTMLImageElement = TameImageElement; TameImageElement.prototype.getSrc = function() { return this.node___.src; }; TameImageElement.prototype.setSrc = function(src) {
            if (!this.editable___) { throw new Error(); }
            this.setAttribute('src', src); return src;
        }; TameImageElement.prototype.getAlt = function() { return this.node___.alt; }; TameImageElement.prototype.setAlt = function(src) {
            if (!this.editable___) { throw new Error(); }
            this.node___.alt = src; return src;
        }; ___.ctor(TameImageElement, TameElement, 'TameImageElement'); ___.all2(___.grantTypedGeneric, TameImageElement.prototype, ['getSrc', 'setSrc', 'getAlt', 'setAlt']); function TameTableCompElement(node, editable) { TameElement.call(this, node, editable); exportFields(this, ['colSpan', 'cells', 'rowSpan', 'rows', 'rowIndex', 'align', 'vAlign', 'nowrap']); }
        extend(TameTableCompElement, TameElement); TameTableCompElement.prototype.getColSpan = function() { return this.node___.colSpan; }; TameTableCompElement.prototype.setColSpan = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.colSpan = newValue; return newValue;
        }; TameTableCompElement.prototype.getCells = function() { return tameNodeList(this.node___.cells, this.editable___); }; TameTableCompElement.prototype.getRowSpan = function() { return this.node___.rowSpan; }; TameTableCompElement.prototype.setRowSpan = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.rowSpan = newValue; return newValue;
        }; TameTableCompElement.prototype.getRows = function() { return tameNodeList(this.node___.rows, this.editable___); }; TameTableCompElement.prototype.getRowIndex = function() { return this.node___.rowIndex; }; TameTableCompElement.prototype.getAlign = function() { return this.node___.align; }; TameTableCompElement.prototype.setAlign = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.align = newValue; return newValue;
        }; TameTableCompElement.prototype.getVAlign = function() { return this.node___.vAlign; }; TameTableCompElement.prototype.setVAlign = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.vAlign = newValue; return newValue;
        }; TameTableCompElement.prototype.getNowrap = function() { return this.node___.nowrap; }; TameTableCompElement.prototype.setNowrap = function(newValue) {
            if (!this.editable___) { throw new Error(); }
            this.node___.nowrap = newValue; return newValue;
        }; ___.ctor(TameTableCompElement, TameElement, 'TameTableCompElement'); function TameTableElement(node, editable) { TameTableCompElement.call(this, node, editable); exportFields(this, ['tBodies', 'tHead', 'tFoot']); }
        extend(TameTableElement, TameTableCompElement); nodeClasses.HTMLTableElement = TameTableElement; TameTableElement.prototype.getTBodies = function() { return tameNodeList(this.node___.tBodies, this.editable___); }; TameTableElement.prototype.getTHead = function() { return tameNode(this.node___.tHead, this.editable___); }; TameTableElement.prototype.getTFoot = function() { return tameNode(this.node___.tFoot, this.editable___); }; TameTableElement.prototype.createTHead = function() {
            if (!this.editable___) { throw new Error(); }
            return tameNode(this.node___.createTHead(), this.editable___);
        }; TameTableElement.prototype.deleteTHead = function() {
            if (!this.editable___) { throw new Error(); }
            this.node___.deleteTHead();
        }; TameTableElement.prototype.createTFoot = function() {
            if (!this.editable___) { throw new Error(); }
            return tameNode(this.node___.createTFoot(), this.editable___);
        }; TameTableElement.prototype.deleteTFoot = function() {
            if (!this.editable___) { throw new Error(); }
            this.node___.deleteTFoot();
        }; ___.ctor(TameTableElement, TameTableCompElement, 'TameTableElement'); ___.all2(___.grantTypedGeneric, TameTableElement.prototype, ['createTHead', 'deleteTHead', 'createTFoot', 'deleteTFoot']); function TameEvent(event) { this.event___ = event; ___.stamp(tameEventTrademark, this, true); exportFields(this, ['type', 'target', 'pageX', 'pageY', 'altKey', 'ctrlKey', 'metaKey', 'shiftKey', 'button', 'screenX', 'screenY', 'relatedTarget', 'fromElement', 'toElement', 'srcElement', 'clientX', 'clientY', 'keyCode', 'which']); }
        nodeClasses.Event = TameEvent; TameEvent.prototype.getType = function() { return String(this.event___.type); }; TameEvent.prototype.getTarget = function() { var event = this.event___; return tameRelatedNode(event.target || event.srcElement, true); }; TameEvent.prototype.getSrcElement = function() { return tameRelatedNode(this.event___.srcElement, true); }; TameEvent.prototype.getRelatedTarget = function() {
            var e = this.event___; var t = e.relatedTarget; if (!t) { if (e.type === 'mouseout') { t = e.toElement; } else if (e.type === 'mouseover') { t = e.fromElement; } }
            return tameRelatedNode(t, true);
        }; TameEvent.prototype.getFromElement = function() { return tameRelatedNode(this.event___.fromElement, true); }; TameEvent.prototype.getToElement = function() { return tameRelatedNode(this.event___.toElement, true); }; TameEvent.prototype.getPageX = function() { return Number(this.event___.pageX); }; TameEvent.prototype.getPageY = function() { return Number(this.event___.pageY); }; TameEvent.prototype.stopPropagation = function() {
            if (this.event___.stopPropagation)
                this.event___.stopPropagation(); else
                this.event___.cancelBubble = true;
        }; TameEvent.prototype.preventDefault = function() {
            if (this.event___.preventDefault)
                this.event___.preventDefault(); else
                this.event___.returnValue = false;
        }; TameEvent.prototype.getAltKey = function() { return Boolean(this.event___.altKey); }; TameEvent.prototype.getCtrlKey = function() { return Boolean(this.event___.ctrlKey); }; TameEvent.prototype.getMetaKey = function() { return Boolean(this.event___.metaKey); }; TameEvent.prototype.getShiftKey = function() { return Boolean(this.event___.shiftKey); }; TameEvent.prototype.getButton = function() { var e = this.event___; return e.button && Number(e.button); }; TameEvent.prototype.getClientX = function() { return Number(this.event___.clientX); }; TameEvent.prototype.getClientY = function() { return Number(this.event___.clientY); }; TameEvent.prototype.getScreenX = function() { return Number(this.event___.screenX); }; TameEvent.prototype.getScreenY = function() { return Number(this.event___.screenY); }; TameEvent.prototype.getWhich = function() { var w = this.event___.which; return w && Number(w); }; TameEvent.prototype.getKeyCode = function() { var kc = this.event___.keyCode; return kc && Number(kc); }; TameEvent.prototype.toString = function() { return 'Not a real event'; }; ___.ctor(TameEvent, void 0, 'TameEvent'); ___.all2(___.grantTypedGeneric, TameEvent.prototype, ['getType', 'getTarget', 'getPageX', 'getPageY', 'stopPropagation', 'getAltKey', 'getCtrlKey', 'getMetaKey', 'getShiftKey', 'getButton', 'getClientX', 'getClientY', 'getScreenX', 'getScreenY', 'getRelatedTarget', 'getFromElement', 'getToElement', 'getSrcElement', 'preventDefault', 'getKeyCode', 'getWhich']); function TameDocument(doc, editable) { this.doc___ = doc; this.editable___ = editable; }
        extend(TameDocument, TameNode); nodeClasses.HTMLDocument = TameDocument; TameDocument.prototype.addEventListener = function(name, listener, useCapture) { }; TameDocument.prototype.removeEventListener = function(name, listener, useCapture) { }; TameDocument.prototype.dispatchEvent = function(evt) { }; TameDocument.prototype.createElement = function(tagName) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            tagName = String(tagName).toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) { throw new Error(); }
            var flags = html4.ELEMENTS[tagName]; if (flags & html4.eflags.UNSAFE) { throw new Error(); }
            var newEl = this.doc___.createElement(tagName); if (elementPolicies.hasOwnProperty(tagName)) { var attribs = elementPolicies[tagName]([]); if (attribs) { for (var i = 0; i < attribs.length; i += 2) { bridal.setAttribute(newEl, attribs[i], attribs[i + 1]); } } }
            return tameNode(newEl, true);
        }; TameDocument.prototype.createTextNode = function(text) {
            if (!this.editable___) { throw new Error(NOT_EDITABLE); }
            return tameNode(this.doc___.createTextNode(text !== null && text !== void 0 ? '' + text : ''), true);
        }; TameDocument.prototype.getElementById = function(id) { id += idSuffix; var node = this.doc___.getElementById(id); return tameNode(node, this.editable___); }; TameDocument.prototype.getElementsByTagName = function(tagName) { var base = imports.htmlEmitter___.cursor_[0]; return tameNodeList(base.getElementsByTagName(String(tagName)), this.editable___); }; TameDocument.prototype.toString = function() { return '[Fake Document]'; }; TameDocument.prototype.write = function(text) { cajita.log('Called document.write() with: ' + text); }; ___.ctor(TameDocument, void 0, 'TameDocument'); ___.all2(___.grantTypedGeneric, TameDocument.prototype, ['addEventListener', 'removeEventListener', 'dispatchEvent', 'createElement', 'createTextNode', 'getElementById', 'getElementsByTagName', 'getElementsByClassName', 'write']); imports.tameNode___ = tameNode; imports.tameEvent___ = function(event) { return new TameEvent(event); }; imports.blessHtml___ = blessHtml; imports.blessCss___ = function(var_args) {
            var arr = []; for (var i = 0, n = arguments.length; i < n; ++i) { arr[i] = arguments[i]; }
            return cssSealerUnsealerPair.seal(arr);
        }; imports.htmlAttr___ = function(s) { return html.escapeAttrib(String(s || '')); }; imports.html___ = safeHtml; imports.rewriteUri___ = function(uri, mimeType) {
            var s = rewriteAttribute(null, null, html4.atype.URI, uri); if (!s) { throw new Error(); }
            return s;
        }; imports.suffix___ = function(nmtokens) {
            var p = String(nmtokens).replace(/^\s+|\s+$/g, '').split(/\s+/g); var out = []; for (var i = 0; i < p.length; ++i) {
                nmtoken = rewriteAttribute(null, null, html4.atype.ID, p[i]); if (!nmtoken) { throw new Error(nmtokens); }
                out.push(nmtoken);
            }
            return out.join(' ');
        }; imports.ident___ = function(nmtokens) {
            var p = String(nmtokens).replace(/^\s+|\s+$/g, '').split(/\s+/g); var out = []; for (var i = 0; i < p.length; ++i) {
                nmtoken = rewriteAttribute(null, null, html4.atype.CLASSES, p[i]); if (!nmtoken) { throw new Error(nmtokens); }
                out.push(nmtoken);
            }
            return out.join(' ');
        }; function TameStyle(style, editable) { this.style___ = style; this.editable___ = editable; }
        nodeClasses.Style = TameStyle; for (var styleProperty in css.properties) {
            if (!cajita.canEnumOwn(css.properties, styleProperty)) { continue; }
            (function(propertyName) {
                ___.useGetHandler(TameStyle.prototype, propertyName, function() { return String(this.style___[propertyName] || ''); }); var pattern = css.properties[propertyName]; ___.useSetHandler(TameStyle.prototype, propertyName, function(val) {
                    if (!this.editable___) { throw new Error('style not editable'); }
                    val = '' + (val || ''); if (val && !pattern.test(val + ' ')) {
                        throw new Error('bad value `' + val + '` for CSS property '
+ propertyName);
                    }
                    val = val.replace(/\burl\s*\(\s*\"([^\"]*)\"\s*\)/gi, function(_, url) { throw new Error('url in style ' + url); }); this.style___[propertyName] = val;
                });
            })(styleProperty);
        }
        TameStyle.prototype.toString = function() { return '[Fake Style]'; }; function TameXMLHttpRequest() { exportFields(this, ['requestHeader', 'onreadystatechange', 'readyState', 'responseText', 'responseXML', 'responseBody', 'status', 'statusText']); alert('Created new XHR: ' + this); }
        nodeClasses.XMLHttpRequest = TameXMLHttpRequest; TameXMLHttpRequest.prototype.abort = function() { }; TameXMLHttpRequest.prototype.getAllResponseHeaders = function() { return ""; }; TameXMLHttpRequest.prototype.getResponseHeader = function(headerName) { return ""; }; TameXMLHttpRequest.prototype.open = function(method, URL, opt_async, opt_userName, opt_password) { }; TameXMLHttpRequest.prototype.send = function(content) { }; TameXMLHttpRequest.prototype.setRequestHeader = function(label, value) { }; TameXMLHttpRequest.prototype.setOnreadystatechange = function(value) { }; TameXMLHttpRequest.prototype.getReadyState = function() { return 0; }; TameXMLHttpRequest.prototype.getResponseText = function() { return ""; }; TameXMLHttpRequest.prototype.getResponseXML = function() { return {}; }; TameXMLHttpRequest.prototype.getResponseBody = function() { return ""; }; TameXMLHttpRequest.prototype.getStatus = function() { return 404; }; TameXMLHttpRequest.prototype.getStatusText = function() { return "Not Found"; }; TameXMLHttpRequest.prototype.toString = function() { return 'Not a real XMLHttpRequest'; }; ___.ctor(TameXMLHttpRequest, void 0, 'TameXMLHttpRequest'); ___.all2(___.grantTypedGeneric, TameXMLHttpRequest.prototype, ['abort', 'getAllResponseHeaders', 'getResponseHeader', 'open', 'send']); imports.cssNumber___ = function(num) {
            if ('number' === typeof num && isFinite(num) && !isNaN(num)) { return '' + num; }
            throw new Error(num);
        }; imports.cssColor___ = function(color) {
            if ('number' !== typeof color || (color != (color | 0))) { throw new Error(color); }
            var hex = '0123456789abcdef'; return '#' + hex.charAt((color >> 20) & 0xf)
+ hex.charAt((color >> 16) & 0xf)
+ hex.charAt((color >> 12) & 0xf)
+ hex.charAt((color >> 8) & 0xf)
+ hex.charAt((color >> 4) & 0xf)
+ hex.charAt(color & 0xf);
        }; imports.cssUri___ = function(uri, mimeType) {
            var s = rewriteAttribute(null, null, html4.atype.URI, uri); if (!s) { throw new Error(); }
            return s;
        }; imports.emitCss___ = function(cssText) { this.getCssContainer___().appendChild(bridal.createStylesheet(document, cssText)); }; imports.getCssContainer___ = function() { return document.getElementsByTagName('head')[0]; }; if (!/^-/.test(idSuffix)) { throw new Error('id suffix "' + idSuffix + '" must start with "-"'); }
        var idClass = idSuffix.substring(1); imports.getIdClass___ = function() { return idClass; }; imports.setTimeout = tameSetTimeout; imports.setInterval = tameSetInterval; imports.clearTimeout = tameClearTimeout; imports.clearInterval = tameClearInterval; imports.document = new TameDocument(document, true); var tameLocation = ___.primFreeze({ toString: function() { return tameLocation.href; }, href: 'http://nosuchhost,fake/', hash: '', host: 'nosuchhost,fake', hostname: 'nosuchhost,fake', pathname: '/', port: '', protocol: 'http:', search: '' }); var tameNavigator = ___.primFreeze({ appCodeName: 'Caja', appName: 'Sandbox', appVersion: '1.0', language: '', platform: 'Caja', oscpu: 'Caja', vendor: '', vendorSub: '', product: 'Caja', productSub: '', userAgent: 'Caja/1.0' }); var tameWindow = { document: imports.document, location: tameLocation, navigator: tameNavigator, setTimeout: tameSetTimeout, setInterval: tameSetInterval, clearTimeout: tameClearTimeout, clearInterval: tameClearInterval, scrollTo: ___.frozenFunc(function(x, y) { if ('number' === typeof x && 'number' === typeof y && !isNaN(x - y) && imports.isProcessingEvent___) { window.scrollTo(x, y); } }), addEventListener: ___.frozenFunc(function(name, listener, useCapture) { }), removeEventListener: ___.frozenFunc(function(name, listener, useCapture) { }), dispatchEvent: ___.frozenFunc(function(evt) { }) }; tameWindow.top = tameWindow.self = tameWindow.opener = tameWindow.parent = tameWindow.window = tameWindow; cajita.forOwnKeys(nodeClasses, ___.func(function(name, ctor) { ___.primFreeze(ctor); tameWindow[name] = ctor; ___.grantRead(tameWindow, name); })); var defaultNodeClasses = ['HTMLAppletElement', 'HTMLAreaElement', 'HTMLBaseElement', 'HTMLBaseFontElement', 'HTMLBodyElement', 'HTMLBRElement', 'HTMLButtonElement', 'HTMLDirectoryElement', 'HTMLDivElement', 'HTMLDListElement', 'HTMLFieldSetElement', 'HTMLFontElement', 'HTMLFrameElement', 'HTMLFrameSetElement', 'HTMLHeadElement', 'HTMLHeadingElement', 'HTMLHRElement', 'HTMLHtmlElement', 'HTMLIFrameElement', 'HTMLIsIndexElement', 'HTMLLabelElement', 'HTMLLegendElement', 'HTMLLIElement', 'HTMLLinkElement', 'HTMLMapElement', 'HTMLMenuElement', 'HTMLMetaElement', 'HTMLModElement', 'HTMLObjectElement', 'HTMLOListElement', 'HTMLOptGroupElement', 'HTMLOptionElement', 'HTMLParagraphElement', 'HTMLParamElement', 'HTMLPreElement', 'HTMLQuoteElement', 'HTMLScriptElement', 'HTMLSelectElement', 'HTMLStyleElement', 'HTMLTableCaptionElement', 'HTMLTableCellElement', 'HTMLTableColElement', 'HTMLTableElement', 'HTMLTableRowElement', 'HTMLTableSectionElement', 'HTMLTextAreaElement', 'HTMLTitleElement', 'HTMLUListElement']; var defaultNodeClassCtor = ___.primFreeze(TameElement); for (var i = 0; i < defaultNodeClasses.length; i++) { tameWindow[defaultNodeClasses[i]] = defaultNodeClassCtor; ___.grantRead(tameWindow, defaultNodeClasses[i]); }
        var outers = imports.outers; if (___.isJSONContainer(outers)) {
            for (var k in tameWindow) { if (!___.hasOwnProp(outers, k) && ___.canEnumPub(tameWindow, k)) { var v = tameWindow[k]; outers[k] = v === tameWindow ? outers : v; } }
            outers.window = outers;
        } else { cajita.freeze(tameWindow); imports.window = tameWindow; } 
    }
    return attachDocumentStub;
})(); function plugin_dispatchEvent___(thisNode, event, pluginId, handler) {
    event = (event || window.event); (typeof console !== 'undefined' && console.log) && console.log('Dispatch %s event thisNode=%o, event=%o, pluginId=%o, handler=%o', (event && event.type), thisNode, event, pluginId, handler); var imports = ___.getImports(pluginId); switch (typeof handler) { case 'string': handler = imports[handler]; break; case 'function': case 'object': break; default: throw new Error('Expected function as event handler, not ' + typeof handler); }
    ___.startCallerStack && ___.startCallerStack(); imports.isProcessingEvent___ = true; try { return ___.callPub(handler, 'call', [imports.tameNode___(thisNode, true), imports.tameEvent___(event)]); } catch (ex) {
        if (ex && ex.cajitaStack___ && 'undefined' !== (typeof console)) { console.error('Event dispatch %s: %s', handler, ___.unsealCallerStack(ex.cajitaStack___).join('\n')); }
        throw ex;
    } finally { imports.isProcessingEvent___ = false; } 
}