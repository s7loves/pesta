function NumberFormatTest(a){TestCase.call(this,a)}NumberFormatTest.inherits(TestCase);var NumberFormatConstants_en={DECIMAL_SEP:".",GROUP_SEP:",",PERCENT:"%",ZERO_DIGIT:"0",PLUS_SIGN:"+",MINUS_SIGN:"-",EXP_SYMBOL:"E",PERMILL:"\u2030",INFINITY:"\u221E",NAN:"NaN",MONETARY_SEP:".",MONETARY_GROUP_SEP:",",DECIMAL_PATTERN:"#,##0.###",SCIENTIFIC_PATTERN:"#E0",PERCENT_PATTERN:"#,##0%",CURRENCY_PATTERN:"\u00A4#,##0.00",DEF_CURRENCY_CODE:"USD"};NumberFormatTest.prototype.setUp=function(){gadgets.i18n.numFormatter_=new gadgets.i18n.NumberFormat(NumberFormatConstants_en)};NumberFormatTest.prototype.testStandardFormat=function(){var a;a=gadgets.i18n.formatNumber(gadgets.i18n.CURRENCY_PATTERN,1234.579);this.assertEquals("$1,234.58",a);a=gadgets.i18n.formatNumber(gadgets.i18n.DECIMAL_PATTERN,1234.579);this.assertEquals("1,234.579",a);a=gadgets.i18n.formatNumber(gadgets.i18n.PERCENT_PATTERN,1234.579);this.assertEquals("123,458%",a);a=gadgets.i18n.formatNumber(gadgets.i18n.SCIENTIFIC_PATTERN,1234.579);this.assertEquals("1E3",a)};NumberFormatTest.prototype.testBasicParse=function(){var a;a=gadgets.i18n.parseNumber("0.0000","123.4579");this.assertEquals(123.4579,a);a=gadgets.i18n.parseNumber("0.0000","+123.4579");this.assertEquals(123.4579,a);a=gadgets.i18n.parseNumber("0.0000","-123.4579");this.assertEquals(-123.4579,a)};NumberFormatTest.prototype.testPrefixParse=function(){var a;a=gadgets.i18n.parseNumber("0.0;(0.0)","123.4579");this.assertEquals(123.4579,a);a=gadgets.i18n.parseNumber("0.0;(0.0)","(123.4579)");this.assertEquals(-123.4579,a)};NumberFormatTest.prototype.testPrecentParse=function(){var a;a=gadgets.i18n.parseNumber("0.0;(0.0)","123.4579%");this.assertEquals((123.4579/100),a);a=gadgets.i18n.parseNumber("0.0;(0.0)","(%123.4579)");this.assertEquals((-123.4579/100),a);a=gadgets.i18n.parseNumber("0.0;(0.0)","123.4579\u2030");this.assertEquals((123.4579/1000),a);a=gadgets.i18n.parseNumber("0.0;(0.0)","(\u2030123.4579)");this.assertEquals((-123.4579/1000),a)};NumberFormatTest.prototype.testPercentAndPerMillAdvance=function(){var a;var b=[0];a=gadgets.i18n.parseNumber("0","120%",b);this.assertEquals(1.2,a);this.assertEquals(4,b[0]);b[0]=0;a=gadgets.i18n.parseNumber("0","120\u2030",b);this.assertEquals(0.12,a);this.assertEquals(4,b[0])};NumberFormatTest.prototype.testInfinityParse=function(){var a;a=gadgets.i18n.parseNumber("0.0;(0.0)","\u221e");this.assertEquals(Number.POSITIVE_INFINITY,a);a=gadgets.i18n.parseNumber("0.0;(0.0)","(\u221e)");this.assertEquals(Number.NEGATIVE_INFINITY,a)};NumberFormatTest.prototype.testExponentParse=function(){var a;a=gadgets.i18n.parseNumber("#E0","1.234E3");this.assertEquals(1234,a);a=gadgets.i18n.parseNumber("0.###E0","1.234E3");this.assertEquals(1234,a);a=gadgets.i18n.parseNumber("#E0","1.2345E4");this.assertEquals(12345,a);a=gadgets.i18n.parseNumber("0E0","1.2345E4");this.assertEquals(12345,a);a=gadgets.i18n.parseNumber("0E0","1.2345E+4");this.assertEquals(12345,a)};NumberFormatTest.prototype.testGroupingParse=function(){var a;a=gadgets.i18n.parseNumber("#,###","1,234,567,890");this.assertEquals(1234567890,a);a=gadgets.i18n.parseNumber("#,####","12,3456,7890");this.assertEquals(1234567890,a);a=gadgets.i18n.parseNumber("#","1234567890");this.assertEquals(1234567890,a)};NumberFormatTest.prototype.testBasicFormat=function(){var a=gadgets.i18n.formatNumber("0.0000",123.45789179565757);this.assertEquals("123.4579",a)};NumberFormatTest.prototype.testGrouping=function(){var a;a=gadgets.i18n.formatNumber("#,###",1234567890);this.assertEquals("1,234,567,890",a);a=gadgets.i18n.formatNumber("#,####",1234567890);this.assertEquals("12,3456,7890",a);a=gadgets.i18n.formatNumber("#",1234567890);this.assertEquals("1234567890",a)};NumberFormatTest.prototype.testPerMill=function(){var a;a=gadgets.i18n.formatNumber("###.###\u2030",0.4857);this.assertEquals("485.7\u2030",a)};NumberFormatTest.prototype.testCurrency=function(){var a;a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",1234.56);this.assertEquals("$1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",-1234.56);this.assertEquals("-$1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4\u00a4 #,##0.00;-\u00a4\u00a4 #,##0.00",1234.56);this.assertEquals("USD 1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4\u00a4 #,##0.00;\u00a4\u00a4 -#,##0.00",-1234.56);this.assertEquals("USD -1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",1234.56,"BRL");this.assertEquals("R$1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",-1234.56,"BRL");this.assertEquals("-R$1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4\u00a4 #,##0.00;(\u00a4\u00a4 #,##0.00)",1234.56,"BRL");this.assertEquals("BRL 1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4\u00a4 #,##0.00;(\u00a4\u00a4 #,##0.00)",-1234.56,"BRL");this.assertEquals("(BRL 1,234.56)",a)};NumberFormatTest.prototype.testQuotes=function(){var a;a=gadgets.i18n.formatNumber("a'fo''o'b#",123);this.assertEquals("afo'ob123",a);a=gadgets.i18n.formatNumber("a''b#",123);this.assertEquals("a'b123",a)};NumberFormatTest.prototype.testZeros=function(){var a;a=gadgets.i18n.formatNumber("#.#",0);this.assertEquals("0",a);a=gadgets.i18n.formatNumber("#.",0);this.assertEquals("0.",a);a=gadgets.i18n.formatNumber(".#",0);this.assertEquals(".0",a);a=gadgets.i18n.formatNumber("#",0);this.assertEquals("0",a);a=gadgets.i18n.formatNumber("#0.#",0);this.assertEquals("0",a);a=gadgets.i18n.formatNumber("#0.",0);this.assertEquals("0.",a);a=gadgets.i18n.formatNumber("#.0",0);this.assertEquals(".0",a);a=gadgets.i18n.formatNumber("#",0);this.assertEquals("0",a);a=gadgets.i18n.formatNumber("000",0);this.assertEquals("000",a)};NumberFormatTest.prototype.testExponential=function(){var a;a=gadgets.i18n.formatNumber("0.####E0",0.01234);this.assertEquals("1.234E-2",a);a=gadgets.i18n.formatNumber("00.000E00",0.01234);this.assertEquals("12.340E-03",a);a=gadgets.i18n.formatNumber("##0.######E000",0.01234);this.assertEquals("12.34E-003",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",0.01234);this.assertEquals("1.234E-2",a);a=gadgets.i18n.formatNumber("0.####E0",123456789);this.assertEquals("1.2346E8",a);a=gadgets.i18n.formatNumber("00.000E00",123456789);this.assertEquals("12.346E07",a);a=gadgets.i18n.formatNumber("##0.######E000",123456789);this.assertEquals("123.456789E006",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",123456789);this.assertEquals("1.235E8",a);a=gadgets.i18n.formatNumber("0.####E0",1.23e+300);this.assertEquals("1.23E300",a);a=gadgets.i18n.formatNumber("00.000E00",1.23e+300);this.assertEquals("12.300E299",a);a=gadgets.i18n.formatNumber("##0.######E000",1.23e+300);this.assertEquals("1.23E300",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",1.23e+300);this.assertEquals("1.23E300",a);a=gadgets.i18n.formatNumber("0.####E0",-3.141592653e-271);this.assertEquals("-3.1416E-271",a);a=gadgets.i18n.formatNumber("00.000E00",-3.141592653e-271);this.assertEquals("-31.416E-272",a);a=gadgets.i18n.formatNumber("##0.######E000",-3.141592653e-271);this.assertEquals("-314.159265E-273",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",-3.141592653e-271);this.assertEquals("[3.142E-271]",a);a=gadgets.i18n.formatNumber("0.####E0",0);this.assertEquals("0E0",a);a=gadgets.i18n.formatNumber("00.000E00",0);this.assertEquals("00.000E00",a);a=gadgets.i18n.formatNumber("##0.######E000",0);this.assertEquals("0E000",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",0);this.assertEquals("0E0",a);a=gadgets.i18n.formatNumber("0.####E0",-1);this.assertEquals("-1E0",a);a=gadgets.i18n.formatNumber("00.000E00",-1);this.assertEquals("-10.000E-01",a);a=gadgets.i18n.formatNumber("##0.######E000",-1);this.assertEquals("-1E000",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",-1);this.assertEquals("[1E0]",a);a=gadgets.i18n.formatNumber("0.####E0",1);this.assertEquals("1E0",a);a=gadgets.i18n.formatNumber("00.000E00",1);this.assertEquals("10.000E-01",a);a=gadgets.i18n.formatNumber("##0.######E000",1);this.assertEquals("1E000",a);a=gadgets.i18n.formatNumber("0.###E0;[0.###E0]",1);this.assertEquals("1E0",a);a=gadgets.i18n.formatNumber("#E0",12345);this.assertEquals("1E4",a);a=gadgets.i18n.formatNumber("0E0",12345);this.assertEquals("1E4",a);a=gadgets.i18n.formatNumber("##0.###E0",12345);this.assertEquals("12.345E3",a);a=gadgets.i18n.formatNumber("##0.###E0",12345.00001);this.assertEquals("12.345E3",a);a=gadgets.i18n.formatNumber("##0.###E0",12345);this.assertEquals("12.345E3",a);a=gadgets.i18n.formatNumber("##0.####E0",7.8912345e-7);this.assertEquals("789.1235E-9",a);a=gadgets.i18n.formatNumber("##0.####E0",7.8e-7);this.assertEquals("780E-9",a);a=gadgets.i18n.formatNumber(".###E0",45678);this.assertEquals(".457E5",a);a=gadgets.i18n.formatNumber(".###E0",0);this.assertEquals(".0E0",a);a=gadgets.i18n.formatNumber("#E0",45678000);this.assertEquals("5E7",a);a=gadgets.i18n.formatNumber("##E0",45678000);this.assertEquals("46E6",a);a=gadgets.i18n.formatNumber("####E0",45678000);this.assertEquals("4568E4",a);a=gadgets.i18n.formatNumber("0E0",45678000);this.assertEquals("5E7",a);a=gadgets.i18n.formatNumber("00E0",45678000);this.assertEquals("46E6",a);a=gadgets.i18n.formatNumber("000E0",45678000);this.assertEquals("457E5",a);a=gadgets.i18n.formatNumber("###E0",0.0000123);this.assertEquals("12E-6",a);a=gadgets.i18n.formatNumber("###E0",0.000123);this.assertEquals("123E-6",a);a=gadgets.i18n.formatNumber("###E0",0.00123);this.assertEquals("1E-3",a);a=gadgets.i18n.formatNumber("###E0",0.0123);this.assertEquals("12E-3",a);a=gadgets.i18n.formatNumber("###E0",0.123);this.assertEquals("123E-3",a);a=gadgets.i18n.formatNumber("###E0",1.23);this.assertEquals("1E0",a);a=gadgets.i18n.formatNumber("###E0",12.3);this.assertEquals("12E0",a);a=gadgets.i18n.formatNumber("###E0",123);this.assertEquals("123E0",a);a=gadgets.i18n.formatNumber("###E0",1230);this.assertEquals("1E3",a)};NumberFormatTest.prototype.testGroupingParse2=function(){var a;a=gadgets.i18n.parseNumber("#,###","1,234,567,890");this.assertEquals(1234567890,a);a=gadgets.i18n.parseNumber("#,####","12,3456,7890");this.assertEquals(1234567890,a);a=gadgets.i18n.parseNumber("#","1234567890");this.assertEquals(1234567890,a)};NumberFormatTest.prototype.testApis=function(){var a;a=gadgets.i18n.formatNumber("#,###",1234567890);this.assertEquals("1,234,567,890",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",1234.56);this.assertEquals("$1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;(\u00a4#,##0.00)",-1234.56);this.assertEquals("($1,234.56)",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;-\u00a4#,##0.00",1234.56,"SEK");this.assertEquals("kr1,234.56",a);a=gadgets.i18n.formatNumber("\u00a4#,##0.00;(\u00a4#,##0.00)",-1234.56,"SEK");this.assertEquals("(kr1,234.56)",a)};