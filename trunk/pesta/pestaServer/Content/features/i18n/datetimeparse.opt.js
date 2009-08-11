var gadgets=gadgets||{};gadgets.i18n=gadgets.i18n||{};gadgets.i18n.DateTimeParse=function(a){this.symbols_=a};gadgets.i18n.DateTimeParse.prototype.year=0;gadgets.i18n.DateTimeParse.prototype.month=0;gadgets.i18n.DateTimeParse.prototype.dayOfMonth=0;gadgets.i18n.DateTimeParse.prototype.hours=0;gadgets.i18n.DateTimeParse.prototype.minutes=0;gadgets.i18n.DateTimeParse.prototype.seconds=0;gadgets.i18n.DateTimeParse.prototype.milliseconds=0;gadgets.i18n.DateTimeParse.ambiguousYearCenturyStart=80;gadgets.i18n.DateTimeParse.prototype.applyPattern=function(e){this.patternParts_=[];var f=false;var a="";for(var b=0;b<e.length;b++){var c=e.charAt(b);if(c==" "){if(a.length>0){this.patternParts_.push({text:a,count:0,abutStart:false});a=""}this.patternParts_.push({text:" ",count:0,abutStart:false});while(b+1<e.length&&e.charAt(b+1)==" "){b++}}else{if(f){if(c=="'"){if(b+1<e.length&&e.charAt(b+1)=="'"){a+=c;++b}else{f=false}}else{a+=c}}else{if(gadgets.i18n.DateTimeParse.PATTERN_CHARS_.indexOf(c)>=0){if(a.length>0){this.patternParts_.push({text:a,count:0,abutStart:false});a=""}var d=this.getNextCharCount_(e,b);this.patternParts_.push({text:c,count:d,abutStart:false});b+=d-1}else{if(c=="'"){if(b+1<e.length&&e.charAt(b+1)=="'"){a+="'";b++}else{f=true}}else{a+=c}}}}}if(a.length>0){this.patternParts_.push({text:a,count:0,abutStart:false})}this.markAbutStart_()};gadgets.i18n.DateTimeParse.prototype.applyStandardPattern=function(b){var a;if(b<4){a=this.symbols_.DATEFORMATS[b]}else{if(b<8){a=this.symbols_.TIMEFORMATS[b-4]}else{if(b<12){a=this.symbols_.DATEFORMATS[b-8];a+=" ";a+=this.symbols_.TIMEFORMATS[b-8]}else{return this.applyStandardPattern(gadgets.i18n.MEDIUM_DATETIME_FORMAT)}}}return this.applyPattern(a)};gadgets.i18n.DateTimeParse.prototype.parse=function(b,c,a){return this.internalParse_(b,c,a,false)};gadgets.i18n.DateTimeParse.prototype.internalParse_=function(k,b,d,e){var a=new gadgets.i18n.DateTimeParse.MyDate_();var l=[b];var j=-1;var c=0;var h=0;for(var f=0;f<this.patternParts_.length;++f){if(this.patternParts_[f].count>0){if(j<0&&this.patternParts_[f].abutStart){j=f;c=b;h=0}if(j>=0){var g=this.patternParts_[f].count;if(f==j){g-=h;h++;if(g==0){return 0}}if(!this.subParse_(k,l,this.patternParts_[f],g,a)){f=j-1;l[0]=c;continue}}else{j=-1;if(!this.subParse_(k,l,this.patternParts_[f],0,a)){return 0}}}else{j=-1;if(this.patternParts_[f].text.charAt(0)==" "){var m=l[0];this.skipSpace_(k,l);if(l[0]>m){continue}}else{if(k.indexOf(this.patternParts_[f].text,l[0])==l[0]){l[0]+=this.patternParts_[f].text.length;continue}}return 0}}return a.calcDate_(d,e)?l[0]-b:0};gadgets.i18n.DateTimeParse.prototype.getNextCharCount_=function(c,d){var b=c.charAt(d);var a=d+1;while(a<c.length&&c.charAt(a)==b){++a}return a-d};gadgets.i18n.DateTimeParse.PATTERN_CHARS_="GyMdkHmsSEDahKzZv";gadgets.i18n.DateTimeParse.NUMERIC_FORMAT_CHARS_="MydhHmsSDkK";gadgets.i18n.DateTimeParse.prototype.isNumericField_=function(a){if(a.count<=0){return false}var b=gadgets.i18n.DateTimeParse.NUMERIC_FORMAT_CHARS_.indexOf(a.text.charAt(0));return b>0||b==0&&a.count<3};gadgets.i18n.DateTimeParse.prototype.markAbutStart_=function(){var b=false;for(var a=0;a<this.patternParts_.length;a++){if(this.isNumericField_(this.patternParts_[a])){if(!b&&a+1<this.patternParts_.length&&this.isNumericField_(this.patternParts_[a+1])){b=true;this.patternParts_[a].abutStart=true}}else{b=false}}};gadgets.i18n.DateTimeParse.prototype.skipSpace_=function(b,c){var a=b.substring(c[0]).match(/^\s+/);if(a){c[0]+=a[0].length}};gadgets.i18n.DateTimeParse.prototype.subParse_=function(f,h,b,a,e){this.skipSpace_(f,h);var g=h[0];var c=b.text.charAt(0);var d=-1;if(this.isNumericField_(b)){if(a>0){if((g+a)>f.length){return false}d=this.parseInt_(f.substring(0,g+a),h)}else{d=this.parseInt_(f,h)}}switch(c){case"G":e.era=this.matchString_(f,h,this.symbols_.ERAS);return true;case"M":return this.subParseMonth_(f,h,e,d);case"E":return this.subParseDayOfWeek_(f,h,e);case"a":e.ampm=this.matchString_(f,h,this.symbols_.AMPMS);return true;case"y":return this.subParseYear_(f,h,g,d,b,e);case"d":e.day=d;return true;case"S":return this.subParseFractionalSeconds_(d,h,g,e);case"h":if(d==12){d=0}case"K":case"H":case"k":e.hours=d;return true;case"m":e.minutes=d;return true;case"s":e.seconds=d;return true;case"z":case"Z":case"v":return this.subparseTimeZoneInGMT_(f,h,e);default:return false}};gadgets.i18n.DateTimeParse.prototype.subParseYear_=function(e,g,f,c,a,d){var b;if(c<0){b=e.charAt(g[0]);if(b!="+"&&b!="-"){return false}g[0]++;c=this.parseInt_(e,g);if(c<0){return false}if(b=="-"){c=-c}}if(b==null&&(g[0]-f)==2&&a.count==2){d.setTwoDigitYear_(c)}else{d.year=c}return true};gadgets.i18n.DateTimeParse.prototype.subParseMonth_=function(c,d,b,a){if(a<0){a=this.matchString_(c,d,this.symbols_.MONTHS);if(a<0){a=this.matchString_(c,d,this.symbols_.SHORTMONTHS)}if(a<0){return false}b.month=a;return true}else{b.month=a-1;return true}};gadgets.i18n.DateTimeParse.prototype.subParseDayOfWeek_=function(c,d,b){var a=this.matchString_(c,d,this.symbols_.WEEKDAYS);if(a<0){a=this.matchString_(c,d,this.symbols_.SHORTWEEKDAYS)}if(a<0){return false}b.dayOfWeek=a;return true};gadgets.i18n.DateTimeParse.prototype.subParseFractionalSeconds_=function(b,e,d,c){var a=e[0]-d;c.milliseconds=a<3?b*Math.pow(10,3-a):Math.round(b/Math.pow(10,a-3));return true};gadgets.i18n.DateTimeParse.prototype.subparseTimeZoneInGMT_=function(b,c,a){if(b.indexOf("GMT",c[0])==c[0]){c[0]+=3;return this.parseTimeZoneOffset_(b,c,a)}return this.parseTimeZoneOffset_(b,c,a)};gadgets.i18n.DateTimeParse.prototype.parseTimeZoneOffset_=function(f,g,d){if(g[0]>=f.length){d.tzOffset=0;return true}var a=1;switch(f.charAt(g[0])){case"-":a=-1;case"+":g[0]++}var b=g[0];var c=this.parseInt_(f,g);if(c==0&&g[0]==b){return false}var e;if(g[0]<f.length&&f.charAt(g[0])==":"){e=c*60;g[0]++;b=g[0];c=this.parseInt_(f,g);if(c==0&&g[0]==b){return false}e+=c}else{e=c;if(e<24&&(g[0]-b)<=2){e*=60}else{e=e%100+e/100*60}}e*=a;d.tzOffset=-e;return true};gadgets.i18n.DateTimeParse.prototype.parseInt_=function(b,c){var a=b.substring(c[0]).match(/^\d+/);if(!a){return -1}c[0]+=a[0].length;return parseInt(a[0],10)};gadgets.i18n.DateTimeParse.prototype.matchString_=function(g,h,f){var c=0;var e=-1;var d=g.substring(h[0]).toLowerCase();for(var b=0;b<f.length;++b){var a=f[b].length;if(a>c&&d.indexOf(f[b].toLowerCase())==0){e=b;c=a}}if(e>=0){h[0]+=c}return e};gadgets.i18n.DateTimeParse.MyDate_=function(){};gadgets.i18n.DateTimeParse.MyDate_.prototype.setTwoDigitYear_=function(b){var a=new Date();var d=a.getFullYear()-gadgets.i18n.DateTimeParse.ambiguousYearCenturyStart;var c=d%100;this.ambiguousYear=(b==c);b+=Math.floor(d/100)*100+(b<c?100:0);return this.year=b};gadgets.i18n.DateTimeParse.MyDate_.prototype.calcDate_=function(d,a){if(this.era!=undefined&&this.year!=undefined&&this.era==0&&this.year>0){this.year=-(this.year-1)}if(this.year!=undefined){d.setFullYear(this.year)}var e=d.getDate();d.setDate(1);if(this.month!=undefined){d.setMonth(this.month)}if(this.day!=undefined){d.setDate(this.day)}else{d.setDate(e)}if(this.hours==undefined){this.hours=d.getHours()}if(this.ampm!=undefined&&this.ampm>0){if(this.hours<12){this.hours+=12}}d.setHours(this.hours);if(this.minutes!=undefined){d.setMinutes(this.minutes)}if(this.seconds!=undefined){d.setSeconds(this.seconds)}if(this.milliseconds!=undefined){d.setMilliseconds(this.milliseconds)}if(a&&(this.year!=undefined&&this.year!=d.getFullYear()||this.month!=undefined&&this.month!=d.getMonth()||this.dayOfMonth!=undefined&&this.dayOfMonth!=d.getDate()||this.hours>=24||this.minutes>=60||this.seconds>=60||this.milliseconds>=1000)){return false}if(this.tzOffset!=undefined){var g=d.getTimezoneOffset();d.setTime(d.getTime()+(this.tzOffset-g)*60*1000)}if(this.ambiguousYear){var c=new Date();c.setFullYear(c.getFullYear()-gadgets.i18n.DateTimeParse.ambiguousYearCenturyStart);if(d.getTime()<c.getTime()){d.setFullYear(c.getFullYear()+100)}}if(this.dayOfWeek!=undefined){if(this.day==undefined){var b=(7+this.dayOfWeek-d.getDay())%7;if(b>3){b-=7}var f=d.getMonth();d.setDate(d.getDate()+b);if(d.getMonth()!=f){d.setDate(d.getDate()+(b>0?-7:7))}}else{if(this.dayOfWeek!=d.getDay()){return false}}}return true};