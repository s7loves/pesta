var JsonMessage=function(a,b){b=b||{};opensocial.Message.call(this,a,b)};JsonMessage.inherits(opensocial.Message);JsonMessage.prototype.toJsonObject=function(){return JsonMessage.copyFields(this.fields_)};JsonMessage.copyFields=function(a){var b={};for(var c in a){b[c]=a[c]}return b};