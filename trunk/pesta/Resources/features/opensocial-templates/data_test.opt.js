function verifyNamespace(A){var B=os.getNamespace(A);
if(!B){B=os.createNamespace(A)
}return B
}function testRequestHandler(){verifyNamespace("test");
var A={};
os.data.registerRequestHandler("test:request",function(D,C){A[D]=C.getAttribute("data")
});
var B='<os:dataSet key="first">  <test:request data="testData"/></os:dataSet><os:dataSet key="second">  <test:request data="${foo}"/></os:dataSet>';
os.data.loadRequests(B);
assertNotNull(os.data.requests_.test);
assertNotNull(os.data.requests_.test["first"]);
assertNotNull(os.data.requests_.test["second"]);
os.data.DataContext.putDataSet("foo","bar");
os.data.executeRequests();
assertEquals("testData",A.first);
assertEquals("bar",A.second)
}function testResultHandler(){var A="test1";
var C="foo";
var B=function(D){D(C)
};
os.data.DataContext.putDataResult(A,B);
assertEquals(C,os.data.DataContext.getDataSet(A))
}function testListener(){var A=false;
os.data.DataContext.registerListener("testKey",function(){A=true
});
os.data.DataContext.putDataSet("testKey",{});
assertEquals(true,A)
};