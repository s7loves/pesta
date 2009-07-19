{
    
    {
        Array.typeTag___ = 'Array'; Object.typeTag___ = 'Object'; String.typeTag___ = 'String'; Boolean.typeTag___ = 'Boolean'; Number.typeTag___ = 'Number'; Date.typeTag___ = 'Date'; RegExp.typeTag___ = 'RegExp'; Error.typeTag___ = 'Error'; EvalError.typeTag___ = 'EvalError'; RangeError.typeTag___ = 'RangeError'; ReferenceError.typeTag___ = 'ReferenceError'; SyntaxError.typeTag___ = 'SyntaxError'; TypeError.typeTag___ = 'TypeError'; URIError.typeTag___ = 'URIError'; Object.prototype.proto___ = null; function 
dateToISOString() { function f(n) { return n < 10 ? '0' + n : n; } return this.getUTCFullYear() + '-' + f(this.getUTCMonth() + 1) + '-' + f(this.getUTCDate()) + 'T' + f(this.getUTCHours()) + ':' + f(this.getUTCMinutes()) + ':' + f(this.getUTCSeconds()) + 'Z'; } if (Date.prototype.toISOString === void 
0) { Date.prototype.toISOString = dateToISOString; } if (Date.prototype.toJSON === void 0) { Date.prototype.toJSON = Date.prototype.toISOString; } function 
arraySlice(self, start, end) { return Array.prototype.slice.call(self, start || 0, end || self.length); } if (Array.slice === void 
0) { Array.slice = arraySlice; } function funcBind(self, var_args) {
    var thisFunc = this; var 
leftArgs = Array.slice(arguments, 1); function funcBound(var_args) { var args = leftArgs.concat(Array.slice(arguments, 0)); return thisFunc.apply(self, args); }; return funcBound;
} if (Function.prototype.bind === void 
0) { Function.prototype.bind = funcBind; } var jsonParse = (function() {
    var number = '(?:-?\\b(?:0|[1-9][0-9]*)(?:\\.[0-9]+)?(?:[eE][+-]?[0-9]+)?\\b)'; var 
oneChar = '(?:[^\\0-\\x08\\x0a-\\x1f\"\\\\]' + '|\\\\(?:[\"/\\\\bfnrt]|u[0-9A-Fa-f]{4}))'; var 
string = '(?:\"' + oneChar + '*\")'; var jsonToken = new RegExp('(?:false|true|null|[\\{\\}\\[\\]]' + '|' + number + '|' + string + ')', 'g'); var 
escapeSequence = new RegExp('\\\\(?:([^u])|u(.{4}))', 'g'); var escapes = { '\"': '\"', '/': '/', '\\': '\\', 'b': '\b', 'f': '', 'n': '\n', 'r': '\r', 't': '	' }; function 
unescapeOne(_, ch, hex) { return ch ? escapes[ch] : String.fromCharCode(parseInt(hex, 16)); } var 
EMPTY_STRING = new String(''); var SLASH = '\\'; var firstTokenCtors = { '{': Object, '[': Array }; return function(json) {
    var 
toks = json.match(jsonToken); var result; var tok = toks[0]; if ('{' === tok) { result = {}; } else
        if ('[' === tok) { result = []; } else { throw new Error(tok); } var key; var stack = [result]; for (var 
i = 1, n = toks.length; i < n; ++i) {
        tok = toks[i]; var cont; switch (tok.charCodeAt(0)) {
            default: 
                {
                    cont = stack[0]; cont[key || cont.length] = +tok; key = void 
0; break;
                } case 34: 
                {
                    tok = tok.substring(1, tok.length - 1); if (tok.indexOf(SLASH) !== -1) { tok = tok.replace(escapeSequence, unescapeOne); } cont = stack[0]; if (!key) {
                        if (cont
instanceof Array) { key = cont.length; } else { key = tok || EMPTY_STRING; break; } 
                    } cont[key] = tok; key = void 
0; break;
                } case 91: 
                {
                    cont = stack[0]; stack.unshift(cont[key || cont.length] = []); key = void 
0; break;
                } case 93: { stack.shift(); break; } case 102: 
                {
                    cont = stack[0]; cont[key || cont.length] = false; key = void 
0; break;
                } case 110: { cont = stack[0]; cont[key || cont.length] = null; key = void 0; break; } case 
116: { cont = stack[0]; cont[key || cont.length] = true; key = void 0; break; } case 123: 
                {
                    cont = stack[0]; stack.unshift(cont[key || cont.length] = {}); key = void 
0; break;
                } case 125: { stack.shift(); break; } 
        } 
    } if (stack.length) { throw new Error(); } return result;
};
})(); var 
escape; var cajita; var ___; (function(global) {
    function ToInt32(alleged_int) { return alleged_int >> 0; } function 
ToUInt32(alleged_int) { return alleged_int >>> 0; } function arrayIndexOf(specimen, i) {
    var 
len = ToUInt32(this.length); i = ToInt32(i); if (i < 0) { if ((i += len) < 0) { i = 0; } } for (; i < len; ++i) {
        if (i
in this && identical(this[i], specimen)) { return i; } 
    } return -1;
} Array.prototype.indexOf = arrayIndexOf; function 
arrayLastIndexOf(specimen, i) {
    var len = ToUInt32(this.length); if (isNaN(i)) { i = len - 1; } else {
        i = ToInt32(i); if (i < 0) { i += len; if (i < 0) { return -1; } } else
            if (i >= len) { i = len - 1; } 
    } for (; i >= 0; --i) { if (i in this && identical(this[i], specimen)) { return i; } } return -1;
} Array.prototype.lastIndexOf = arrayLastIndexOf; var 
endsWith_canDelete___ = /_canDelete___$/; var endsWith_canRead___ = /_canRead___$/; var 
endsWith_canSet___ = /_canSet___$/; var endsWith___ = /___$/; var endsWith__ = /__$/; function 
typeOf(obj) {
    var result = typeof obj; if (result !== 'function') { return result; } var ctor = obj.constructor; if (typeof 
ctor === 'function' && ctor.typeTag___ === 'RegExp' && obj instanceof ctor) { return 'object'; } return 'function';
} var 
myOriginalHOP = Object.prototype.hasOwnProperty; function hasOwnProp(obj, name) {
    var t = typeof 
obj; if (t !== 'object' && t !== 'function') { return false; } return myOriginalHOP.call(obj, name);
} function 
identical(x, y) { if (x === y) { return x !== 0 || 1 / x === 1 / y; } else { return x !== x && y !== y; } } function 
callFault(var_args) { return asFunc(this).apply(USELESS, arguments); } Object.prototype.CALL___ = callFault; function 
defaultLogger(str, opt_stop) { } var myLogFunc = frozenFunc(defaultLogger); function getLogFunc() { return myLogFunc; } function 
setLogFunc(newLogFunc) { myLogFunc = newLogFunc; } function log(str) { myLogFunc(String(str)); } function 
fail(var_args) {
    if (typeof console !== 'undefined' && typeOf(console.trace) === 'function') { console.trace(); } var 
message = Array.slice(arguments, 0).join(''); myLogFunc(message, true); throw new Error(message);
} function 
enforce(test, var_args) { return test || fail.apply({}, Array.slice(arguments, 1)); } function 
enforceType(specimen, typename, opt_name) { if (typeOf(specimen) !== typename) { fail('expected ', typename, ' instead of ', typeOf(specimen), ': ', opt_name || specimen); } return specimen; } function 
enforceNat(specimen) { enforceType(specimen, 'number'); if (Math.floor(specimen) !== specimen) { fail('Must be integral: ', specimen); } if (specimen < 0) { fail('Must not be negative: ', specimen); } if (Math.floor(specimen - 1) !== specimen - 1) { fail('Beyond precision limit: ', specimen); } if (Math.floor(specimen - 1) >= specimen) { fail('Must not be infinite: ', specimen); } return specimen; } function 
debugReference(obj) {
    switch (typeOf(obj)) {
        case 'object': 
            {
                if (obj === null) { return '<null>'; } var 
constr = directConstructor(obj); return '[' + (constr && constr.name || 'Object') + ']';
            } default: { return '(' + obj + ':' + typeOf(obj) + ')'; } 
    } 
} var 
myKeeper = { 'toString': function toString() { return '<Logging Keeper>'; }, 'handleRead': function 
handleRead(obj, name) { return void 0; }, 'handleCall': function handleCall(obj, name, args) { fail('Not callable: (', debugReference(obj), ').', name); }, 'handleSet': function 
handleSet(obj, name, val) { fail('Not settable: (', debugReference(obj), ').', name); }, 'handleDelete': function 
handleDelete(obj, name) { fail('Not deletable: (', debugReference(obj), ').', name); } 
}; Object.prototype.handleRead___ = function 
handleRead___(name) { var handlerName = name + '_getter___'; if (this[handlerName]) { return this[handlerName](); } return myKeeper.handleRead(this, name); }; Object.prototype.handleCall___ = function 
handleCall___(name, args) { var handlerName = name + '_handler___'; if (this[handlerName]) { return this[handlerName].call(this, args); } return myKeeper.handleCall(this, name, args); }; Object.prototype.handleSet___ = function 
handleSet___(name, val) { var handlerName = name + '_setter___'; if (this[handlerName]) { return this[handlerName](val); } return myKeeper.handleSet(this, name, val); }; Object.prototype.handleDelete___ = function 
handleDelete___(name) { var handlerName = name + '_deleter___'; if (this[handlerName]) { return this[handlerName](); } return myKeeper.handleDelete(this, name); }; function 
endsWith(str, suffix) { enforceType(str, 'string'); enforceType(suffix, 'string'); var d = str.length - suffix.length; return d >= 0 && str.lastIndexOf(suffix) === d; } function 
directConstructor(obj) {
    if (obj === null) { return void 0; } if (obj === void 0) {
        return void 
0;
    } if (typeOf(obj) === 'function') { return void 0; } obj = Object(obj); var result; if (myOriginalHOP.call(obj, 'proto___')) {
        var 
proto = obj.proto___; if (proto === null) { return void 0; } result = proto.constructor; if (result.prototype !== proto || typeOf(result) !== 'function') { result = directConstructor(proto); } 
    } else {
        if (!myOriginalHOP.call(obj, 'constructor')) { result = obj.constructor; } else {
            var 
oldConstr = obj.constructor; if (delete obj.constructor) { result = obj.constructor; obj.constructor = oldConstr; } else
                if (isPrototypical(obj)) { log('Guessing the directConstructor of : ' + obj); result = Object; } else { fail('Discovery of direct constructors unsupported when the ', 'constructor property is not deletable: ', oldConstr); } 
        } if (typeOf(result) !== 'function' || !(obj
instanceof result)) { fail('Discovery of direct constructors for foreign begotten ', 'objects not implemented on this platform.\n'); } if (result.prototype.constructor === result) { obj.proto___ = result.prototype; } 
    } return result;
} function 
getFuncCategory(fun) { enforceType(fun, 'function'); if (fun.typeTag___) { return fun.typeTag___; } else { return fun; } } function 
isDirectInstanceOf(obj, ctor) { var constr = directConstructor(obj); if (constr === void 0) { return false; } return getFuncCategory(constr) === getFuncCategory(ctor); } function 
isInstanceOf(obj, ctor) { if (obj instanceof ctor) { return true; } if (isDirectInstanceOf(obj, ctor)) { return true; } return false; } function 
isRecord(obj) { return isDirectInstanceOf(obj, Object); } function isArray(obj) { return isDirectInstanceOf(obj, Array); } function 
isJSONContainer(obj) {
    var constr = directConstructor(obj); if (constr === void 0) { return false; } var 
typeTag = constr.typeTag___; if (typeTag !== 'Object' && typeTag !== 'Array') { return false; } return !isPrototypical(obj);
} function 
isFrozen(obj) {
    if (!obj) { return true; } if (obj.FROZEN___ === obj) { return true; } var t = typeof 
obj; return t !== 'object' && t !== 'function';
} function primFreeze(obj) {
    if (isFrozen(obj)) { return obj; } var 
badFlags = []; for (var k in obj) { if (endsWith_canSet___.test(k) || endsWith_canDelete___.test(k)) { if (obj[k]) { badFlags.push(k); } } } for (var 
i = 0; i < badFlags.length; i++) {
        var flag = badFlags[i]; if (myOriginalHOP.call(obj, flag)) {
            if (!(delete 
obj[flag])) { fail('internal: failed delete: ', debugReference(obj), '.', flag); } 
        } if (obj[flag]) { obj[flag] = false; } 
    } obj.FROZEN___ = obj; if (typeOf(obj) === 'function') { if (isFunc(obj)) { grantCall(obj, 'call'); grantCall(obj, 'apply'); obj.CALL___ = obj; } if (obj.prototype) { primFreeze(obj.prototype); } } return obj;
} function 
freeze(obj) { if (!isJSONContainer(obj)) { if (typeOf(obj) === 'function') { enforce(isFrozen(obj), 'Internal: non-frozen function: ' + obj); return obj; } fail('cajita.freeze(obj) applies only to JSON Containers: ', debugReference(obj)); } return primFreeze(obj); } function 
copy(obj) {
    if (!isJSONContainer(obj)) { fail('cajita.copy(obj) applies only to JSON Containers: ', debugReference(obj)); } var 
result = isArray(obj) ? [] : {}; forOwnKeys(obj, frozenFunc(function(k, v) { result[k] = v; })); return result;
} function 
snapshot(obj) { return primFreeze(copy(obj)); } function canRead(obj, name) {
    if (obj === void 
0 || obj === null) { return false; } return !(!obj[name + '_canRead___']);
} function canEnum(obj, name) {
    if (obj === void 
0 || obj === null) { return false; } return !(!obj[name + '_canEnum___']);
} function canCall(obj, name) {
    if (obj === void 
0 || obj === null) { return false; } if (obj[name + '_canCall___']) { return true; } if (obj[name + '_grantCall___']) { fastpathCall(obj, name); return true; } return false;
} function 
canSet(obj, name) { if (obj === void 0 || obj === null) { return false; } if (obj[name + '_canSet___'] === obj) { return true; } if (obj[name + '_grantSet___'] === obj) { fastpathSet(obj, name); return true; } return false; } function 
canDelete(obj, name) { if (obj === void 0 || obj === null) { return false; } return obj[name + '_canDelete___'] === obj; } function 
fastpathRead(obj, name) { if (name === 'toString') { fail('internal: Can\'t fastpath .toString'); } obj[name + '_canRead___'] = obj; } function 
fastpathEnumOnly(obj, name) { obj[name + '_canEnum___'] = obj; } function fastpathCall(obj, name) { if (name === 'toString') { fail('internal: Can\'t fastpath .toString'); } if (obj[name + '_canSet___']) { obj[name + '_canSet___'] = false; } if (obj[name + '_grantSet___']) { obj[name + '_grantSet___'] = false; } obj[name + '_canCall___'] = obj; } function 
fastpathSet(obj, name) { if (name === 'toString') { fail('internal: Can\'t fastpath .toString'); } if (isFrozen(obj)) { fail('Can\'t set .', name, ' on frozen (', debugReference(obj), ')'); } fastpathEnumOnly(obj, name); fastpathRead(obj, name); if (obj[name + '_canCall___']) { obj[name + '_canCall___'] = false; } if (obj[name + '_grantCall___']) { obj[name + '_grantCall___'] = false; } obj[name + '_canSet___'] = obj; } function 
fastpathDelete(obj, name) { if (name === 'toString') { fail('internal: Can\'t fastpath .toString'); } if (isFrozen(obj)) { fail('Can\'t delete .', name, ' on frozen (', debugReference(obj), ')'); } obj[name + '_canDelete___'] = obj; } function 
grantRead(obj, name) { fastpathRead(obj, name); } function grantEnumOnly(obj, name) { fastpathEnumOnly(obj, name); } function 
grantCall(obj, name) { fastpathCall(obj, name); obj[name + '_grantCall___'] = obj; } function 
grantSet(obj, name) { fastpathSet(obj, name); obj[name + '_grantSet___'] = obj; } function grantDelete(obj, name) { fastpathDelete(obj, name); } function 
isCtor(constr) { return constr && !(!constr.CONSTRUCTOR___); } function isFunc(fun) { return fun && !(!fun.FUNC___); } function 
isXo4aFunc(func) { return func && !(!func.XO4A___); } function ctor(constr, opt_Sup, opt_name) { enforceType(constr, 'function', opt_name); if (isFunc(constr)) { fail('Simple functions can\'t be constructors: ', constr); } if (isXo4aFunc(constr)) { fail('Exophoric functions can\'t be constructors: ', constr); } constr.CONSTRUCTOR___ = true; if (opt_Sup) { derive(constr, opt_Sup); } if (opt_name) { constr.NAME___ = String(opt_name); } return constr; } function 
derive(constr, sup) { sup = asCtor(sup); if (isFrozen(constr)) { fail('Derived constructor already frozen: ', constr); } if (!isFrozen(constr.prototype)) { constr.prototype.proto___ = sup.prototype; } } function 
reifyIfXo4a(xfunc, opt_name) {
    if (!isXo4aFunc(xfunc)) { return asFirstClass(xfunc); } var 
result = { 'call': frozenFunc(function callXo4a(self, var_args) {
    if (self === null || self === void 
0) { self = USELESS; } return xfunc.apply(self, Array.slice(arguments, 1));
}), 'apply': frozenFunc(function 
applyXo4a(self, args) { if (self === null || self === void 0) { self = USELESS; } return xfunc.apply(self, args); }), 'bind': frozenFunc(function 
bindXo4a(self, var_args) { var args = arguments; if (self === null || self === void 0) { self = USELESS; args = [self].concat(Array.slice(args, 1)); } return frozenFunc(xfunc.bind.apply(xfunc, args)); }), 'length': xfunc.length, 'toString': frozenFunc(function 
xo4aToString() { return xfunc.toString(); })
}; if (opt_name !== void 0) { result.name = opt_name; } return primFreeze(result);
} function 
xo4a(func, opt_name) { enforceType(func, 'function', opt_name); if (isCtor(func)) { fail('Internal: Constructors can\'t be exophora: ', func); } if (isFunc(func)) { fail('Internal: Simple functions can\'t be exophora: ', func); } func.XO4A___ = true; return primFreeze(func); } function 
func(fun, opt_name) { enforceType(fun, 'function', opt_name); if (isCtor(fun)) { fail('Constructors can\'t be simple functions: ', fun); } if (isXo4aFunc(fun)) { fail('Exophoric functions can\'t be simple functions: ', fun); } fun.FUNC___ = true; if (opt_name) { fun.NAME___ = String(opt_name); } return fun; } function 
frozenFunc(fun, opt_name) { return primFreeze(func(fun, opt_name)); } function asCtorOnly(constr) { if (isCtor(constr) || isFunc(constr)) { return constr; } enforceType(constr, 'function'); fail('Untamed functions can\'t be called as constructors: ', constr); } function 
asCtor(constr) { return primFreeze(asCtorOnly(constr)); } function asFunc(fun) { if (fun && fun.FUNC___) { if (fun.FROZEN___ === fun) { return fun; } else { return primFreeze(fun); } } enforceType(fun, 'function'); if (isCtor(fun)) { if (fun === Number || fun === String || fun === Boolean) { return primFreeze(fun); } fail('Constructors can\'t be called as simple functions: ', fun); } if (isXo4aFunc(fun)) { fail('Exophoric functions can\'t be called as simple functions: ', fun); } fail('Untamed functions can\'t be called as simple functions: ', fun); } function 
isApplicator(funoid) { if (typeof funoid !== 'object') { return false; } if (funoid === null) { return false; } return canCallPub(funoid, 'apply'); } function 
toFunc(fun) { if (isApplicator(fun)) { return frozenFunc(function applier(var_args) { return callPub(fun, 'apply', [USELESS, Array.slice(arguments, 0)]); }); } return asFunc(fun); } function 
isPrototypical(obj) {
    if (typeOf(obj) !== 'object') { return false; } if (obj === null) { return false; } var 
constr = obj.constructor; if (typeOf(constr) !== 'function') { return false; } return constr.prototype === obj;
} function 
asFirstClass(value) {
    switch (typeOf(value)) {
        case 'function': 
            {
                if (isFunc(value) || isCtor(value)) { if (isFrozen(value)) { return value; } fail('Internal: non-frozen function encountered: ', value); } else
                    if (isXo4aFunc(value)) { fail('Internal: toxic exophora encountered: ', value); } else { fail('Internal: toxic function encountered: ', value); } break;
            } case 'object': { if (value !== null && isPrototypical(value)) { fail('Internal: prototypical object encountered: ', value); } return value; } default: { return value; } 
    } 
} function 
canReadPub(obj, name) {
    if (typeof name === 'number') { return name in obj; } name = String(name); if (obj === null) { return false; } if (obj === void 
0) { return false; } if (obj[name + '_canRead___']) { return true; } if (endsWith__.test(name)) { return false; } if (name === 'toString') { return false; } if (!isJSONContainer(obj)) { return false; } if (!myOriginalHOP.call(obj, name)) { return false; } fastpathRead(obj, name); return true;
} function 
hasOwnPropertyOf(obj, name) { if (typeof name === 'number') { return hasOwnProp(obj, name); } name = String(name); if (obj && obj[name + '_canRead___'] === obj) { return true; } return canReadPub(obj, name) && myOriginalHOP.call(obj, name); } function 
inPub(name, obj) {
    if (obj === null || obj === void 0) { throw new TypeError('invalid \"in\" operand: ' + obj); } obj = Object(obj); if (canReadPub(obj, name)) { return true; } if (canCallPub(obj, name)) { return true; } if (name + '_getter___' in
obj) { return true; } if (name + '_handler___' in obj) { return true; } return false;
} function 
readPub(obj, name) {
    if (typeof name === 'number') { if (typeof obj === 'string') { return obj.charAt(name); } else { return obj[name]; } } name = String(name); if (canReadPub(obj, name)) { return obj[name]; } if (obj === null || obj === void 
0) { throw new TypeError('Can\'t read ' + name + ' on ' + obj); } return obj.handleRead___(name);
} function 
readOwn(obj, name, pumpkin) {
    if (typeof obj !== 'object' || !obj) { if (typeOf(obj) !== 'object') { return pumpkin; } } if (typeof 
name === 'number') { if (myOriginalHOP.call(obj, name)) { return obj[name]; } return pumpkin; } name = String(name); if (obj[name + '_canRead___'] === obj) { return obj[name]; } if (!myOriginalHOP.call(obj, name)) { return pumpkin; } if (endsWith__.test(name)) { return pumpkin; } if (name === 'toString') { return pumpkin; } if (!isJSONContainer(obj)) { return pumpkin; } fastpathRead(obj, name); return obj[name];
} function 
enforceStaticPath(result, permitsUsed) { forOwnKeys(permitsUsed, frozenFunc(function(name, subPermits) { enforce(isFrozen(result), 'Assumed frozen: ', result); if (name === '()') { } else { enforce(canReadPub(result, name), 'Assumed readable: ', result, '.', name); if (inPub('()', subPermits)) { enforce(canCallPub(result, name), 'Assumed callable: ', result, '.', name, '()'); } enforceStaticPath(readPub(result, name), subPermits); } })); } function 
readImport(module_imports, name, opt_permitsUsed) {
    var pumpkin = {}; var result = readOwn(module_imports, name, pumpkin); if (result === pumpkin) {
        log('Linkage warning: ' + name + ' not importable'); return void 
0;
    } if (opt_permitsUsed) { enforceStaticPath(result, opt_permitsUsed); } return result;
} function 
canInnocentEnum(obj, name) { name = String(name); if (endsWith___.test(name)) { return false; } return true; } function 
canEnumPub(obj, name) { if (obj === null) { return false; } if (obj === void 0) { return false; } name = String(name); if (obj[name + '_canEnum___']) { return true; } if (endsWith__.test(name)) { return false; } if (!isJSONContainer(obj)) { return false; } if (!myOriginalHOP.call(obj, name)) { return false; } fastpathEnumOnly(obj, name); if (name === 'toString') { return true; } fastpathRead(obj, name); return true; } function 
canEnumOwn(obj, name) { name = String(name); if (obj && obj[name + '_canEnum___'] === obj) { return true; } return canEnumPub(obj, name) && myOriginalHOP.call(obj, name); } function 
Token(name) { name = String(name); return primFreeze({ 'toString': frozenFunc(function tokenToString() { return name; }) }); } frozenFunc(Token); var 
BREAK = Token('BREAK'); var NO_RESULT = Token('NO_RESULT'); function forOwnKeys(obj, fn) {
    fn = toFunc(fn); var 
keys = ownKeys(obj); for (var i = 0; i < keys.length; i++) { if (fn(keys[i], readPub(obj, keys[i])) === BREAK) { return; } } 
} function 
forAllKeys(obj, fn) { fn = toFunc(fn); var keys = allKeys(obj); for (var i = 0; i < keys.length; i++) { if (fn(keys[i], readPub(obj, keys[i])) === BREAK) { return; } } } function 
ownKeys(obj) {
    var result = []; if (isArray(obj)) { var len = obj.length; for (var i = 0; i < len; i++) { result.push(i); } } else {
        for (var 
k in obj) { if (canEnumOwn(obj, k)) { result.push(k); } } if (obj !== void 0 && obj !== null && obj.handleEnum___) { result = result.concat(obj.handleEnum___(true)); } 
    } return result;
} function 
allKeys(obj) {
    if (isArray(obj)) { return ownKeys(obj); } else {
        var result = []; for (var k in
obj) { if (canEnumPub(obj, k)) { result.push(k); } } if (obj !== void 0 && obj !== null && obj.handleEnum___) { result = result.concat(obj.handleEnum___(false)); } return result;
    } 
} function 
canCallPub(obj, name) {
    if (obj === null) { return false; } if (obj === void 0) { return false; } name = String(name); if (obj[name + '_canCall___']) { return true; } if (obj[name + '_grantCall___']) { fastpathCall(obj, name); return true; } if (!canReadPub(obj, name)) { return false; } if (endsWith__.test(name)) { return false; } if (name === 'toString') { return false; } var 
func = obj[name]; if (!isFunc(func) && !isXo4aFunc(func)) { return false; } fastpathCall(obj, name); return true;
} function 
callPub(obj, name, args) { name = String(name); if (obj === null || obj === void 0) { throw new TypeError('Can\'t call ' + name + ' on ' + obj); } if (obj[name + '_canCall___'] || canCallPub(obj, name)) { return obj[name].apply(obj, args); } if (obj.handleCall___) { return obj.handleCall___(name, args); } fail('not callable:', debugReference(obj), '.', name); } function 
canSetPub(obj, name) { name = String(name); if (canSet(obj, name)) { return true; } if (endsWith__.test(name)) { return false; } if (name === 'valueOf') { return false; } if (name === 'toString') { return false; } return !isFrozen(obj) && isJSONContainer(obj); } function 
setPub(obj, name, val) {
    if (typeof name === 'number' && obj instanceof Array && obj.FROZEN___ !== obj) { return obj[name] = val; } name = String(name); if (obj === null || obj === void 
0) { throw new TypeError('Can\'t set ' + name + ' on ' + obj); } if (obj[name + '_canSet___'] === obj) { return obj[name] = val; } else
        if (canSetPub(obj, name)) { fastpathSet(obj, name); return obj[name] = val; } else { return obj.handleSet___(name, val); } 
} function 
canSetStatic(ctor, staticMemberName) {
    staticMemberName = '' + staticMemberName; if (typeOf(ctor) !== 'function') { log('Cannot set static member of non function', ctor); return false; } if (isFrozen(ctor)) { log('Cannot set static member of frozen function', ctor); return false; } if (staticMemberName
in ctor) { log('Cannot override static member ', staticMemberName); return false; } if (endsWith__.test(staticMemberName) || staticMemberName === 'valueOf') { log('Illegal static member name ', staticMemberName); return false; } if (staticMemberName === 'toString') { return false; } return true;
} function 
setStatic(ctor, staticMemberName, staticMemberValue) { staticMemberName = '' + staticMemberName; if (canSetStatic(ctor, staticMemberName)) { ctor[staticMemberName] = staticMemberValue; fastpathEnumOnly(ctor, staticMemberName); fastpathRead(ctor, staticMemberName); } else { ctor.handleSet___(staticMemberName, staticMemberValue); } } function 
canDeletePub(obj, name) { name = String(name); if (isFrozen(obj)) { return false; } if (endsWith__.test(name)) { return false; } if (name === 'valueOf') { return false; } if (name === 'toString') { return false; } if (isJSONContainer(obj)) { return true; } return false; } function 
deletePub(obj, name) { name = String(name); if (obj === null || obj === void 0) { throw new TypeError('Can\'t delete ' + name + ' on ' + obj); } if (canDeletePub(obj, name)) { return deleteFieldEntirely(obj, name); } else { return obj.handleDelete___(name); } } function 
deleteFieldEntirely(obj, name) {
    delete obj[name + '_canRead___']; delete obj[name + '_canEnum___']; delete 
obj[name + '_canCall___']; delete obj[name + '_grantCall___']; delete obj[name + '_grantSet___']; delete 
obj[name + '_canSet___']; delete obj[name + '_canDelete___']; return delete obj[name] || (fail('not deleted: ', name), false);
} function 
args(original) { return primFreeze(Array.slice(original, 0)); } var USELESS = Token('USELESS'); function 
manifest(ignored) { } var callStackSealer = makeSealerUnsealerPair(); function tameException(ex) {
    try {
        switch (typeOf(ex)) {
            case 'object': 
                {
                    if (ex === null) { return null; } if (isInstanceOf(ex, Error)) {
                        var 
message = ex.message || ex.desc; var stack = ex.stack; var name = ex.constructor && ex.constructor.name; message = !message ? void 
0 : '' + message; stack = !stack ? void 0 : callStackSealer.seal('' + stack); name = !name ? void 0 : '' + name; return primFreeze({ 'message': message, 'name': name, 'stack': stack });
                    } return '' + ex;
                } case 'string': ; case 'number': ; case 'boolean': ; case 'undefined': { return ex; } case 'function': 
                {
                    return void 
0;
                } default: { log('Unrecognized exception type ' + typeOf(ex)); return void 0; } 
        } 
    } catch (_) {
        return void 
0;
    } 
} function primBeget(proto) { function F() { } F.prototype = proto; var result = new F(); result.proto___ = proto; return result; } function 
useGetHandler(obj, name, getHandler) { obj[name + '_getter___'] = getHandler; } function useApplyHandler(obj, name, applyHandler) { obj[name + '_handler___'] = applyHandler; } function 
useCallHandler(obj, name, callHandler) { useApplyHandler(obj, name, function callApplier(args) { return callHandler.apply(this, args); }); } function 
useSetHandler(obj, name, setHandler) { obj[name + '_setter___'] = setHandler; } function useDeleteHandler(obj, name, deleteHandler) { obj[name + '_deleter___'] = deleteHandler; } function 
grantFunc(obj, name) { frozenFunc(obj[name], name); grantCall(obj, name); grantRead(obj, name); } function 
grantGeneric(proto, name) {
    var func = xo4a(proto[name], name); grantCall(proto, name); var 
pseudoFunc = reifyIfXo4a(func, name); useGetHandler(proto, name, function xo4aGetter() { return pseudoFunc; });
} function 
handleGeneric(obj, name, func) {
    xo4a(func); useCallHandler(obj, name, func); var pseudoFunc = reifyIfXo4a(func, name); useGetHandler(obj, name, function 
genericGetter() { return pseudoFunc; });
} function grantTypedGeneric(proto, name) {
    var 
original = proto[name]; handleGeneric(proto, name, function guardedApplier(var_args) { if (!inheritsFrom(this, proto)) { fail('Can\'t call .', name, ' on a non ', directConstructor(proto), ': ', this); } return original.apply(this, arguments); });
} function 
grantMutator(proto, name) {
    var original = proto[name]; handleGeneric(proto, name, function 
nonMutatingApplier(var_args) { if (isFrozen(this)) { fail('Can\'t .', name, ' a frozen object'); } return original.apply(this, arguments); });
} function 
enforceMatchable(regexp) { if (isInstanceOf(regexp, RegExp)) { if (isFrozen(regexp)) { fail('Can\'t match with frozen RegExp: ', regexp); } } else { enforceType(regexp, 'string'); } } function 
all2(func2, arg1, arg2s) { var len = arg2s.length; for (var i = 0; i < len; i += 1) { func2(arg1, arg2s[i]); } } all2(grantRead, Math, ['E', 'LN10', 'LN2', 'LOG2E', 'LOG10E', 'PI', 'SQRT1_2', 'SQRT2']); all2(grantFunc, Math, ['abs', 'acos', 'asin', 'atan', 'atan2', 'ceil', 'cos', 'exp', 'floor', 'log', 'max', 'min', 'pow', 'random', 'round', 'sin', 'sqrt', 'tan']); function 
grantToString(proto) { proto.TOSTRING___ = xo4a(proto.toString, 'toString'); } useGetHandler(Object.prototype, 'toString', function 
toStringGetter() { if (hasOwnProp(this, 'toString') && typeOf(this.toString) === 'function' && !hasOwnProp(this, 'TOSTRING___')) { return this.toString; } return reifyIfXo4a(this.TOSTRING___, 'toString'); }); useApplyHandler(Object.prototype, 'toString', function 
toStringApplier(args) { return this.toString.apply(this, args); }); useSetHandler(Object.prototype, 'toString', function 
toStringSetter(meth) {
    if (isFrozen(this)) { return myKeeper.handleSet(this, 'toString', meth); } meth = asFirstClass(meth); this.TOSTRING___ = meth; this.toString = function 
delegatingToString(var_args) {
        var args = Array.slice(arguments, 0); if (typeOf(meth) === 'function') { return meth.apply(this, args); } var 
methApply = readPub(meth, 'apply'); if (typeOf(methApply) === 'function') { return methApply.call(meth, this, args); } var 
result = Object.toString.call(this); log('Not correctly printed: ' + result); return result;
    }; return meth;
}); useDeleteHandler(Object.prototype, 'toString', function 
toStringDeleter() {
    if (isFrozen(this)) { return myKeeper.handleDelete(this, 'toString'); } return delete 
this.toString && delete this.TOSTRING___;
}); ctor(Object, void 0, 'Object'); grantToString(Object.prototype); all2(grantGeneric, Object.prototype, ['toLocaleString', 'valueOf', 'isPrototypeOf']); grantRead(Object.prototype, 'length'); handleGeneric(Object.prototype, 'hasOwnProperty', function 
hasOwnPropertyHandler(name) { return hasOwnPropertyOf(this, name); }); handleGeneric(Object.prototype, 'propertyIsEnumerable', function 
propertyIsEnumerableHandler(name) { name = String(name); return canEnumPub(this, name); }); handleGeneric(Function.prototype, 'apply', function 
applyHandler(self, realArgs) { return toFunc(this).apply(self, realArgs); }); handleGeneric(Function.prototype, 'call', function 
callHandler(self, var_args) { return toFunc(this).apply(self, Array.slice(arguments, 1)); }); handleGeneric(Function.prototype, 'bind', function 
bindHandler(self, var_args) {
    var thisFunc = this; var leftArgs = Array.slice(arguments, 1); function 
boundHandler(var_args) { var args = leftArgs.concat(Array.slice(arguments, 0)); return callPub(thisFunc, 'apply', [self, args]); } return frozenFunc(boundHandler);
}); ctor(Array, Object, 'Array'); grantFunc(Array, 'slice'); grantToString(Array.prototype); all2(grantTypedGeneric, Array.prototype, ['toLocaleString']); all2(grantGeneric, Array.prototype, ['concat', 'join', 'slice', 'indexOf', 'lastIndexOf']); all2(grantMutator, Array.prototype, ['pop', 'push', 'reverse', 'shift', 'splice', 'unshift']); handleGeneric(Array.prototype, 'sort', function 
sortHandler(comparator) { if (isFrozen(this)) { fail('Can\'t sort a frozen array.'); } if (comparator) { return Array.prototype.sort.call(this, toFunc(comparator)); } else { return Array.prototype.sort.call(this); } }); ctor(String, Object, 'String'); grantFunc(String, 'fromCharCode'); grantToString(String.prototype); all2(grantTypedGeneric, String.prototype, ['toLocaleString', 'indexOf', 'lastIndexOf']); all2(grantGeneric, String.prototype, ['charAt', 'charCodeAt', 'concat', 'localeCompare', 'slice', 'substr', 'substring', 'toLowerCase', 'toLocaleLowerCase', 'toUpperCase', 'toLocaleUpperCase']); handleGeneric(String.prototype, 'match', function 
matchHandler(regexp) { enforceMatchable(regexp); return this.match(regexp); }); handleGeneric(String.prototype, 'replace', function 
replaceHandler(searcher, replacement) {
    enforceMatchable(searcher); if (isFunc(replacement)) { replacement = asFunc(replacement); } else
        if (isApplicator(replacement)) { replacement = toFunc(replacement); } else { replacement = '' + replacement; } return this.replace(searcher, replacement);
}); handleGeneric(String.prototype, 'search', function 
searchHandler(regexp) { enforceMatchable(regexp); return this.search(regexp); }); handleGeneric(String.prototype, 'split', function 
splitHandler(separator, limit) { enforceMatchable(separator); return this.split(separator, limit); }); ctor(Boolean, Object, 'Boolean'); grantToString(Boolean.prototype); ctor(Number, Object, 'Number'); all2(grantRead, Number, ['MAX_VALUE', 'MIN_VALUE', 'NaN', 'NEGATIVE_INFINITY', 'POSITIVE_INFINITY']); grantToString(Number.prototype); all2(grantTypedGeneric, Number.prototype, ['toFixed', 'toExponential', 'toPrecision']); ctor(Date, Object, 'Date'); grantFunc(Date, 'parse'); grantFunc(Date, 'UTC'); grantToString(Date.prototype); all2(grantTypedGeneric, Date.prototype, ['toDateString', 'toTimeString', 'toUTCString', 'toLocaleString', 'toLocaleDateString', 'toLocaleTimeString', 'toISOString', 'getDay', 'getUTCDay', 'getTimezoneOffset', 'getTime', 'getFullYear', 'getUTCFullYear', 'getMonth', 'getUTCMonth', 'getDate', 'getUTCDate', 'getHours', 'getUTCHours', 'getMinutes', 'getUTCMinutes', 'getSeconds', 'getUTCSeconds', 'getMilliseconds', 'getUTCMilliseconds']); all2(grantMutator, Date.prototype, ['setTime', 'setFullYear', 'setUTCFullYear', 'setMonth', 'setUTCMonth', 'setDate', 'setUTCDate', 'setHours', 'setUTCHours', 'setMinutes', 'setUTCMinutes', 'setSeconds', 'setUTCSeconds', 'setMilliseconds', 'setUTCMilliseconds']); ctor(RegExp, Object, 'RegExp'); grantToString(RegExp.prototype); handleGeneric(RegExp.prototype, 'exec', function 
execHandler(specimen) { if (isFrozen(this)) { fail('Can\'t .exec a frozen RegExp'); } specimen = String(specimen); return this.exec(specimen); }); handleGeneric(RegExp.prototype, 'test', function 
testHandler(specimen) { if (isFrozen(this)) { fail('Can\'t .test a frozen RegExp'); } specimen = String(specimen); return this.test(specimen); }); all2(grantRead, RegExp.prototype, ['source', 'global', 'ignoreCase', 'multiline', 'lastIndex']); ctor(Error, Object, 'Error'); grantToString(Error.prototype); grantRead(Error.prototype, 'name'); grantRead(Error.prototype, 'message'); ctor(EvalError, Error, 'EvalError'); ctor(RangeError, Error, 'RangeError'); ctor(ReferenceError, Error, 'ReferenceError'); ctor(SyntaxError, Error, 'SyntaxError'); ctor(TypeError, Error, 'TypeError'); ctor(URIError, Error, 'URIError'); var 
sharedImports; var myNewModuleHandler; function getNewModuleHandler() { return myNewModuleHandler; } function 
setNewModuleHandler(newModuleHandler) { myNewModuleHandler = newModuleHandler; } var obtainNewModule = freeze({ 'handle': frozenFunc(function 
handleOnly(newModule) { return newModule; })
}); function makeNormalNewModuleHandler() {
    var 
imports = copy(sharedImports); var lastOutcome = void 0; return freeze({ 'getImports': frozenFunc(function 
getImports() { return imports; }), 'setImports': frozenFunc(function setImports(newImports) { imports = newImports; }), 'getLastOutcome': frozenFunc(function 
getLastOutcome() { return lastOutcome; }), 'getLastValue': frozenFunc(function getLastValue() {
    if (lastOutcome && lastOutcome[0]) { return lastOutcome[1]; } else {
        return void 
0;
    } 
}), 'handle': frozenFunc(function handle(newModule) {
    lastOutcome = void 0; try { var result = newModule(___, imports); if (result !== NO_RESULT) { lastOutcome = [true, result]; } } catch (ex) { lastOutcome = [false, ex]; } if (lastOutcome) { if (lastOutcome[0]) { return lastOutcome[1]; } else { throw lastOutcome[1]; } } else {
        return void 
0;
    } 
}), 'handleUncaughtException': function handleUncaughtException(exception, onerror, source, lineNum) {
    lastOutcome = [false, exception]; tameException(exception); var 
message = 'unknown'; if ('object' === typeOf(exception) && exception !== null) { message = String(exception.message || exception.desc || message); } if (isApplicator(onerror)) { onerror = toFunc(onerror); } var 
shouldReport = isFunc(onerror) ? onerror.CALL___(message, String(source), String(lineNum)) : onerror !== null; if (shouldReport !== false) { cajita.log(source + ':' + lineNum + ': ' + message); } 
} 
});
} function 
loadModule(module) { return callPub(myNewModuleHandler, 'handle', [frozenFunc(module)]); } var 
registeredImports = []; function getId(imports) {
    enforceType(imports, 'object', 'imports'); var 
id; if ('id___' in imports) { id = enforceType(imports.id___, 'number', 'id'); } else { id = imports.id___ = registeredImports.length; } registeredImports[id] = imports; return id;
} function 
getImports(id) {
    var result = registeredImports[enforceType(id, 'number', 'id')]; if (result === void 
0) { fail('imports#', id, ' unregistered'); } return result;
} function unregister(imports) {
    enforceType(imports, 'object', 'imports'); if ('id___' in
imports) {
        var id = enforceType(imports.id___, 'number', 'id'); registeredImports[id] = void 
0;
    } 
} function hasTrademark(trademark, obj) {
    if (!hasOwnProp(obj, 'trademarks___')) { return false; } var 
list = obj.trademarks___; for (var i = 0; i < list.length; ++i) { if (list[i] === trademark) { return true; } } return false;
} function 
guard(trademark, obj) { if (!hasTrademark(trademark, obj)) { fail('This object does not have the given trademark'); } } function 
stamp(trademark, obj, opt_allow_constructed) {
    if (typeOf(trademark) !== 'object') { fail('The supplied trademark is not an object.'); } if (isFrozen(obj)) { fail('The supplied object ' + obj + ' is frozen.'); } if (!isJSONContainer(obj) && typeOf(obj) !== 'function' && !obj.underConstruction___ && !opt_allow_constructed) { fail('The supplied object ', obj, ' has already been constructed and may not be stamped.'); } var 
list = obj.underConstruction___ ? 'delayedTrademarks___' : 'trademarks___'; if (!obj[list]) { obj[list] = []; } obj[list].push(trademark); return obj;
} function 
initializeMap(list) { var result = {}; for (var i = 0; i < list.length; i += 2) { setPub(result, list[i], asFirstClass(list[i + 1])); } return result; } function 
makeSealerUnsealerPair() {
    var flag = false; var squirrel = null; function seal(payload) {
        function 
box() { flag = true; squirrel = payload; } box.toString = frozenFunc(function toString() { return '(box)'; }); return frozenFunc(box);
    } function 
unseal(box) { flag = false; squirrel = null; try { box.CALL___(); if (!flag) { throw new Error('Sealer/Unsealer mismatch'); } return squirrel; } finally { flag = false; squirrel = null; } } return freeze({ 'seal': frozenFunc(seal), 'unseal': frozenFunc(unseal) });
} function 
construct(ctor, args) {
    ctor = asCtor(ctor); switch (args.length) {
        case 0: return new ctor(); case 
1: return new ctor(args[0]); case 2: return new ctor(args[0], args[1]); case 3: return new 
ctor(args[0], args[1], args[2]); case 4: return new ctor(args[0], args[1], args[2], args[3]); case 
5: return new ctor(args[0], args[1], args[2], args[3], args[4]); case 6: return new ctor(args[0], args[1], args[2], args[3], args[4], args[5]); case 
7: return new ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6]); case 8: return new 
ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); case 9: return new 
ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]); case 
10: return new ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]); case 
11: return new ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]); case 
12: return new ctor(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]); default: 
            {
                if (ctor.typeTag___ === 'Array') { return ctor.apply(USELESS, args); } var 
tmp = function tmp(args) { return ctor.apply(this, args); }; tmp.prototype = ctor.prototype; return new 
tmp(args);
            } 
    } 
} var magicCount = 0; var MAGIC_NUM = Math.random(); var MAGIC_TOKEN = Token('MAGIC_TOKEN_FOR:' + MAGIC_NUM); var 
MAGIC_NAME = '_index;' + MAGIC_NUM + ';'; function newTable(opt_useKeyLifetime) {
    magicCount++; var 
myMagicIndexName = MAGIC_NAME + magicCount + '___'; function setOnKey(key, value) {
    if (key !== Object(key)) { fail('Can\'t use key lifetime on primitive keys: ', key); } var 
list = key[myMagicIndexName]; if (!list) { key[myMagicIndexName] = [MAGIC_TOKEN, value]; } else {
        var 
i = 0; for (; i < list.length; i += 2) { if (list[i] === MAGIC_TOKEN) { break; } } list[i] = MAGIC_TOKEN; list[i + 1] = value;
    } 
} function 
getOnKey(key) {
    if (key !== Object(key)) { fail('Can\'t use key lifetime on primitive keys: ', key); } var 
list = key[myMagicIndexName]; if (!list) { return void 0; } else {
        var i = 0; for (; i < list.length; i += 2) { if (list[i] === MAGIC_TOKEN) { return list[i + 1]; } } return void 
0;
    } 
} if (opt_useKeyLifetime) { return primFreeze({ 'set': frozenFunc(setOnKey), 'get': frozenFunc(getOnKey) }); } var 
myValues = []; function setOnTable(key, value) {
    switch (typeof key) {
        case 'object': ; case 'function': 
            {
                if (null === key) { myValues.prim_null = value; return; } var 
index = getOnKey(key); if (index === void 0) { index = myValues.length; setOnKey(key, index); } myValues[index] = value; return;
            } case 'string': { myValues['str_' + key] = value; return; } default: { myValues['prim_' + key] = value; return; } 
    } 
} function 
getOnTable(key) {
    switch (typeof key) {
        case 'object': ; case 'function': 
            {
                if (null === key) { return myValues.prim_null; } var 
index = getOnKey(key); if (void 0 === index) { return void 0; } return myValues[index];
            } case 'string': { return myValues['str_' + key]; } default: { return myValues['prim_' + key]; } 
    } 
} return primFreeze({ 'set': frozenFunc(setOnTable), 'get': frozenFunc(getOnTable) });
} function 
inheritsFrom(obj, allegedParent) {
    if (null === obj) { return false; } if (void 0 === obj) { return false; } if (typeOf(obj) === 'function') { return false; } if (typeOf(allegedParent) !== 'object') { return false; } if (null === allegedParent) { return false; } function 
F() { } F.prototype = allegedParent; return Object(obj) instanceof F;
} function getSuperCtor(func) {
    enforceType(func, 'function'); if (isCtor(func) || isFunc(func)) {
        var 
result = directConstructor(func.prototype); if (isCtor(result) || isFunc(result)) { return result; } 
    } return void 
0;
} var attribute = new RegExp('^([\\s\\S]*)_(?:canRead|canCall|getter|handler)___$'); function 
getOwnPropertyNames(obj) {
    var result = []; var seen = {}; var implicit = isJSONContainer(obj); for (var 
k in obj) {
        if (hasOwnProp(obj, k)) {
            if (implicit && !endsWith__.test(k)) { if (!myOriginalHOP.call(seen, k)) { seen[k] = true; result.push(k); } } else {
                var 
match = attribute.exec(k); if (match !== null) { var base = match[1]; if (!myOriginalHOP.call(seen, base)) { seen[base] = true; result.push(base); } } 
            } 
        } 
    } return result;
} function 
getProtoPropertyNames(func) { enforceType(func, 'function'); return getOwnPropertyNames(func.prototype); } function 
getProtoPropertyValue(func, name) { return asFirstClass(readPub(func.prototype, name)); } function 
beget(parent) { if (!isRecord(parent)) { fail('Can only beget() records: ', parent); } return primBeget(parent); } cajita = { 'log': log, 'fail': fail, 'enforce': enforce, 'enforceType': enforceType, 'enforceNat': enforceNat, 'directConstructor': directConstructor, 'getFuncCategory': getFuncCategory, 'isDirectInstanceOf': isDirectInstanceOf, 'isInstanceOf': isInstanceOf, 'isRecord': isRecord, 'isArray': isArray, 'isJSONContainer': isJSONContainer, 'freeze': freeze, 'isFrozen': isFrozen, 'copy': copy, 'snapshot': snapshot, 'canReadPub': canReadPub, 'readPub': readPub, 'hasOwnPropertyOf': hasOwnPropertyOf, 'readOwn': readOwn, 'canEnumPub': canEnumPub, 'canEnumOwn': canEnumOwn, 'canInnocentEnum': canInnocentEnum, 'BREAK': BREAK, 'allKeys': allKeys, 'forAllKeys': forAllKeys, 'ownKeys': ownKeys, 'forOwnKeys': forOwnKeys, 'canCallPub': canCallPub, 'callPub': callPub, 'canSetPub': canSetPub, 'setPub': setPub, 'canDeletePub': canDeletePub, 'deletePub': deletePub, 'hasTrademark': hasTrademark, 'guard': guard, 'makeSealerUnsealerPair': makeSealerUnsealerPair, 'USELESS': USELESS, 'manifest': manifest, 'construct': construct, 'newTable': newTable, 'inheritsFrom': inheritsFrom, 'getSuperCtor': getSuperCtor, 'getOwnPropertyNames': getOwnPropertyNames, 'getProtoPropertyNames': getProtoPropertyNames, 'getProtoPropertyValue': getProtoPropertyValue, 'beget': beget }; forOwnKeys(cajita, frozenFunc(function(k, v) { switch (typeOf(v)) { case 'object': { if (v !== null) { primFreeze(v); } break; } case 'function': { frozenFunc(v); break; } } })); safeJSON = { 'parse': frozenFunc(jsonParse), 'stringify': frozenFunc(function(obj) { return ''; }) }; sharedImports = { 'cajita': cajita, 'null': null, 'false': false, 'true': true, 'NaN': NaN, 'Infinity': Infinity, 'undefined': void 
0, 'parseInt': frozenFunc(parseInt), 'parseFloat': frozenFunc(parseFloat), 'isNaN': frozenFunc(isNaN), 'isFinite': frozenFunc(isFinite), 'decodeURI': frozenFunc(decodeURI), 'decodeURIComponent': frozenFunc(decodeURIComponent), 'encodeURI': frozenFunc(encodeURI), 'encodeURIComponent': frozenFunc(encodeURIComponent), 'escape': escape ? frozenFunc(escape) : void 
0, 'Math': Math, 'JSON': safeJSON, 'Object': Object, 'Array': Array, 'String': String, 'Boolean': Boolean, 'Number': Number, 'Date': Date, 'RegExp': RegExp, 'Error': Error, 'EvalError': EvalError, 'RangeError': RangeError, 'ReferenceError': ReferenceError, 'SyntaxError': SyntaxError, 'TypeError': TypeError, 'URIError': URIError
}; forOwnKeys(sharedImports, frozenFunc(function(k, v) { switch (typeOf(v)) { case 'object': { if (v !== null) { primFreeze(v); } break; } case 'function': { primFreeze(v); break; } } })); primFreeze(sharedImports); ___ = { 'getLogFunc': getLogFunc, 'setLogFunc': setLogFunc, 'primFreeze': primFreeze, 'canRead': canRead, 'grantRead': grantRead, 'canEnum': canEnum, 'grantEnumOnly': grantEnumOnly, 'canCall': canCall, 'grantCall': grantCall, 'canSet': canSet, 'grantSet': grantSet, 'canDelete': canDelete, 'grantDelete': grantDelete, 'readImport': readImport, 'isCtor': isCtor, 'isFunc': isFunc, 'isXo4aFunc': isXo4aFunc, 'ctor': ctor, 'func': func, 'frozenFunc': frozenFunc, 'asFunc': asFunc, 'toFunc': toFunc, 'xo4a': xo4a, 'initializeMap': initializeMap, 'inPub': inPub, 'canSetStatic': canSetStatic, 'setStatic': setStatic, 'typeOf': typeOf, 'hasOwnProp': hasOwnProp, 'identical': identical, 'args': args, 'tameException': tameException, 'primBeget': primBeget, 'callStackUnsealer': callStackSealer.unseal, 'RegExp': RegExp, 'stamp': stamp, 'asFirstClass': asFirstClass, 'useGetHandler': useGetHandler, 'useApplyHandler': useApplyHandler, 'useCallHandler': useCallHandler, 'useSetHandler': useSetHandler, 'useDeleteHandler': useDeleteHandler, 'grantFunc': grantFunc, 'grantGeneric': grantGeneric, 'handleGeneric': handleGeneric, 'grantTypedGeneric': grantTypedGeneric, 'grantMutator': grantMutator, 'enforceMatchable': enforceMatchable, 'all2': all2, 'sharedImports': sharedImports, 'getNewModuleHandler': getNewModuleHandler, 'setNewModuleHandler': setNewModuleHandler, 'obtainNewModule': obtainNewModule, 'makeNormalNewModuleHandler': makeNormalNewModuleHandler, 'loadModule': loadModule, 'NO_RESULT': NO_RESULT, 'getId': getId, 'getImports': getImports, 'unregister': unregister }; forOwnKeys(cajita, frozenFunc(function(k, v) {
    if (k
in ___) { fail('internal: initialization conflict: ', k); } if (typeOf(v) === 'function') { grantFunc(cajita, k); } ___[k] = v;
})); setNewModuleHandler(makeNormalNewModuleHandler());
})(this);
    } 
    {
        var 
unicode = {}; unicode.BASE_CHAR = 'A-Za-z\xc0-\xd6\xd8-\xf6\xf8-\xff' + '\u0100-\u0131\u0134-\u013e\u0141-\u0148\u014a-\u017e\u0180-\u01c3' + '\u01cd-\u01f0\u01f4-\u01f5\u01fa-\u0217\u0250-\u02a8\u02bb-\u02c1' + '\u0386\u0388-\u038a\u038c\u038e-\u03a1\u03a3-\u03ce\u03d0-\u03d6' + '\u03da\u03dc\u03de\u03e0\u03e2-\u03f3\u0401-\u040c\u040e-\u044f' + '\u0451-\u045c\u045e-\u0481\u0490-\u04c4\u04c7-\u04c8\u04cb-\u04cc' + '\u04d0-\u04eb\u04ee-\u04f5\u04f8-\u04f9\u0531-\u0556\u0559' + '\u0561-\u0586\u05d0-\u05ea\u05f0-\u05f2\u0621-\u063a\u0641-\u064a' + '\u0671-\u06b7\u06ba-\u06be\u06c0-\u06ce\u06d0-\u06d3\u06d5' + '\u06e5-\u06e6\u0905-\u0939\u093d\u0958-\u0961\u0985-\u098c' + '\u098f-\u0990\u0993-\u09a8\u09aa-\u09b0\u09b2\u09b6-\u09b9' + '\u09dc-\u09dd\u09df-\u09e1\u09f0-\u09f1\u0a05-\u0a0a\u0a0f-\u0a10' + '\u0a13-\u0a28\u0a2a-\u0a30\u0a32-\u0a33\u0a35-\u0a36\u0a38-\u0a39' + '\u0a59-\u0a5c\u0a5e\u0a72-\u0a74\u0a85-\u0a8b\u0a8d\u0a8f-\u0a91' + '\u0a93-\u0aa8\u0aaa-\u0ab0\u0ab2-\u0ab3\u0ab5-\u0ab9\u0abd\u0ae0' + '\u0b05-\u0b0c\u0b0f-\u0b10\u0b13-\u0b28\u0b2a-\u0b30\u0b32-\u0b33' + '\u0b36-\u0b39\u0b3d\u0b5c-\u0b5d\u0b5f-\u0b61\u0b85-\u0b8a' + '\u0b8e-\u0b90\u0b92-\u0b95\u0b99-\u0b9a\u0b9c\u0b9e-\u0b9f' + '\u0ba3-\u0ba4\u0ba8-\u0baa\u0bae-\u0bb5\u0bb7-\u0bb9\u0c05-\u0c0c' + '\u0c0e-\u0c10\u0c12-\u0c28\u0c2a-\u0c33\u0c35-\u0c39\u0c60-\u0c61' + '\u0c85-\u0c8c\u0c8e-\u0c90\u0c92-\u0ca8\u0caa-\u0cb3\u0cb5-\u0cb9' + '\u0cde\u0ce0-\u0ce1\u0d05-\u0d0c\u0d0e-\u0d10\u0d12-\u0d28' + '\u0d2a-\u0d39\u0d60-\u0d61\u0e01-\u0e2e\u0e30\u0e32-\u0e33' + '\u0e40-\u0e45\u0e81-\u0e82\u0e84\u0e87-\u0e88\u0e8a\u0e8d' + '\u0e94-\u0e97\u0e99-\u0e9f\u0ea1-\u0ea3\u0ea5\u0ea7\u0eaa-\u0eab' + '\u0ead-\u0eae\u0eb0\u0eb2-\u0eb3\u0ebd\u0ec0-\u0ec4\u0f40-\u0f47' + '\u0f49-\u0f69\u10a0-\u10c5\u10d0-\u10f6\u1100\u1102-\u1103' + '\u1105-\u1107\u1109\u110b-\u110c\u110e-\u1112\u113c\u113e\u1140' + '\u114c\u114e\u1150\u1154-\u1155\u1159\u115f-\u1161\u1163\u1165' + '\u1167\u1169\u116d-\u116e\u1172-\u1173\u1175\u119e\u11a8\u11ab' + '\u11ae-\u11af\u11b7-\u11b8\u11ba\u11bc-\u11c2\u11eb\u11f0\u11f9' + '\u1e00-\u1e9b\u1ea0-\u1ef9\u1f00-\u1f15\u1f18-\u1f1d\u1f20-\u1f45' + '\u1f48-\u1f4d\u1f50-\u1f57\u1f59\u1f5b\u1f5d\u1f5f-\u1f7d' + '\u1f80-\u1fb4\u1fb6-\u1fbc\u1fbe\u1fc2-\u1fc4\u1fc6-\u1fcc' + '\u1fd0-\u1fd3\u1fd6-\u1fdb\u1fe0-\u1fec\u1ff2-\u1ff4\u1ff6-\u1ffc' + '\u2126\u212a-\u212b\u212e\u2180-\u2182\u3041-\u3094\u30a1-\u30fa' + '\u3105-\u312c\uac00-\ud7a3'; unicode.IDEOGRAPHIC = '\u4e00-\u9fa5\u3007\u3021-\u3029'; unicode.LETTER = unicode.BASE_CHAR + unicode.IDEOGRAPHIC; unicode.COMBINING_CHAR = '\u0300-\u0345\u0360-\u0361\u0483-\u0486\u0591-\u05a1\u05a3-\u05b9' + '\u05bb-\u05bd\u05bf\u05c1-\u05c2\u05c4\u064b-\u0652\u0670' + '\u06d6-\u06dc\u06dd-\u06df\u06e0-\u06e4\u06e7-\u06e8\u06ea-\u06ed' + '\u0901-\u0903\u093c\u093e-\u094c\u094d\u0951-\u0954\u0962-\u0963' + '\u0981-\u0983\u09bc\u09be\u09bf\u09c0-\u09c4\u09c7-\u09c8' + '\u09cb-\u09cd\u09d7\u09e2-\u09e3\u0a02\u0a3c\u0a3e\u0a3f' + '\u0a40-\u0a42\u0a47-\u0a48\u0a4b-\u0a4d\u0a70-\u0a71\u0a81-\u0a83' + '\u0abc\u0abe-\u0ac5\u0ac7-\u0ac9\u0acb-\u0acd\u0b01-\u0b03\u0b3c' + '\u0b3e-\u0b43\u0b47-\u0b48\u0b4b-\u0b4d\u0b56-\u0b57\u0b82-\u0b83' + '\u0bbe-\u0bc2\u0bc6-\u0bc8\u0bca-\u0bcd\u0bd7\u0c01-\u0c03' + '\u0c3e-\u0c44\u0c46-\u0c48\u0c4a-\u0c4d\u0c55-\u0c56\u0c82-\u0c83' + '\u0cbe-\u0cc4\u0cc6-\u0cc8\u0cca-\u0ccd\u0cd5-\u0cd6\u0d02-\u0d03' + '\u0d3e-\u0d43\u0d46-\u0d48\u0d4a-\u0d4d\u0d57\u0e31\u0e34-\u0e3a' + '\u0e47-\u0e4e\u0eb1\u0eb4-\u0eb9\u0ebb-\u0ebc\u0ec8-\u0ecd' + '\u0f18-\u0f19\u0f35\u0f37\u0f39\u0f3e\u0f3f\u0f71-\u0f84' + '\u0f86-\u0f8b\u0f90-\u0f95\u0f97\u0f99-\u0fad\u0fb1-\u0fb7\u0fb9' + '\u20d0-\u20dc\u20e1\u302a-\u302f\u3099\u309a', unicode.DIGIT = '0-9\u0660-\u0669\u06f0-\u06f9\u0966-\u096f\u09e6-\u09ef' + '\u0a66-\u0a6f\u0ae6-\u0aef\u0b66-\u0b6f\u0be7-\u0bef\u0c66-\u0c6f' + '\u0ce6-\u0cef\u0d66-\u0d6f\u0e50-\u0e59\u0ed0-\u0ed9\u0f20-\u0f29'; unicode.EXTENDER = '\xb7\u02d0\u02d1\u0387\u0640\u0e46\u0ec6\u3005\u3031-\u3035' + '\u309d-\u309e\u30fc-\u30fe';
    } 
    {
        function 
HtmlEmitter(base) { if (!base) { throw new Error(); } this.cursor_ = [base]; } HtmlEmitter.prototype = { 'top_': function() { return this.cursor_[this.cursor_.length - 1]; }, 'doc_': function() { return this.cursor_[0].ownerDocument || document; }, 'b': function(tagName, unary) { this.cursor_.push(this.doc_().createElement(tagName)); return this; }, 'e': function(tagName) { --this.cursor_.length; return this; }, 'f': function(unary) {
    if (unary) {
        var 
child = this.cursor_.pop(); this.top_().appendChild(child);
    } else { var topIdx = this.cursor_.length - 1; this.cursor_[topIdx - 1].appendChild(this.cursor_[topIdx]); } return this;
}, 'a': function(name, value) {
    var 
node = this.top_(); bridal.setAttribute(node, name, value); switch (name) { case 'value': { if ('INPUT' === node.tagName || 'TEXTAREA' == node.tagName) { node.defaultValue = value; } break; } case 'checked': { if ('INPUT' === node.tagName) { node.defaultChecked = !(!value); } break; } case 'selected': { if ('OPTION' === node.tagName) { node.defaultSelected = !(!value); } break; } } return this;
}, 'h': function(handlerName, handlerBody) {
    var 
node = this.top_(); bridal.setAttribute(node, handlerName, handlerBody); node[handlerName] = new 
Function('event', handlerBody); return this;
}, 'pc': function(text) { this.top_().appendChild(this.doc_().createTextNode(text)); return this; }, 'cd': function(text) { this.top_().appendChild(this.doc_().createTextNode(text)); return this; }, 'ih': function(html) {
    var 
top = this.top_(); if (top.firstChild) { var container = this.doc_().createElement(top.nodeName); container.innerHTML = html; while (container.firstChild) { top.appendChild(container.firstChild); } } else { top.innerHTML = html; } return this;
} 
};
    } 
    {
        var 
bridal = (function() {
    var features = { 'attachEvent': !(!document.createElement('div').attachEvent), 'setAttributeExtraParam': new 
RegExp('Internet Explorer').test(navigator.appName)
    }; function addEventListener(element, type, handler, useCapture) { if (features.attachEvent) { element.attachEvent('on' + type, handler); } else { element.addEventListener(type, handler, useCapture); } } function 
removeEventListener(element, type, handler, useCapture) { if (features.attachEvent) { element.detachEvent('on' + type, handler); } else { element.removeEventListener(type, handler, useCapture); } } function 
cloneNode(node, deep) { var clone; if (!document.all) { clone = node.cloneNode(deep); } else { clone = constructClone(node, deep); } fixupClone(node, clone); return clone; } var 
endsWith__ = /__$/; function constructClone(node, deep) {
    var clone; if (node.nodeType === 1) {
        var 
tagDesc = node.tagName; switch (node.tagName) { case 'INPUT': { tagDesc = '<input name=\"' + html.escapeAttrib(node.name) + '\" type=\"' + html.escapeAttrib(node.type) + '\" value=\"' + html.escapeAttrib(node.defaultValue) + '\"' + (node.defaultChecked ? ' checked=\"checked\">' : '>'); break; } case 'OPTION': { tagDesc = '<option ' + (node.defaultSelected ? ' selected=\"selected\">' : '>'); break; } case 'TEXTAREA': { tagDesc = '<textarea value=\"' + html.escapeAttrib(node.defaultValue) + '\">'; break; } } clone = document.createElement(tagDesc); var 
attrs = node.attributes; for (var i = 0, attr; attr = attrs[i]; ++i) { if (attr.specified && !endsWith__.test(attr.name)) { clone.setAttribute(attr.nodeName, attr.nodeValue); } } 
    } else { clone = node.cloneNode(false); } if (deep) {
        for (var 
child = node.firstChild; child; child = child.nextSibling) { var cloneChild = constructClone(child, deep); clone.appendChild(cloneChild); } 
    } return clone;
} function 
fixupClone(node, clone) {
    for (var child = node.firstChild, cloneChild = clone.firstChild; cloneChild; child = child.nextSibling, cloneChild = cloneChild.nextSibling) { fixupClone(child, cloneChild); } if (node.nodeType === 1) { switch (node.tagName) { case 'INPUT': { clone.value = node.value; clone.checked = node.checked; break; } case 'OPTION': { clone.selected = node.selected; clone.value = node.value; break; } case 'TEXTAREA': { clone.value = node.value; break; } } } var 
originalAttribs = node.attributes___; if (originalAttribs) {
        var attribs = {}; clone.attributes___ = attribs; cajita.forOwnKeys(originalAttribs, ___.func(function(k, v) {
            switch (typeof 
v) { case 'string': ; case 'number': ; case 'boolean': { attribs[k] = v; break; } } 
        }));
    } 
} function 
createStylesheet(document, cssText) { var styleSheet = document.createElement('style'); styleSheet.setAttribute('type', 'text/css'); if (styleSheet.styleSheet) { styleSheet.styleSheet.cssText = cssText; } else { styleSheet.appendChild(document.createTextNode(cssText)); } return styleSheet; } function 
setAttribute(node, name, value) {
    if (name === 'style' && typeof node.style.cssText === 'string') { node.style.cssText = value; } else
        if (name === 'class') { node.className = value; } else if (features.setAttributeExtraParam) { node.setAttribute(name, value, 0); } else { node.setAttribute(name, value); } return value;
} return { 'addEventListener': addEventListener, 'removeEventListener': removeEventListener, 'cloneNode': cloneNode, 'createStylesheet': createStylesheet, 'setAttribute': setAttribute };
})();
    } 
    {
        var 
domitaModules; if (!domitaModules) { domitaModules = {}; } domitaModules.classUtils = function() {
    function 
exportFields(object, fields) {
        for (var i = fields.length; --i >= 0; ) {
            var field = fields[i]; var 
fieldUCamel = field.charAt(0).toUpperCase() + field.substring(1); var getterName = 'get' + fieldUCamel; var 
setterName = 'set' + fieldUCamel; var count = 0; if (object[getterName]) { ++count; ___.useGetHandler(object, field, object[getterName]); } if (object[setterName]) { ++count; ___.useSetHandler(object, field, object[setterName]); } if (!count) {
                throw new 
Error('Failed to export field ' + field + ' on ' + object);
            } 
        } 
    } function extend(subClass, baseClass) {
        var 
noop = function() { }; noop.prototype = baseClass.prototype; subClass.prototype = new noop(); subClass.prototype.constructor = subClass;
    } return { 'exportFields': exportFields, 'extend': extend };
}; domitaModules.XMLHttpRequestCtor = function(XMLHttpRequest, ActiveXObject) {
    if (XMLHttpRequest) { return XMLHttpRequest; } else
        if (ActiveXObject) {
        var activeXClassId; return function ActiveXObjectForIE() {
            if (activeXClassId === void 
0) {
                activeXClassId = null; var activeXClassIds = ['MSXML2.XMLHTTP.5.0', 'MSXML2.XMLHTTP.4.0', 'MSXML2.XMLHTTP.3.0', 'MSXML2.XMLHTTP', 'MICROSOFT.XMLHTTP.1.0', 'MICROSOFT.XMLHTTP.1', 'MICROSOFT.XMLHTTP']; for (var 
i = 0, n = activeXClassIds.length; i < n; i++) {
                    var candidate = activeXClassIds[i]; try {
                        void new 
ActiveXObject(candidate); activeXClassId = candidate; break;
                    } catch (e) { } 
                } activeXClassIds = null;
            } return new 
ActiveXObject(activeXClassId);
        };
    } else { throw new Error('ActiveXObject not available'); } 
}; domitaModules.TameXMLHttpRequest = function(xmlHttpRequestMaker, uriCallback) {
    var 
classUtils = domitaModules.classUtils(); function TameXMLHttpRequest() {
    this.xhr___ = new 
xmlHttpRequestMaker(); classUtils.exportFields(this, ['onreadystatechange', 'readyState', 'responseText', 'responseXML', 'status', 'statusText']);
} TameXMLHttpRequest.prototype.setOnreadystatechange = function(handler) {
    var 
self = this; this.xhr___.onreadystatechange = function(event) {
    var evt = { 'target': self }; return ___.callPub(handler, 'call', [void 
0, evt]);
}; this.handler___ = handler;
}; TameXMLHttpRequest.prototype.getReadyState = function() { return Number(this.xhr___.readyState); }; TameXMLHttpRequest.prototype.open = function(method, URL, opt_async, opt_userName, opt_password) {
    method = String(method); var 
safeUri = uriCallback.rewrite(String(URL), '*/*'); if (safeUri === void 0) { throw 'URI violates security policy'; } switch (arguments.length) {
        case 
2: { this.async___ = true; this.xhr___.open(method, safeUri); break; } case 3: { this.async___ = opt_async; this.xhr___.open(method, safeUri, Boolean(opt_async)); break; } case 
4: { this.async___ = opt_async; this.xhr___.open(method, safeUri, Boolean(opt_async), String(opt_userName)); break; } case 
5: { this.async___ = opt_async; this.xhr___.open(method, safeUri, Boolean(opt_async), String(opt_userName), String(opt_password)); break; } default: { throw 'XMLHttpRequest cannot accept ' + arguments.length + ' arguments'; break; } 
    } 
}; TameXMLHttpRequest.prototype.setRequestHeader = function(label, value) { this.xhr___.setRequestHeader(String(label), String(value)); }; TameXMLHttpRequest.prototype.send = function(opt_data) {
    if (arguments.length === 0) { this.xhr___.send(''); } else
        if (typeof opt_data === 'string') { this.xhr___.send(opt_data); } else { this.xhr___.send(''); } if (this.xhr___.overrideMimeType) {
        if (!this.async___ && this.handler___) {
            var 
evt = { 'target': this }; ___.callPub(this.handler___, 'call', [void 0, evt]);
        } 
    } 
}; TameXMLHttpRequest.prototype.abort = function() { this.xhr___.abort(); }; TameXMLHttpRequest.prototype.getAllResponseHeaders = function() {
    var 
result = this.xhr___.getAllResponseHeaders(); return result === undefined || result === null ? result : String(result);
}; TameXMLHttpRequest.prototype.getResponseHeader = function(headerName) {
    var 
result = this.xhr___.getResponseHeader(String(headerName)); return result === undefined || result === null ? result : String(result);
}; TameXMLHttpRequest.prototype.getResponseText = function() {
    var 
result = this.xhr___.responseText; return result === undefined || result === null ? result : String(result);
}; TameXMLHttpRequest.prototype.getResponseXML = function() { return {}; }; TameXMLHttpRequest.prototype.getStatus = function() {
    var 
result = this.xhr___.status; return result === undefined || result === null ? result : Number(result);
}; TameXMLHttpRequest.prototype.getStatusText = function() {
    var 
result = this.xhr___.statusText; return result === undefined || result === null ? result : String(result);
}; TameXMLHttpRequest.prototype.toString = function() { return 'Not a real XMLHttpRequest'; }; ___.ctor(TameXMLHttpRequest, void 
0, 'TameXMLHttpRequest'); ___.all2(___.grantTypedGeneric, TameXMLHttpRequest.prototype, ['open', 'setRequestHeader', 'send', 'abort', 'getAllResponseHeaders', 'getResponseHeader']); return TameXMLHttpRequest;
}; attachDocumentStub = (function() {
    function 
arrayRemove(array, from, to) { var rest = array.slice((to || from) + 1 || array.length); array.length = from < 0 ? array.length + from : from; return array.push.apply(array, rest); } var 
tameNodeTrademark = {}; var tameEventTrademark = {}; function Html(htmlFragment) { this.html___ = String(htmlFragment || ''); } Html.prototype.valueOf = Html.prototype.toString = function() { return this.html___; }; function 
safeHtml(htmlFragment) { return htmlFragment instanceof Html ? htmlFragment.html___ : html.escapeAttrib(String(htmlFragment || '')); } function 
blessHtml(htmlFragment) { return htmlFragment instanceof Html ? htmlFragment : new Html(htmlFragment); } var 
XML_SPACE = '	\n\r '; var XML_NAME_PATTERN = new RegExp('^[' + unicode.LETTER + '_:][' + unicode.LETTER + unicode.DIGIT + '.\\-_:' + unicode.COMBINING_CHAR + unicode.EXTENDER + ']*$'); var 
XML_NMTOKEN_PATTERN = new RegExp('^[' + unicode.LETTER + unicode.DIGIT + '.\\-_:' + unicode.COMBINING_CHAR + unicode.EXTENDER + ']+$'); var 
XML_NMTOKENS_PATTERN = new RegExp('^(?:[' + XML_SPACE + ']*[' + unicode.LETTER + unicode.DIGIT + '.\\-_:' + unicode.COMBINING_CHAR + unicode.EXTENDER + ']+)+[' + XML_SPACE + ']*$'); var 
JS_SPACE = '	\n\r '; var JS_IDENT = '(?:[a-zA-Z_][a-zA-Z0-9$_]*[a-zA-Z0-9$]|[a-zA-Z])_?'; var 
SIMPLE_HANDLER_PATTERN = new RegExp('^[' + JS_SPACE + ']*' + '(return[' + JS_SPACE + ']+)?' + '(' + JS_IDENT + ')[' + JS_SPACE + ']*' + '\\((?:this' + '(?:[' + JS_SPACE + ']*,[' + JS_SPACE + ']*event)?' + '[' + JS_SPACE + ']*)?\\)' + '[' + JS_SPACE + ']*(?:;?[' + JS_SPACE + ']*)$'); function 
isXmlName(s) { return XML_NAME_PATTERN.test(s); } function isXmlNmTokens(s) { return XML_NMTOKENS_PATTERN.test(s); } function 
trimCssSpaces(input) { return input.replace(/^[ \t\r\n\f]+|[ \t\r\n\f]+$/g, ''); } function 
sanitizeStyleAttrValue(styleAttrValue) {
    var sanitizedDeclarations = []; var declarations = styleAttrValue.split(/;/g); for (var 
i = 0; declarations && i < declarations.length; i++) {
        var parts = declarations[i].split(':'); var 
property = trimCssSpaces(parts[0]).toLowerCase(); var value = trimCssSpaces(parts.slice(1).join(':')); var 
stylePropertyName = property.replace(/-[a-z]/g, function(m) { return m.substring(1).toUpperCase(); }); if (css.properties.hasOwnProperty(stylePropertyName) && css.properties[stylePropertyName].test(value + ' ')) { sanitizedDeclarations.push(property + ': ' + value); } 
    } return sanitizedDeclarations.join(' ; ');
} function 
mimeTypeForAttr(tagName, attribName) { if (tagName === 'img' && attribName === 'src') { return 'image/*'; } return '*/*'; } function
assert(cond) {
    if (!cond) {
        if (typeof console !== 'undefined') { console.error('domita assertion failed'); console.trace(); } throw new 
Error();
    } 
} var classUtils = domitaModules.classUtils(); var cssSealerUnsealerPair = cajita.makeSealerUnsealerPair(); var 
timeoutIdTrademark = {}; function tameSetTimeout(timeout, delayMillis) { var timeoutId = setTimeout(function() { ___.callPub(timeout, 'call', [___.USELESS]); }, delayMillis | 0); return ___.freeze(___.stamp(timeoutIdTrademark, { 'timeoutId___': timeoutId })); } ___.frozenFunc(tameSetTimeout); function 
tameClearTimeout(timeoutId) { ___.guard(timeoutIdTrademark, timeoutId); clearTimeout(timeoutId.timeoutId___); } ___.frozenFunc(tameClearTimeout); var 
intervalIdTrademark = {}; function tameSetInterval(interval, delayMillis) { var intervalId = setInterval(function() { ___.callPub(interval, 'call', [___.USELESS]); }, delayMillis | 0); return ___.freeze(___.stamp(intervalIdTrademark, { 'intervalId___': intervalId })); } ___.frozenFunc(tameSetInterval); function 
tameClearInterval(intervalId) { ___.guard(intervalIdTrademark, intervalId); clearInterval(intervalId.intervalId___); } ___.frozenFunc(tameClearInterval); function 
attachDocumentStub(idSuffix, uriCallback, imports, pseudoBodyNode, optPseudoWindowLocation) {
    if (arguments.length < 4) {
        throw new 
Error('arity mismatch: ' + arguments.length);
    } if (!optPseudoWindowLocation) { optPseudoWindowLocation = {}; } var 
elementPolicies = {}; elementPolicies.form = function(attribs) {
    var sawHandler = false; for (var 
i = 0, n = attribs.length; i < n; i += 2) { if (attribs[i] === 'onsubmit') { sawHandler = true; } } if (!sawHandler) { attribs.push('onsubmit', 'return false'); } return attribs;
}; elementPolicies.a = elementPolicies.area = function(attribs) { attribs.push('target', '_blank'); return attribs; }; function 
sanitizeHtml(htmlText) { var out = []; htmlSanitizer(htmlText, out); return out.join(''); } var 
htmlSanitizer = html.makeHtmlSanitizer(function sanitizeAttributes(tagName, attribs) {
    for (var 
i = 0; i < attribs.length; i += 2) {
        var attribName = attribs[i]; var value = attribs[i + 1]; var atype = null, attribKey; if ((attribKey = tagName + ':' + attribName, html4
.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey))) {
            atype = html4
.ATTRIBS[attribKey]; value = rewriteAttribute(tagName, attribName, atype, value);
        } else { value = null; } if (value !== null && value !== void 
0) { attribs[i + 1] = value; } else { attribs.splice(i, 2); i -= 2; } 
    } var policy = elementPolicies[tagName]; if (policy && elementPolicies.hasOwnProperty(tagName)) { return policy(attribs); } return attribs;
}); function 
tameInnerHtml(htmlText) { var out = []; innerHtmlTamer(htmlText, out); return out.join(''); } var 
innerHtmlTamer = html.makeSaxParser({ 'startTag': function(tagName, attribs, out) {
    out.push('<', tagName); for (var 
i = 0; i < attribs.length; i += 2) {
        var attribName = attribs[i]; if (attribName === 'target') { continue; } var 
attribKey; var atype; if ((attribKey = tagName + ':' + attribName, html4.ATTRIBS.hasOwnProperty(attribKey)) || (attribKey = '*:' + attribName, html4
.ATTRIBS.hasOwnProperty(attribKey))) { atype = html4.ATTRIBS[attribKey]; } else { return ''; } var 
value = attribs[i + 1]; switch (atype) {
            case html4.atype.ID: ; case html4.atype.IDREF: ; case 
html4.atype.IDREFS: { if (value.length <= idSuffix.length || idSuffix !== value.substring(value.length - idSuffix.length)) { continue; } value = value.substring(0, value.length - idSuffix.length); break; } 
        } if (value !== null) { out.push(' ', attribName, '=\"', html.escapeAttrib(value), '\"'); } 
    } out.push('>');
}, 'endTag': function(name, out) { out.push('</', name, '>'); }, 'pcdata': function(text, out) { out.push(text); }, 'rcdata': function(text, out) { out.push(text); }, 'cdata': function(text, out) { out.push(text); } 
}); var 
illegalSuffix = /__(?:\s|$)/; function rewriteAttribute(tagName, attribName, type, value) {
    switch (type) {
        case 
html4.atype.ID: ; case html4.atype.IDREF: ; case html4.atype.IDREFS: { value = String(value); if (value && !illegalSuffix.test(value) && isXmlName(value)) { return value + idSuffix; } return null; } case 
html4.atype.CLASSES: ; case html4.atype.GLOBAL_NAME: ; case html4.atype.LOCAL_NAME: { value = String(value); if (value && !illegalSuffix.test(value) && isXmlNmTokens(value)) { return value; } return null; } case 
html4.atype.SCRIPT: 
            {
                value = String(value); var match = value.match(SIMPLE_HANDLER_PATTERN); if (!match) { return null; } var 
doesReturn = match[1]; var fnName = match[2]; var pluginId = ___.getId(imports); value = (doesReturn ? 'return ' : '') + 'plugin_dispatchEvent___(' + 'this, event, ' + pluginId + ', \"' + fnName + '\");'; if (attribName === 'onsubmit') { value = 'try { ' + value + ' } finally { return false; }'; } return value;
            } case 
html4.atype.URI: { value = String(value); if (!uriCallback) { return null; } return uriCallback.rewrite(value, mimeTypeForAttr(tagName, attribName)) || null; } case 
html4.atype.STYLE: 
            {
                if ('function' !== typeof value) { return sanitizeStyleAttrValue(String(value)); } var 
cssPropertiesAndValues = cssSealerUnsealerPair.unseal(value); if (!cssPropertiesAndValues) { return null; } var 
css = []; for (var i = 0; i < cssPropertiesAndValues.length; i += 2) {
                    var propName = cssPropertiesAndValues[i]; var 
propValue = cssPropertiesAndValues[i + 1]; var semi = propName.indexOf(';'); if (semi >= 0) { propName = propName.substring(0, semi); } css.push(propName + ' : ' + propValue);
                } return css.join(' ; ');
            } case 
html4.atype.FRAME_TARGET: return null; default: return String(value);
    } 
} function makeCache() {
    var 
cache = cajita.newTable(false); cache.set(null, null); cache.set(void 0, null); return cache;
} var 
editableTameNodeCache = makeCache(); var readOnlyTameNodeCache = makeCache(); function 
tameNode(node, editable) {
    if (node === null || node === void 0) { return null; } var cache = editable ? editableTameNodeCache : readOnlyTameNodeCache; var 
tamed = cache.get(node); if (tamed !== void 0) { return tamed; } switch (node.nodeType) {
        case 
1: 
            {
                var tagName = node.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName) || html4
.ELEMENTS[tagName] & html4.eflags.UNSAFE) { tamed = new TameOpaqueNode(node, editable); break; } switch (tagName) {
                    case 'a': 
                        {
                            tamed = new 
TameAElement(node, editable); break;
                        } case 'form': { tamed = new TameFormElement(node, editable); break; } case 'select': ; case 'button': ; case 'option': ; case 'textarea': ; case 'input': 
                        {
                            tamed = new 
TameInputElement(node, editable); break;
                        } case 'img': { tamed = new TameImageElement(node, editable); break; } case 'td': ; case 'tr': ; case 'thead': ; case 'tfoot': ; case 'tbody': ; case 'th': 
                        {
                            tamed = new 
TameTableCompElement(node, editable); break;
                        } case 'table': { tamed = new TameTableElement(node, editable); break; } default: 
                        {
                            tamed = new 
TameElement(node, editable); break;
                        } 
                } break;
            } case 2: { tamed = new TameAttrNode(node, editable); break; } case 
3: { tamed = new TameTextNode(node, editable); break; } case 8: { tamed = new TameCommentNode(node, editable); break; } default: 
            {
                tamed = new 
TameOpaqueNode(node, editable); break;
            } 
    } if (node.nodeType === 1) { cache.set(node, tamed); } return tamed;
} function 
tameRelatedNode(node, editable) {
    if (node === null || node === void 0) { return null; } try {
        for (var 
ancestor = node; ancestor; ancestor = ancestor.parentNode) { if (idClass === ancestor.className) { return tameNode(node, editable); } } 
    } catch (e) { } return null;
} function 
tameNodeList(nodeList, editable, opt_keyAttrib) {
    var tamed = []; var node, untamed; for (var 
i = 0, k = -1, n = nodeList.length; i < n; ++i) {
        untamed = nodeList.item(i); if (untamed.nodeName && endsWith__.test(untamed.nodeName)) { continue; } node = tameNode(untamed, editable); tamed[++k] = node; var 
key = opt_keyAttrib && node.getAttribute(opt_keyAttrib); if (key && !(key.charAt(key.length - 1) === '_' || key
in tamed || key === String(key & 2147483647))) { tamed[key] = node; } 
    } node = nodeList = untamed = null; tamed.item = ___.frozenFunc(function(k) {
        k &= 2147483647; if (isNaN(k)) {
            throw new 
Error();
        } return tamed[k] || null;
    }); return cajita.freeze(tamed);
} function tameGetElementsByTagName(rootNode, tagName, editable) {
    tagName = String(tagName); if (tagName !== '*') {
        tagName = tagName.toLowerCase(); if (!___.hasOwnProp(html4
.ELEMENTS, tagName) || html4.ELEMENTS[tagName] & html4.ELEMENTS.UNSAFE) { return new fakeNodeList([]); } 
    } return tameNodeList(rootNode.getElementsByTagName(tagName), editable);
} function 
tameGetElementsByClassName(rootNode, className, editable) {
    className = String(className); var 
classes = className.match(/[^\t\n\f\r ]+/g); for (var i = classes ? classes.length : 0; --i >= 0; ) {
        var 
classi = classes[i]; if (illegalSuffix.test(classi) || !isXmlNmTokens(classi)) { classes[i] = classes[nClasses - 1]; --classes.length; } 
    } if (!classes || classes.length === 0) { return fakeNodeList([]); } if (typeof 
rootNode.getElementsByClassName === 'function') { return tameNodeList(rootNode.getElementsByClassName(classes.join(' ')), editable); } else {
        var 
nClasses = classes.length; for (var i = nClasses; --i >= 0; ) { classes[i] = ' ' + classes[i] + ' '; } var 
candidates = rootNode.getElementsByTagName('*'); var matches = []; candidate_loop: for (var 
j = 0, n = candidates.length, k = -1; j < n; ++j) {
            var candidate = candidates[j]; var candidateClass = ' ' + candidate.className + ' '; for (var 
i = nClasses; --i >= 0; ) { if (-1 === candidateClass.indexOf(classes[i])) { continue candidate_loop; } } var 
tamed = tameNode(candidate, editable); if (tamed) { matches[++k] = tamed; } 
        } return fakeNodeList(matches);
    } 
} function 
makeEventHandlerWrapper(thisNode, listener) {
    if ('function' !== typeof listener && !('object' === typeof 
listener && listener !== null && ___.canCallPub(listener, 'call'))) {
        throw new Error('Expected function not ' + typeof 
listener);
    } function wrapper(event) { return plugin_dispatchEvent___(thisNode, event, ___.getId(imports), listener); } wrapper.originalListener___ = listener; return wrapper;
} var 
NOT_EDITABLE = 'Node not editable.'; var INVALID_SUFFIX = 'Property names may not end in \'__\'.'; var 
UNSAFE_TAGNAME = 'Unsafe tag name.'; var UNKNOWN_TAGNAME = 'Unknown tag name.'; var CUSTOM_EVENT_TYPE_SUFFIX = ':custom'; function 
tameEventName(name, opt_isCustom) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error('Invalid event name ' + name);
    } if (opt_isCustom || html4.atype.SCRIPT !== html4.ATTRIBS['*:on' + name]) { name = name + CUSTOM_EVENT_TYPE_SUFFIX; } return name;
} function 
tameAddEventListener(name, listener, useCapture) {
    if (!this.editable___) { throw new Error(NOT_EDITABLE); } if (!this.wrappedListeners___) { this.wrappedListeners___ = []; } name = tameEventName(name); useCapture = Boolean(useCapture); var 
wrappedListener = makeEventHandlerWrapper(this.node___, listener); this.wrappedListeners___.push(wrappedListener); bridal.addEventListener(this.node___, name, wrappedListener, useCapture);
} function 
tameRemoveEventListener(name, listener, useCapture) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } name = tameEventName(name); if (!this.wrappedListeners___) { return; } var 
wrappedListener; for (var i = this.wrappedListeners___.length; --i >= 0; ) { if (this.wrappedListeners___[i].originalListener___ === listener) { wrappedListener = this.wrappedListeners___[i]; arrayRemove(this.wrappedListeners___, i, i); break; } } if (!wrappedListener) { return; } bridal.removeEventListener(this.node___, name, wrappedListener, useCapture);
} var 
nodeClasses = {}; var tameNodeFields = ['nodeType', 'nodeValue', 'nodeName', 'firstChild', 'lastChild', 'nextSibling', 'previousSibling', 'parentNode', 'ownerDocument', 'childNodes', 'attributes']; function 
TameNode(editable) { this.editable___ = editable; ___.stamp(tameNodeTrademark, this, true); classUtils.exportFields(this, tameNodeFields); } TameNode.prototype.getOwnerDocument = function() {
    if (!this.editable___ && tameDocument.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return tameDocument;
}; nodeClasses.Node = TameNode; ___.ctor(TameNode, void 
0, 'TameNode'); var tameNodeMembers = ['getNodeType', 'getNodeValue', 'getNodeName', 'cloneNode', 'appendChild', 'insertBefore', 'removeChild', 'replaceChild', 'getFirstChild', 'getLastChild', 'getNextSibling', 'getPreviousSibling', 'getElementsByClassName', 'getElementsByTagName', 'getOwnerDocument', 'dispatchEvent', 'hasChildNodes']; function 
TameBackedNode(node, editable) { if (!node) { throw new Error('Creating tame node with undefined native delegate'); } this.node___ = node; TameNode.call(this, editable); } classUtils.extend(TameBackedNode, TameNode); TameBackedNode.prototype.getNodeType = function() { return this.node___.nodeType; }; TameBackedNode.prototype.getNodeName = function() { return this.node___.nodeName; }; TameBackedNode.prototype.getNodeValue = function() { return this.node___.nodeValue; }; TameBackedNode.prototype.cloneNode = function(deep) {
    var 
clone = bridal.cloneNode(this.node___, Boolean(deep)); return tameNode(clone, true);
}; TameBackedNode.prototype.appendChild = function(child) {
    cajita.guard(tameNodeTrademark, child); if (!this.editable___ || !child.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.appendChild(child.node___);
}; TameBackedNode.prototype.insertBefore = function(toInsert, child) {
    cajita.guard(tameNodeTrademark, toInsert); if (child === void 
0) { child = null; } if (child !== null) { cajita.guard(tameNodeTrademark, child); } if (!this.editable___ || !toInsert.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.insertBefore(toInsert.node___, child !== null ? child.node___ : null);
}; TameBackedNode.prototype.removeChild = function(child) {
    cajita.guard(tameNodeTrademark, child); if (!this.editable___ || !child.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.removeChild(child.node___);
}; TameBackedNode.prototype.replaceChild = function(child, replacement) {
    cajita.guard(tameNodeTrademark, child); cajita.guard(tameNodeTrademark, replacement); if (!this.editable___ || !replacement.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.replaceChild(child.node___, replacement.node___);
}; TameBackedNode.prototype.getFirstChild = function() { return tameNode(this.node___.firstChild, this.editable___); }; TameBackedNode.prototype.getLastChild = function() { return tameNode(this.node___.lastChild, this.editable___); }; TameBackedNode.prototype.getNextSibling = function() { return tameNode(this.node___.nextSibling, this.editable___); }; TameBackedNode.prototype.getPreviousSibling = function() { return tameNode(this.node___.previousSibling, this.editable___); }; TameBackedNode.prototype.getParentNode = function() {
    var 
parent = this.node___.parentNode; if (parent === tameDocument.body___) {
        if (tameDocument.editable___ && !this.editable___) {
            throw new 
Error(NOT_EDITABLE);
        } return tameDocument.getBody();
    } return tameRelatedNode(this.node___.parentNode, this.editable___);
}; TameBackedNode.prototype.getElementsByTagName = function(tagName) { return tameGetElementsByTagName(this.node___, tagName, this.editable___); }; TameBackedNode.prototype.getElementsByClassName = function(className) { return tameGetElementsByClassName(this.node___, className, this.editable___); }; TameBackedNode.prototype.getChildNodes = function() { return tameNodeList(this.node___.childNodes, this.editable___); }; TameBackedNode.prototype.getAttributes = function() { return tameNodeList(this.node___.attributes, this.editable___); }; var 
endsWith__ = /__$/; TameBackedNode.prototype.handleRead___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        return void 
0;
    } var handlerName = name + '_getter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (___.hasOwnProp(this.node___.properties___, name)) { return this.node___.properties___[name]; } else {
        return void 
0;
    } 
}; TameBackedNode.prototype.handleCall___ = function(name, args) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } var handlerName = name + '_handler___'; if (this[handlerName]) { return this[handlerName].call(this, args); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName].call(this, args); } if (___.hasOwnProp(this.node___.properties___, name)) { return this.node___.properties___[name].call(this, args); } else {
        throw new 
TypeError(name + ' is not a function.');
    } 
}; TameBackedNode.prototype.handleSet___ = function(name, val) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } if (!this.editable___) { throw new Error(NOT_EDITABLE); } var handlerName = name + '_setter___'; if (this[handlerName]) { return this[handlerName](val); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](val); } if (!this.node___.properties___) { this.node___.properties___ = {}; } this[name + '_canEnum___'] = true; return this.node___.properties___[name] = val;
}; TameBackedNode.prototype.handleDelete___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } if (!this.editable___) { throw new Error(NOT_EDITABLE); } var handlerName = name + '_deleter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (this.node___.properties___) {
        return delete 
this.node___.properties___[name] && delete this[name + '_canEnum___'];
    } else { return true; } 
}; TameBackedNode.prototype.handleEnum___ = function(ownFlag) { if (this.node___.properties___) { return cajita.allKeys(this.node___.properties___); } return []; }; TameBackedNode.prototype.hasChildNodes = function() { return !(!this.node___.hasChildNodes()); }; TameBackedNode.prototype.dispatchEvent = function 
dispatchEvent(evt) { cajita.guard(tameEventTrademark, evt); return Boolean(this.node___.dispatchEvent(evt.event___)); }; ___.ctor(TameBackedNode, TameNode, 'TameBackedNode'); ___.all2(___.grantTypedGeneric, TameBackedNode.prototype, tameNodeMembers); function 
TamePseudoNode(editable) { TameNode.call(this, editable); this.properties___ = {}; } classUtils.extend(TamePseudoNode, TameNode); TamePseudoNode.prototype.appendChild = TamePseudoNode.prototype.insertBefore = TamePseudoNode.prototype.removeChild = TamePseudoNode.prototype.replaceChild = function(child) {
    throw new 
Error(NOT_EDITABLE);
}; TamePseudoNode.prototype.getFirstChild = function() { var children = this.getChildNodes(); return children.length ? children[0] : null; }; TamePseudoNode.prototype.getLastChild = function() {
    var 
children = this.getChildNodes(); return children.length ? children[children.length - 1] : null;
}; TamePseudoNode.prototype.getNextSibling = function() {
    var 
parentNode = this.getParentNode(); if (!parentNode) { return null; } var siblings = parentNode.getChildNodes(); for (var 
i = siblings.length - 1; --i >= 0; ) { if (siblings[i] === this) { return siblings[i + 1]; } } return null;
}; TamePseudoNode.prototype.getPreviousSibling = function() {
    var 
parentNode = this.getParentNode(); if (!parentNode) { return null; } var siblings = parentNode.getChildNodes(); for (var 
i = siblings.length; --i >= 1; ) { if (siblings[i] === this) { return siblings[i - 1]; } } return null;
}; TamePseudoNode.prototype.handleRead___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        return void 
0;
    } var handlerName = name + '_getter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (___.hasOwnProp(this.properties___, name)) { return this.properties___[name]; } else {
        return void 
0;
    } 
}; TamePseudoNode.prototype.handleCall___ = function(name, args) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } var handlerName = name + '_handler___'; if (this[handlerName]) { return this[handlerName].call(this, args); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName].call(this, args); } if (___.hasOwnProp(this.properties___, name)) { return this.properties___[name].call(this, args); } else {
        throw new 
TypeError(name + ' is not a function.');
    } 
}; TamePseudoNode.prototype.handleSet___ = function(name, val) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } if (!this.editable___) { throw new Error(NOT_EDITABLE); } var handlerName = name + '_setter___'; if (this[handlerName]) { return this[handlerName](val); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](val); } if (!this.properties___) { this.properties___ = {}; } this[name + '_canEnum___'] = true; return this.properties___[name] = val;
}; TamePseudoNode.prototype.handleDelete___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } if (!this.editable___) { throw new Error(NOT_EDITABLE); } var handlerName = name + '_deleter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (this.properties___) {
        return delete 
this.properties___[name] && delete this[name + '_canEnum___'];
    } else { return true; } 
}; TamePseudoNode.prototype.handleEnum___ = function(ownFlag) { if (this.properties___) { return cajita.allKeys(this.properties___); } return []; }; TamePseudoNode.prototype.hasChildNodes = function() { return this.getFirstChild() != null; }; ___.ctor(TamePseudoNode, TameNode, 'TamePseudoNode'); ___.all2(___.grantTypedGeneric, TamePseudoNode.prototype, tameNodeMembers); function 
TamePseudoElement(tagName, tameDoc, childNodesGetter, parentNodeGetter, innerHTMLGetter, editable) { TamePseudoNode.call(this, editable); this.tagName___ = tagName; this.tameDoc___ = tameDoc; this.childNodesGetter___ = childNodesGetter; this.parentNodeGetter___ = parentNodeGetter; this.innerHTMLGetter___ = innerHTMLGetter; classUtils.exportFields(this, ['tagName', 'innerHTML']); } classUtils.extend(TamePseudoElement, TamePseudoNode); TamePseudoElement.prototype.getNodeType = function() { return 1; }; TamePseudoElement.prototype.getNodeName = function() { return this.tagName___; }; TamePseudoElement.prototype.getTagName = function() { return this.tagName___; }; TamePseudoElement.prototype.getNodeValue = function() { return null; }; TamePseudoElement.prototype.getAttribute = function(attribName) { return ''; }; TamePseudoElement.prototype.hasAttribute = function(attribName) { return false; }; TamePseudoElement.prototype.getOwnerDocument = function() { return this.tameDoc___; }; TamePseudoElement.prototype.getChildNodes = function() { return this.childNodesGetter___(); }; TamePseudoElement.prototype.getAttributes = function() { return tameNodeList([], false); }; TamePseudoElement.prototype.getParentNode = function() { return this.parentNodeGetter___(); }; TamePseudoElement.prototype.getInnerHTML = function() { return this.innerHTMLGetter___(); }; TamePseudoElement.prototype.getElementsByTagName = function(tagName) { tagName = String(tagName).toLowerCase(); if (tagName === this.tagName___) { return fakeNodeList([]); } return this.getOwnerDocument().getElementsByTagName(tagName); }; TamePseudoElement.prototype.getElementsByClassName = function(className) { return this.getOwnerDocument().getElementsByClassName(className); }; TamePseudoElement.prototype.toString = function() { return '<' + this.tagName___ + '>'; }; ___.ctor(TamePseudoElement, TamePseudoNode, 'TamePseudoElement'); ___.all2(___.grantTypedGeneric, TamePseudoElement.prototype, ['getTagName', 'getAttribute', 'hasAttribute']); function 
TameOpaqueNode(node, editable) { TameBackedNode.call(this, node, editable); } classUtils.extend(TameOpaqueNode, TameBackedNode); TameOpaqueNode.prototype.getNodeValue = TameBackedNode.prototype.getNodeValue; TameOpaqueNode.prototype.getNodeType = TameBackedNode.prototype.getNodeType; TameOpaqueNode.prototype.getNodeName = TameBackedNode.prototype.getNodeName; TameOpaqueNode.prototype.getNextSibling = TameBackedNode.prototype.getNextSibling; TameOpaqueNode.prototype.getPreviousSibling = TameBackedNode.prototype.getPreviousSibling; TameOpaqueNode.prototype.getFirstChild = TameBackedNode.prototype.getFirstChild; TameOpaqueNode.prototype.getLastChild = TameBackedNode.prototype.getLastChild; TameOpaqueNode.prototype.getParentNode = TameBackedNode.prototype.getParentNode; TameOpaqueNode.prototype.getChildNodes = TameBackedNode.prototype.getChildNodes; TameOpaqueNode.prototype.getAttributes = function() { return tameNodeList([], false); }; for (var 
i = tameNodeMembers.length; --i >= 0; ) {
        var k = tameNodeMembers[i]; if (!TameOpaqueNode.prototype.hasOwnProperty(k)) {
            TameOpaqueNode.prototype[k] = ___.frozenFunc(function() {
                throw new 
Error('Node is opaque');
            });
        } 
    } ___.all2(___.grantTypedGeneric, TameOpaqueNode.prototype, tameNodeMembers); function 
TameAttrNode(node, editable) { assert(node.nodeType === 2); TameBackedNode.call(this, node, editable); classUtils.exportFields(this, ['name', 'nodeValue', 'value', 'specified']); } classUtils.extend(TameAttrNode, TameBackedNode); nodeClasses.Attr = TameAttrNode; TameAttrNode.prototype.setNodeValue = function(value) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.nodeValue = String(value || ''); return value;
}; TameAttrNode.prototype.getName = TameAttrNode.prototype.getNodeName; TameAttrNode.prototype.getValue = TameAttrNode.prototype.getNodeValue; TameAttrNode.prototype.setValue = TameAttrNode.prototype.setNodeValue; TameAttrNode.prototype.getSpecified = function() { return this.node___.specified; }; TameAttrNode.prototype.toString = function() { return '#attr'; }; ___.ctor(TameAttrNode, TameBackedNode, 'TameAttrNode'); function 
TameTextNode(node, editable) { assert(node.nodeType === 3); TameBackedNode.call(this, node, editable); classUtils.exportFields(this, ['nodeValue', 'data']); } classUtils.extend(TameTextNode, TameBackedNode); nodeClasses.Text = TameTextNode; TameTextNode.prototype.setNodeValue = function(value) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.nodeValue = String(value || ''); return value;
}; TameTextNode.prototype.getData = TameTextNode.prototype.getNodeValue; TameTextNode.prototype.setData = TameTextNode.prototype.setNodeValue; TameTextNode.prototype.toString = function() { return '#text'; }; ___.ctor(TameTextNode, TameBackedNode, 'TameTextNode'); ___.all2(___.grantTypedGeneric, TameTextNode.prototype, ['setNodeValue', 'getData', 'setData']); function 
TameCommentNode(node, editable) { assert(node.nodeType === 8); TameBackedNode.call(this, node, editable); } classUtils.extend(TameCommentNode, TameBackedNode); nodeClasses.CommentNode = TameCommentNode; TameCommentNode.prototype.toString = function() { return '#comment'; }; ___.ctor(TameCommentNode, TameBackedNode, 'TameCommentNode'); function 
getAttributeType(tagName, attribName) {
    var attribKey; attribKey = tagName + ':' + attribName; if (html4
.ATTRIBS.hasOwnProperty(attribKey)) { return html4.ATTRIBS[attribKey]; } attribKey = '*:' + attribName; if (html4
.ATTRIBS.hasOwnProperty(attribKey)) { return html4.ATTRIBS[attribKey]; } return void 
0;
} function TameElement(node, editable) { assert(node.nodeType === 1); TameBackedNode.call(this, node, editable); classUtils.exportFields(this, ['className', 'id', 'innerHTML', 'tagName', 'style', 'offsetLeft', 'offsetTop', 'offsetWidth', 'offsetHeight', 'offsetParent', 'scrollLeft', 'scrollTop', 'scrollWidth', 'scrollHeight', 'title', 'dir']); } classUtils.extend(TameElement, TameBackedNode); nodeClasses.Element = nodeClasses.HTMLElement = TameElement; TameElement.prototype.getId = function() { return this.getAttribute('id') || ''; }; TameElement.prototype.setId = function(newId) { return this.setAttribute('id', newId); }; TameElement.prototype.getAttribute = function(attribName) {
    attribName = String(attribName).toLowerCase(); var 
tagName = this.node___.tagName.toLowerCase(); var atype = getAttributeType(tagName, attribName); if (atype === void 
0) { if (this.node___.attributes___) { return this.node___.attributes___[attribName] || null; } return null; } var 
value = this.node___.getAttribute(attribName); if ('string' !== typeof value) { return value; } switch (atype) {
        case 
html4.atype.ID: ; case html4.atype.IDREF: ; case html4.atype.IDREFS: 
            {
                if (!value) { return null; } var 
n = idSuffix.length; var len = value.length; var end = len - n; if (end > 0 && idSuffix === value.substring(end, len)) { return value.substring(0, end); } return null;
            } default: 
            {
                if ('' === value) {
                    var 
attr = this.node___.getAttributeNode(attribName); if (attr && !attr.specified) { return null; } 
                } return value;
            } 
    } 
}; TameElement.prototype.hasAttribute = function(attribName) {
    attribName = String(attribName).toLowerCase(); var 
tagName = this.node___.tagName.toLowerCase(); var atype = getAttributeType(tagName, attribName); if (atype === void 
0) { return !(!(this.node___.attributes___ && ___.hasOwnProp(this.node___.attributes___, attribName))); } else {
        var 
node = this.node___; if (node.hasAttribute) { return node.hasAttribute(attribName); } else {
            var 
attr = node.getAttributeNode(attribName); return attr !== null && attr.specified;
        } 
    } 
}; TameElement.prototype.setAttribute = function(attribName, value) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } attribName = String(attribName).toLowerCase(); var tagName = this.node___.tagName.toLowerCase(); var 
atype = getAttributeType(tagName, attribName); if (atype === void 0) { if (!this.node___.attributes___) { this.node___.attributes___ = {}; } this.node___.attributes___[attribName] = String(value); } else {
        var 
sanitizedValue = rewriteAttribute(tagName, attribName, atype, value); if (sanitizedValue !== null) { bridal.setAttribute(this.node___, attribName, sanitizedValue); } 
    } return value;
}; TameElement.prototype.removeAttribute = function(attribName) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } attribName = String(attribName).toLowerCase(); var tagName = this.node___.tagName.toLowerCase(); var 
atype = getAttributeType(tagName, attribName); if (atype === void 0) {
        if (this.node___.attributes___) {
            delete 
this.node___.attributes___[attribName];
        } 
    } else { this.node___.removeAttribute(attribName); } 
}; TameElement.prototype.getClassName = function() { return this.getAttribute('class') || ''; }; TameElement.prototype.setClassName = function(classes) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('class', String(classes));
}; TameElement.prototype.getTitle = function() { return this.getAttribute('title') || ''; }; TameElement.prototype.setTitle = function(classes) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('title', String(classes));
}; TameElement.prototype.getDir = function() { return this.getAttribute('dir') || ''; }; TameElement.prototype.setDir = function(classes) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('dir', String(classes));
}; TameElement.prototype.getTagName = TameBackedNode.prototype.getNodeName; TameElement.prototype.getInnerHTML = function() {
    var 
tagName = this.node___.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) { return ''; } var 
flags = html4.ELEMENTS[tagName]; var innerHtml = this.node___.innerHTML; if (flags & html4
.eflags.CDATA) { innerHtml = html.escapeAttrib(innerHtml); } else if (flags & html4.eflags.RCDATA) { innerHtml = html.normalizeRCData(innerHtml); } else { innerHtml = tameInnerHtml(innerHtml); } return innerHtml;
}; TameElement.prototype.setInnerHTML = function(htmlFragment) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } var tagName = this.node___.tagName.toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) {
        throw new 
Error();
    } var flags = html4.ELEMENTS[tagName]; if (flags & html4.eflags.UNSAFE) {
        throw new 
Error();
    } var sanitizedHtml; if (flags & html4.eflags.RCDATA) { sanitizedHtml = html.normalizeRCData(String(htmlFragment || '')); } else {
        sanitizedHtml = htmlFragment
instanceof Html ? safeHtml(htmlFragment) : sanitizeHtml(String(htmlFragment || ''));
    } this.node___.innerHTML = sanitizedHtml; return htmlFragment;
}; TameElement.prototype.setStyle = function(style) { this.setAttribute('style', style); return this.getStyle(); }; TameElement.prototype.getStyle = function() {
    return new 
TameStyle(this.node___.style, this.editable___);
}; TameElement.prototype.updateStyle = function(style) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } var cssPropertiesAndValues = cssSealerUnsealerPair.unseal(style); if (!cssPropertiesAndValues) {
        throw new 
Error();
    } var styleNode = this.node___.style; for (var i = 0; i < cssPropertiesAndValues.length; i += 2) {
        var 
propName = cssPropertiesAndValues[i]; var propValue = cssPropertiesAndValues[i + 1]; var 
semi = propName.indexOf(';'); if (semi >= 0) { propName = propName.substring(semi + 1); } styleNode[propName] = propValue;
    } 
}; TameElement.prototype.getOffsetLeft = function() { return this.node___.offsetLeft; }; TameElement.prototype.getOffsetTop = function() { return this.node___.offsetTop; }; TameElement.prototype.getOffsetWidth = function() { return this.node___.offsetWidth; }; TameElement.prototype.getOffsetHeight = function() { return this.node___.offsetHeight; }; TameElement.prototype.getOffsetParent = function() { return tameRelatedNode(this.node___.offsetParent, this.editable___); }; TameElement.prototype.getScrollLeft = function() { return this.node___.scrollLeft; }; TameElement.prototype.getScrollTop = function() { return this.node___.scrollTop; }; TameElement.prototype.getScrollWidth = function() { return this.node___.scrollWidth; }; TameElement.prototype.getScrollHeight = function() { return this.node___.scrollHeight; }; TameElement.prototype.toString = function() { return '<' + this.node___.tagName + '>'; }; TameElement.prototype.addEventListener = tameAddEventListener; TameElement.prototype.removeEventListener = tameRemoveEventListener; ___.ctor(TameElement, TameBackedNode, 'TameElement'); ___.all2(___.grantTypedGeneric, TameElement.prototype, ['addEventListener', 'removeEventListener', 'getAttribute', 'setAttribute', 'removeAttribute', 'hasAttribute', 'getClassName', 'setClassName', 'getId', 'setId', 'getInnerHTML', 'setInnerHTML', 'updateStyle', 'getStyle', 'setStyle', 'getTagName', 'getOffsetLeft', 'getOffsetTop', 'getOffsetWidth', 'getOffsetHeight']); (function() {
    var 
attrNameRe = /:(.*)/; for (var html4Attrib in html4.ATTRIBS) {
        if (html4.atype.SCRIPT === html4
.ATTRIBS[html4Attrib]) {
            (function(attribName) {
                ___.useSetHandler(TameElement.prototype, attribName, function 
eventHandlerSetter(listener) { if (!this.editable___) { throw new Error(NOT_EDITABLE); } if (!listener) { this.node___[attribName] = null; } else { this.node___[attribName] = makeEventHandlerWrapper(this.node___, listener); return listener; } });
            })((html4Attrib.match(attrNameRe))[1]);
        } 
    } 
})(); function 
TameAElement(node, editable) { TameElement.call(this, node, editable); classUtils.exportFields(this, ['href']); } classUtils.extend(TameAElement, TameElement); nodeClasses.HTMLAnchorElement = TameAElement; TameAElement.prototype.focus = function() { this.node___.focus(); }; TameAElement.prototype.getHref = function() { return this.node___.href; }; TameAElement.prototype.setHref = function(href) { this.setAttribute('href', href); return href; }; ___.ctor(TameAElement, TameElement, 'TameAElement'); ___.all2(___.grantTypedGeneric, TameAElement.prototype, ['getHref', 'setHref', 'focus']); function 
TameFormElement(node, editable) { TameElement.call(this, node, editable); this.length = node.length; classUtils.exportFields(this, ['action', 'elements', 'enctype', 'method', 'target']); } classUtils.extend(TameFormElement, TameElement); nodeClasses.HTMLFormElement = TameFormElement; TameFormElement.prototype.submit = function() { return this.node___.submit(); }; TameFormElement.prototype.reset = function() { return this.node___.reset(); }; TameFormElement.prototype.getAction = function() { return this.getAttribute('action') || ''; }; TameFormElement.prototype.setAction = function(newVal) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('action', String(newVal));
}; TameFormElement.prototype.getElements = function() { return tameNodeList(this.node___.elements, this.editable___, 'name'); }; TameFormElement.prototype.getEnctype = function() { return this.getAttribute('enctype') || ''; }; TameFormElement.prototype.setEnctype = function(newVal) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('enctype', String(newVal));
}; TameFormElement.prototype.getMethod = function() { return this.getAttribute('method') || ''; }; TameFormElement.prototype.setMethod = function(newVal) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('method', String(newVal));
}; TameFormElement.prototype.getTarget = function() { return this.getAttribute('target') || ''; }; TameFormElement.prototype.setTarget = function(newVal) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.setAttribute('target', String(newVal));
}; TameFormElement.prototype.reset = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.reset();
}; TameFormElement.prototype.submit = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.submit();
}; ___.ctor(TameFormElement, TameElement, 'TameFormElement'); ___.all2(___.grantTypedGeneric, TameFormElement.prototype, ['getElements', 'reset', 'submit']); function 
TameInputElement(node, editable) { TameElement.call(this, node, editable); classUtils.exportFields(this, ['form', 'value', 'defaultValue', 'checked', 'disabled', 'readOnly', 'options', 'selected', 'selectedIndex', 'name', 'accessKey', 'tabIndex', 'text', 'defaultChecked', 'defaultSelected', 'maxLength', 'size', 'type', 'index', 'label', 'multiple', 'cols', 'rows']); } classUtils.extend(TameInputElement, TameElement); nodeClasses.HTMLInputElement = TameInputElement; TameInputElement.prototype.getChecked = function() { return this.node___.checked; }; TameInputElement.prototype.setChecked = function(checked) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return this.node___.checked = !(!checked);
}; TameInputElement.prototype.getValue = function() {
    var 
value = this.node___.value; return value === null || value === void 0 ? null : String(value);
}; TameInputElement.prototype.setValue = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.value = newValue === null || newValue === void 0 ? '' : '' + newValue; return newValue;
}; TameInputElement.prototype.getDefaultValue = function() {
    var 
value = this.node___.defaultValue; return value === null || value === void 0 ? null : String(value);
}; TameInputElement.prototype.setDefaultValue = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.defaultValue = newValue === null || newValue === void 0 ? '' : '' + newValue; return newValue;
}; TameInputElement.prototype.focus = function() { this.node___.focus(); }; TameInputElement.prototype.blur = function() { this.node___.blur(); }; TameInputElement.prototype.select = function() { this.node___.select(); }; TameInputElement.prototype.getForm = function() { return tameRelatedNode(this.node___.form, this.editable___); }; TameInputElement.prototype.getDisabled = function() { return this.node___.disabled; }; TameInputElement.prototype.setDisabled = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.disabled = newValue; return newValue;
}; TameInputElement.prototype.getReadOnly = function() { return this.node___.readOnly; }; TameInputElement.prototype.setReadOnly = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.readOnly = newValue; return newValue;
}; TameInputElement.prototype.getOptions = function() { return tameNodeList(this.node___.options, this.editable___, 'name'); }; TameInputElement.prototype.getDefaultSelected = function() { return this.node___.defaultSelected; }; TameInputElement.prototype.setDefaultSelected = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.defaultSelected = !(!newValue); return newValue;
}; TameInputElement.prototype.getSelected = function() { return this.node___.selected; }; TameInputElement.prototype.setSelected = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.selected = newValue; return newValue;
}; TameInputElement.prototype.getSelectedIndex = function() { return this.node___.selectedIndex; }; TameInputElement.prototype.setSelectedIndex = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.selectedIndex = newValue | 0; return newValue;
}; TameInputElement.prototype.getName = function() { return this.node___.name; }; TameInputElement.prototype.setName = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.name = newValue; return newValue;
}; TameInputElement.prototype.getAccessKey = function() { return this.node___.accessKey; }; TameInputElement.prototype.setAccessKey = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.accessKey = newValue; return newValue;
}; TameInputElement.prototype.getTabIndex = function() { return this.node___.tabIndex; }; TameInputElement.prototype.getText = function() { return String(this.node___.text); }; TameInputElement.prototype.setTabIndex = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.tabIndex = newValue; return newValue;
}; TameInputElement.prototype.getDefaultChecked = function() { return this.node___.defaultChecked; }; TameInputElement.prototype.setDefaultChecked = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.defaultChecked = newValue; return newValue;
}; TameInputElement.prototype.getMaxLength = function() { return this.node___.maxLength; }; TameInputElement.prototype.setMaxLength = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.maxLength = newValue; return newValue;
}; TameInputElement.prototype.getSize = function() { return this.node___.size; }; TameInputElement.prototype.setSize = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.size = newValue; return newValue;
}; TameInputElement.prototype.getType = function() { return String(this.node___.type); }; TameInputElement.prototype.setType = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.type = newValue; return newValue;
}; TameInputElement.prototype.getIndex = function() { return this.node___.index; }; TameInputElement.prototype.setIndex = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.index = newValue; return newValue;
}; TameInputElement.prototype.getLabel = function() { return this.node___.label; }; TameInputElement.prototype.setLabel = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.label = newValue; return newValue;
}; TameInputElement.prototype.getMultiple = function() { return this.node___.multiple; }; TameInputElement.prototype.setMultiple = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.multiple = newValue; return newValue;
}; TameInputElement.prototype.getCols = function() { return this.node___.cols; }; TameInputElement.prototype.setCols = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.cols = newValue; return newValue;
}; TameInputElement.prototype.getRows = function() { return this.node___.rows; }; TameInputElement.prototype.setRows = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.rows = newValue; return newValue;
}; ___.ctor(TameInputElement, TameElement, 'TameInputElement'); ___.all2(___.grantTypedGeneric, TameInputElement.prototype, ['getValue', 'setValue', 'focus', 'getForm', 'getType', 'select']); function 
TameImageElement(node, editable) { TameElement.call(this, node, editable); classUtils.exportFields(this, ['src', 'alt']); } classUtils.extend(TameImageElement, TameElement); nodeClasses.HTMLImageElement = TameImageElement; TameImageElement.prototype.getSrc = function() { return this.node___.src; }; TameImageElement.prototype.setSrc = function(src) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.setAttribute('src', src); return src;
}; TameImageElement.prototype.getAlt = function() { return this.node___.alt; }; TameImageElement.prototype.setAlt = function(src) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.alt = src; return src;
}; ___.ctor(TameImageElement, TameElement, 'TameImageElement'); ___.all2(___.grantTypedGeneric, TameImageElement.prototype, ['getSrc', 'setSrc', 'getAlt', 'setAlt']); function 
TameTableCompElement(node, editable) { TameElement.call(this, node, editable); classUtils.exportFields(this, ['colSpan', 'cells', 'rowSpan', 'rows', 'rowIndex', 'align', 'vAlign', 'nowrap']); } classUtils.extend(TameTableCompElement, TameElement); TameTableCompElement.prototype.getColSpan = function() { return this.node___.colSpan; }; TameTableCompElement.prototype.setColSpan = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.colSpan = newValue; return newValue;
}; TameTableCompElement.prototype.getCells = function() { return tameNodeList(this.node___.cells, this.editable___); }; TameTableCompElement.prototype.getRowSpan = function() { return this.node___.rowSpan; }; TameTableCompElement.prototype.setRowSpan = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.rowSpan = newValue; return newValue;
}; TameTableCompElement.prototype.getRows = function() { return tameNodeList(this.node___.rows, this.editable___); }; TameTableCompElement.prototype.getRowIndex = function() { return this.node___.rowIndex; }; TameTableCompElement.prototype.getAlign = function() { return this.node___.align; }; TameTableCompElement.prototype.setAlign = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.align = newValue; return newValue;
}; TameTableCompElement.prototype.getVAlign = function() { return this.node___.vAlign; }; TameTableCompElement.prototype.setVAlign = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.vAlign = newValue; return newValue;
}; TameTableCompElement.prototype.getNowrap = function() { return this.node___.nowrap; }; TameTableCompElement.prototype.setNowrap = function(newValue) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.nowrap = newValue; return newValue;
}; ___.ctor(TameTableCompElement, TameElement, 'TameTableCompElement'); function 
TameTableElement(node, editable) { TameTableCompElement.call(this, node, editable); classUtils.exportFields(this, ['tBodies', 'tHead', 'tFoot']); } classUtils.extend(TameTableElement, TameTableCompElement); nodeClasses.HTMLTableElement = TameTableElement; TameTableElement.prototype.getTBodies = function() { return tameNodeList(this.node___.tBodies, this.editable___); }; TameTableElement.prototype.getTHead = function() { return tameNode(this.node___.tHead, this.editable___); }; TameTableElement.prototype.getTFoot = function() { return tameNode(this.node___.tFoot, this.editable___); }; TameTableElement.prototype.createTHead = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return tameNode(this.node___.createTHead(), this.editable___);
}; TameTableElement.prototype.deleteTHead = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.deleteTHead();
}; TameTableElement.prototype.createTFoot = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return tameNode(this.node___.createTFoot(), this.editable___);
}; TameTableElement.prototype.deleteTFoot = function() {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } this.node___.deleteTFoot();
}; ___.ctor(TameTableElement, TameTableCompElement, 'TameTableElement'); ___.all2(___.grantTypedGeneric, TameTableElement.prototype, ['createTHead', 'deleteTHead', 'createTFoot', 'deleteTFoot']); function 
tameEvent(event) {
    if (event.tamed___) { return event.tamed___; } return event.tamed___ = new 
TameEvent(event);
} function TameEvent(event) { this.event___ = event; ___.stamp(tameEventTrademark, this, true); classUtils.exportFields(this, ['type', 'target', 'pageX', 'pageY', 'altKey', 'ctrlKey', 'metaKey', 'shiftKey', 'button', 'screenX', 'screenY', 'currentTarget', 'relatedTarget', 'fromElement', 'toElement', 'srcElement', 'clientX', 'clientY', 'keyCode', 'which']); } nodeClasses.Event = TameEvent; TameEvent.prototype.getType = function() {
    var 
type = String(this.event___.type); var suffix = CUSTOM_EVENT_TYPE_SUFFIX; var tlen = type.length, slen = suffix.length; var 
end = tlen - slen; if (end >= 0) { type = type.substring(0, end); } return type;
}; TameEvent.prototype.getTarget = function() {
    var 
event = this.event___; return tameRelatedNode(event.target || event.srcElement, true);
}; TameEvent.prototype.getSrcElement = function() { return tameRelatedNode(this.event___.srcElement, true); }; TameEvent.prototype.getCurrentTarget = function() {
    var 
e = this.event___; return tameRelatedNode(e.currentTarget, true);
}; TameEvent.prototype.getRelatedTarget = function() {
    var 
e = this.event___; var t = e.relatedTarget; if (!t) {
        if (e.type === 'mouseout') { t = e.toElement; } else
            if (e.type === 'mouseover') { t = e.fromElement; } 
    } return tameRelatedNode(t, true);
}; TameEvent.prototype.getFromElement = function() { return tameRelatedNode(this.event___.fromElement, true); }; TameEvent.prototype.getToElement = function() { return tameRelatedNode(this.event___.toElement, true); }; TameEvent.prototype.getPageX = function() { return Number(this.event___.pageX); }; TameEvent.prototype.getPageY = function() { return Number(this.event___.pageY); }; TameEvent.prototype.stopPropagation = function() { if (this.event___.stopPropagation) { this.event___.stopPropagation(); } else { this.event___.cancelBubble = true; } }; TameEvent.prototype.preventDefault = function() { if (this.event___.preventDefault) { this.event___.preventDefault(); } else { this.event___.returnValue = false; } }; TameEvent.prototype.getAltKey = function() { return Boolean(this.event___.altKey); }; TameEvent.prototype.getCtrlKey = function() { return Boolean(this.event___.ctrlKey); }; TameEvent.prototype.getMetaKey = function() { return Boolean(this.event___.metaKey); }; TameEvent.prototype.getShiftKey = function() { return Boolean(this.event___.shiftKey); }; TameEvent.prototype.getButton = function() {
    var 
e = this.event___; return e.button && Number(e.button);
}; TameEvent.prototype.getClientX = function() { return Number(this.event___.clientX); }; TameEvent.prototype.getClientY = function() { return Number(this.event___.clientY); }; TameEvent.prototype.getScreenX = function() { return Number(this.event___.screenX); }; TameEvent.prototype.getScreenY = function() { return Number(this.event___.screenY); }; TameEvent.prototype.getWhich = function() {
    var 
w = this.event___.which; return w && Number(w);
}; TameEvent.prototype.getKeyCode = function() {
    var 
kc = this.event___.keyCode; return kc && Number(kc);
}; TameEvent.prototype.toString = function() { return '[Fake Event]'; }; ___.ctor(TameEvent, void 
0, 'TameEvent'); ___.all2(___.grantTypedGeneric, TameEvent.prototype, ['getType', 'getTarget', 'getPageX', 'getPageY', 'stopPropagation', 'getAltKey', 'getCtrlKey', 'getMetaKey', 'getShiftKey', 'getButton', 'getClientX', 'getClientY', 'getScreenX', 'getScreenY', 'getRelatedTarget', 'getFromElement', 'getToElement', 'getSrcElement', 'preventDefault', 'getKeyCode', 'getWhich']); function 
TameCustomHTMLEvent(event) { TameEvent.call(this, event); this.properties___ = {}; } classUtils.extend(TameCustomHTMLEvent, TameEvent); TameCustomHTMLEvent.prototype.initEvent = function(type, bubbles, cancelable) { type = tameEventName(type, true); bubbles = Boolean(bubbles); cancelable = Boolean(cancelable); this.event___.initEvent(type, bubbles, cancelable); }; TameCustomHTMLEvent.prototype.handleRead___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        return void 
0;
    } var handlerName = name + '_getter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (___.hasOwnProp(this.event___.properties___, name)) { return this.event___.properties___[name]; } else {
        return void 
0;
    } 
}; TameCustomHTMLEvent.prototype.handleCall___ = function(name, args) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } var handlerName = name + '_handler___'; if (this[handlerName]) { return this[handlerName].call(this, args); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName].call(this, args); } if (___.hasOwnProp(this.event___.properties___, name)) { return this.event___.properties___[name].call(this, args); } else {
        throw new 
TypeError(name + ' is not a function.');
    } 
}; TameCustomHTMLEvent.prototype.handleSet___ = function(name, val) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } var handlerName = name + '_setter___'; if (this[handlerName]) { return this[handlerName](val); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](val); } if (!this.event___.properties___) { this.event___.properties___ = {}; } this[name + '_canEnum___'] = true; return this.event___.properties___[name] = val;
}; TameCustomHTMLEvent.prototype.handleDelete___ = function(name) {
    name = String(name); if (endsWith__.test(name)) {
        throw new 
Error(INVALID_SUFFIX);
    } var handlerName = name + '_deleter___'; if (this[handlerName]) { return this[handlerName](); } handlerName = handlerName.toLowerCase(); if (this[handlerName]) { return this[handlerName](); } if (this.event___.properties___) {
        return delete 
this.event___.properties___[name] && delete this[name + '_canEnum___'];
    } else { return true; } 
}; TameCustomHTMLEvent.prototype.handleEnum___ = function(ownFlag) { if (this.event___.properties___) { return cajita.allKeys(this.event___.properties___); } return []; }; TameCustomHTMLEvent.prototype.toString = function() { return '[Fake CustomEvent]'; }; ___.grantTypedGeneric(TameCustomHTMLEvent.prototype, 'initEvent'); ___.ctor(TameCustomHTMLEvent, TameEvent, 'TameCustomHTMLEvent'); function 
fakeNodeList(array) { array.item = ___.func(function(i) { return array[i]; }); return cajita.freeze(array); } function 
TameHTMLDocument(doc, body, domain, editable) {
    TamePseudoNode.call(this, editable); this.doc___ = doc; this.body___ = body; this.domain___ = domain; var 
tameDoc = this; var tameBody = tameNode(body, editable); var tameBodyElement = new TamePseudoElement('BODY', this, function() { return tameNodeList(body.childNodes, editable); }, function() { return tameHtmlElement; }, function() { return tameInnerHtml(body.innerHTML); }, editable); cajita.forOwnKeys({ 'appendChild': 0, 'removeChild': 0, 'insertBefore': 0, 'replaceChild': 0 }, ___.frozenFunc(function(k) { tameBodyElement[k] = tameBody[k].bind(tameBody); ___.grantFunc(tameBodyElement, k); })); var 
title = doc.createTextNode(body.getAttribute('title') || ''); var tameTitleElement = new 
TamePseudoElement('TITLE', this, function() { return [tameNode(title, false)]; }, function() { return tameHeadElement; }, function() { return html.escapeAttrib(title.nodeValue); }, editable); var 
tameHeadElement = new TamePseudoElement('HEAD', this, function() { return [tameTitleElement]; }, function() { return tameHtmlElement; }, function() { return '<title>' + tameTitleElement.getInnerHTML() + '</title>'; }, editable); var 
tameHtmlElement = new TamePseudoElement('HTML', this, function() { return [tameHeadElement, tameBodyElement]; }, function() { return tameDoc; }, function() { return '<head>' + tameHeadElement.getInnerHTML + '</head><body>' + tameBodyElement.getInnerHTML() + '</body>'; }, editable); this.documentElement___ = tameHtmlElement; classUtils.exportFields(this, ['documentElement', 'body', 'title', 'domain']);
} classUtils.extend(TameHTMLDocument, TamePseudoNode); nodeClasses.HTMLDocument = TameHTMLDocument; TameHTMLDocument.prototype.getNodeType = function() { return 9; }; TameHTMLDocument.prototype.getNodeName = function() { return '#document'; }; TameHTMLDocument.prototype.getNodeValue = function() { return null; }; TameHTMLDocument.prototype.getChildNodes = function() { return [this.documentElement___]; }; TameHTMLDocument.prototype.getAttributes = function() { return []; }; TameHTMLDocument.prototype.getParentNode = function() { return null; }; TameHTMLDocument.prototype.getElementsByTagName = function(tagName) { tagName = String(tagName).toLowerCase(); switch (tagName) { case 'body': return fakeNodeList([this.getBody()]); case 'head': return fakeNodeList([this.getHead()]); case 'title': return fakeNodeList([this.getTitle()]); default: return tameGetElementsByTagName(this.body___, tagName, this.editable___); } }; TameHTMLDocument.prototype.getDocumentElement = function() { return this.documentElement___; }; TameHTMLDocument.prototype.getBody = function() { return this.documentElement___.getLastChild(); }; TameHTMLDocument.prototype.getHead = function() { return this.documentElement___.getFirstChild(); }; TameHTMLDocument.prototype.getTitle = function() { return this.getHead().getFirstChild(); }; TameHTMLDocument.prototype.getDomain = function() { return this.domain___; }; TameHTMLDocument.prototype.getElementsByClassName = function(className) { return tameGetElementsByClassName(this.body___, className, this.editable___); }; TameHTMLDocument.prototype.addEventListener = function(name, listener, useCapture) { }; TameHTMLDocument.prototype.removeEventListener = function(name, listener, useCapture) { }; TameHTMLDocument.prototype.createElement = function(tagName) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } tagName = String(tagName).toLowerCase(); if (!html4.ELEMENTS.hasOwnProperty(tagName)) {
        throw new 
Error(UNKNOWN_TAGNAME + '[' + tagName + ']');
    } var flags = html4.ELEMENTS[tagName]; if (flags & html4
.eflags.UNSAFE) { throw new Error(UNSAFE_TAGNAME + '[' + tagName + ']'); } var newEl = this.doc___.createElement(tagName); if (elementPolicies.hasOwnProperty(tagName)) {
        var 
attribs = elementPolicies[tagName]([]); if (attribs) { for (var i = 0; i < attribs.length; i += 2) { bridal.setAttribute(newEl, attribs[i], attribs[i + 1]); } } 
    } return tameNode(newEl, true);
}; TameHTMLDocument.prototype.createTextNode = function(text) {
    if (!this.editable___) {
        throw new 
Error(NOT_EDITABLE);
    } return tameNode(this.doc___.createTextNode(text !== null && text !== void 
0 ? '' + text : ''), true);
}; TameHTMLDocument.prototype.getElementById = function(id) {
    id += idSuffix; var 
node = this.doc___.getElementById(id); return tameNode(node, this.editable___);
}; TameHTMLDocument.prototype.toString = function() { return '[Fake Document]'; }; TameHTMLDocument.prototype.write = function(text) { cajita.log('Called document.write() with: ' + text); }; TameHTMLDocument.prototype.createEvent = function(type) {
    type = String(type); if (type !== 'HTMLEvents') {
        throw new 
Error('Unrecognized event type ' + type);
    } var document = this.doc___; var rawEvent; if (document.createEvent) { rawEvent = document.createEvent(type); } else { rawEvent = document.createEventObject(); rawEvent.eventType = 'ondataavailable'; } var 
tamedEvent = new TameCustomHTMLEvent(rawEvent); rawEvent.tamed___ = tamedEvent; return tamedEvent;
}; ___.ctor(TameHTMLDocument, TamePseudoNode, 'TameHTMLDocument'); ___.all2(___.grantTypedGeneric, TameHTMLDocument.prototype, ['addEventListener', 'removeEventListener', 'createElement', 'createTextNode', 'createEvent', 'getElementById', 'getElementsByClassName', 'getElementsByTagName', 'write']); imports.tameNode___ = tameNode; imports.tameEvent___ = tameEvent; imports.blessHtml___ = blessHtml; imports.blessCss___ = function(var_args) {
    var 
arr = []; for (var i = 0, n = arguments.length; i < n; ++i) { arr[i] = arguments[i]; } return cssSealerUnsealerPair.seal(arr);
}; imports.htmlAttr___ = function(s) { return html.escapeAttrib(String(s || '')); }; imports.html___ = safeHtml; imports.rewriteUri___ = function(uri, mimeType) {
    var 
s = rewriteAttribute(null, null, html4.atype.URI, uri); if (!s) { throw new Error(); } return s;
}; imports.suffix___ = function(nmtokens) {
    var 
p = String(nmtokens).replace(/^\s+|\s+$/g, '').split(/\s+/g); var out = []; for (var i = 0; i < p.length; ++i) {
        var 
nmtoken = rewriteAttribute(null, null, html4.atype.ID, p[i]); if (!nmtoken) { throw new Error(nmtokens); } out.push(nmtoken);
    } return out.join(' ');
}; imports.ident___ = function(nmtokens) {
    var 
p = String(nmtokens).replace(/^\s+|\s+$/g, '').split(/\s+/g); var out = []; for (var i = 0; i < p.length; ++i) {
        var 
nmtoken = rewriteAttribute(null, null, html4.atype.CLASSES, p[i]); if (!nmtoken) {
            throw new 
Error(nmtokens);
        } out.push(nmtoken);
    } return out.join(' ');
}; function TameStyle(style, editable) { this.style___ = style; this.editable___ = editable; } nodeClasses.Style = TameStyle; for (var 
styleProperty in css.properties) {
        if (!cajita.canEnumOwn(css.properties, styleProperty)) { continue; } (function(propertyName) {
            ___.useGetHandler(TameStyle.prototype, propertyName, function() {
                if (!this.style___) {
                    return void 
0;
                } return String(this.style___[propertyName] || '');
            }); var pattern = css.properties[propertyName]; ___.useSetHandler(TameStyle.prototype, propertyName, function(val) {
                if (!this.editable___) {
                    throw new 
Error('style not editable');
                } val = '' + (val || ''); if (val && !pattern.test(val + ' ')) {
                    throw new 
Error('bad value `' + val + '` for CSS property ' + propertyName);
                } val = val.replace(/\burl\s*\(\s*\"([^\"]*)\"\s*\)/gi, function(_, url) {
                    throw new 
Error('url in style ' + url);
                }); this.style___[propertyName] = val;
            });
        })(styleProperty);
    } TameStyle.prototype.toString = function() { return '[Fake Style]'; }; var 
COMPUTED_STYLE_WHITELIST = { 'display': true, 'filter': true, 'float': true, 'height': true, 'opacity': true, 'overflow': true, 'position': true, 'visibility': true, 'width': true }; function 
TameComputedStyle(style) { this.style___ = style; } cajita.forOwnKeys(COMPUTED_STYLE_WHITELIST, ___.func(function(propertyName, _) {
    ___.useGetHandler(TameComputedStyle.prototype, propertyName, function() {
        if (!this.style___) {
            return void 
0;
        } return String(this.style___[propertyName] || '');
    });
})); TameComputedStyle.prototype.toString = function() { return '[Fake Computed Style]'; }; nodeClasses.XMLHttpRequest = domitaModules.TameXMLHttpRequest(domitaModules.XMLHttpRequestCtor(window.XMLHttpRequest, window.ActiveXObject), uriCallback); imports.cssNumber___ = function(num) {
    if ('number' === typeof 
num && isFinite(num) && !isNaN(num)) { return '' + num; } throw new Error(num);
}; imports.cssColor___ = function(color) {
    if ('number' !== typeof 
color || color != (color | 0)) { throw new Error(color); } var hex = '0123456789abcdef'; return '#' + hex.charAt(color >> 20 & 15) + hex.charAt(color >> 16 & 15) + hex.charAt(color >> 12 & 15) + hex.charAt(color >> 8 & 15) + hex.charAt(color >> 4 & 15) + hex.charAt(color & 15);
}; imports.cssUri___ = function(uri, mimeType) {
    var 
s = rewriteAttribute(null, null, html4.atype.URI, uri); if (!s) { throw new Error(); } return s;
}; imports.emitCss___ = function(cssText) { this.getCssContainer___().appendChild(bridal.createStylesheet(document, cssText)); }; imports.getCssContainer___ = function() { return (document.getElementsByTagName('head'))[0]; }; if (!/^-/.test(idSuffix)) {
        throw new 
Error('id suffix \"' + idSuffix + '\" must start with \"-\"');
    } var idClass = idSuffix.substring(1); imports.getIdClass___ = function() { return idClass; }; imports.setTimeout = tameSetTimeout; imports.setInterval = tameSetInterval; imports.clearTimeout = tameClearTimeout; imports.clearInterval = tameClearInterval; var 
tameDocument = new TameHTMLDocument(document, pseudoBodyNode, String(optPseudoWindowLocation.hostname) || 'nosuchhost,fake', true); imports.document = tameDocument; var 
tameLocation = ___.primFreeze({ 'toString': ___.frozenFunc(function() { return tameLocation.href; }), 'href': String(optPseudoWindowLocation.href) || 'http://nosuchhost,fake/', 'hash': String(optPseudoWindowLocation.hash) || '', 'host': String(optPseudoWindowLocation.host) || 'nosuchhost,fake', 'hostname': String(optPseudoWindowLocation.hostname) || 'nosuchhost,fake', 'pathname': String(optPseudoWindowLocation.pathname) || '/', 'port': String(optPseudoWindowLocation.port) || '', 'protocol': String(optPseudoWindowLocation.protocol) || 'http:', 'search': String(optPseudoWindowLocation.search) || '' }); var 
tameNavigator = ___.primFreeze({ 'appCodeName': 'Caja', 'appName': 'Sandbox', 'appVersion': '1.0', 'language': '', 'platform': 'Caja', 'oscpu': 'Caja', 'vendor': '', 'vendorSub': '', 'product': 'Caja', 'productSub': '', 'userAgent': 'Caja/1.0' }); var 
PSEUDO_ELEMENT_WHITELIST = { 'first-letter': true, 'first-line': true }; var tameWindow = { 'document': imports.document, 'location': tameLocation, 'navigator': tameNavigator, 'setTimeout': tameSetTimeout, 'setInterval': tameSetInterval, 'clearTimeout': tameClearTimeout, 'clearInterval': tameClearInterval, 'scrollTo': ___.frozenFunc(function(x, y) {
    if ('number' === typeof 
x && 'number' === typeof y && !isNaN(x - y) && imports.isProcessingEvent___) { window.scrollTo(x, y); } 
}), 'addEventListener': ___.frozenFunc(function(name, listener, useCapture) { }), 'removeEventListener': ___.frozenFunc(function(name, listener, useCapture) { }), 'dispatchEvent': ___.frozenFunc(function(evt) { }), 'getComputedStyle': ___.frozenFunc(function(tameElement, pseudoElement) {
    cajita.guard(tameNodeTrademark, tameElement); pseudoElement = pseudoElement === null || pseudoElement === void 
0 ? void 0 : String(pseudoElement).toLowerCase(); if (pseudoElement !== void 0 && !PSEUDO_ELEMENT_WHITELIST.hasOwnProperty(pseudoElement)) {
        throw new 
Error('Bad pseudo class ' + pseudoElement);
    } var rawNode = tameElement.node___; if (rawNode.currentStyle && pseudoElement === void 
0) { return new TameComputedStyle(rawNode.currentStyle); } else if (window.getComputedStyle) {
        var 
rawStyleNode = window.getComputedStyle(tameElement.node___, pseudoElement); return new 
TameComputedStyle(rawStyleNode);
    } else { throw new Error('Computed style not available for pseudo element ' + pseudoElement); } 
})
}; tameWindow.top = tameWindow.self = tameWindow.opener = tameWindow.parent = tameWindow.window = tameWindow; cajita.forOwnKeys(nodeClasses, ___.func(function(name, ctor) { ___.primFreeze(ctor); tameWindow[name] = ctor; ___.grantRead(tameWindow, name); })); var 
defaultNodeClasses = ['HTMLAppletElement', 'HTMLAreaElement', 'HTMLBaseElement', 'HTMLBaseFontElement', 'HTMLBodyElement', 'HTMLBRElement', 'HTMLButtonElement', 'HTMLDirectoryElement', 'HTMLDivElement', 'HTMLDListElement', 'HTMLFieldSetElement', 'HTMLFontElement', 'HTMLFrameElement', 'HTMLFrameSetElement', 'HTMLHeadElement', 'HTMLHeadingElement', 'HTMLHRElement', 'HTMLHtmlElement', 'HTMLIFrameElement', 'HTMLIsIndexElement', 'HTMLLabelElement', 'HTMLLegendElement', 'HTMLLIElement', 'HTMLLinkElement', 'HTMLMapElement', 'HTMLMenuElement', 'HTMLMetaElement', 'HTMLModElement', 'HTMLObjectElement', 'HTMLOListElement', 'HTMLOptGroupElement', 'HTMLOptionElement', 'HTMLParagraphElement', 'HTMLParamElement', 'HTMLPreElement', 'HTMLQuoteElement', 'HTMLScriptElement', 'HTMLSelectElement', 'HTMLStyleElement', 'HTMLTableCaptionElement', 'HTMLTableCellElement', 'HTMLTableColElement', 'HTMLTableElement', 'HTMLTableRowElement', 'HTMLTableSectionElement', 'HTMLTextAreaElement', 'HTMLTitleElement', 'HTMLUListElement']; var 
defaultNodeClassCtor = ___.primFreeze(TameElement); for (var i = 0; i < defaultNodeClasses.length; i++) { tameWindow[defaultNodeClasses[i]] = defaultNodeClassCtor; ___.grantRead(tameWindow, defaultNodeClasses[i]); } var 
outers = imports.outers; if (___.isJSONContainer(outers)) {
        for (var k in tameWindow) {
            if (!___.hasOwnProp(outers, k) && ___.canEnumPub(tameWindow, k)) {
                var 
v = tameWindow[k]; outers[k] = v === tameWindow ? outers : v;
            } 
        } outers.window = outers;
    } else { cajita.freeze(tameWindow); imports.window = tameWindow; } 
} return attachDocumentStub;
})(); function 
plugin_dispatchEvent___(thisNode, event, pluginId, handler) {
    event = event || window.event; if (typeof 
console !== 'undefined' && console.log) { var sig = String(handler).match(/^function\b[^\)]*\)/); console.log('Dispatch %s event thisNode=%o, event=%o, pluginId=%o, handler=%o', event && event.type, thisNode, event, pluginId, sig ? sig[0] : handler); } var 
imports = ___.getImports(pluginId); switch (typeof handler) {
        case 'string': { handler = imports[handler]; break; } case 'function': ; case 'object': break; default: throw new 
Error('Expected function as event handler, not ' + typeof handler);
    } if (___.startCallerStack) { ___.startCallerStack(); } imports.isProcessingEvent___ = true; try { return ___.callPub(handler, 'call', [imports.tameNode___(thisNode, true), imports.tameEvent___(event)]); } catch (ex) {
        if (ex && ex.cajitaStack___ && 'undefined' !== typeof 
console) { console.error('Event dispatch %s: %s', handler, ___.unsealCallerStack(ex.cajitaStack___).join('\n')); } throw ex;
    } finally { imports.isProcessingEvent___ = false; } 
} 
    } 
}