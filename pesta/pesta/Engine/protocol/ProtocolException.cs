using System;
using System.Net;
using Pesta.Engine.social;
using Pesta.Utilities;

namespace Pesta.Engine.protocol
{
    public class ProtocolException : Exception
    {
        private readonly int errorCode;

        /**
        * The applicatin specific response value associated with this exception.
        */
        private readonly Object response;

        public ProtocolException(int errorCode, String errorMessage, Exception cause)
                : base(errorMessage, cause)
        {
            checkErrorCode(errorCode);
            this.errorCode = errorCode;
            this.response = null;
        }

        public ProtocolException(int errorCode, String errorMessage)
                : this(errorCode, errorMessage, null)
        {

        }

        public ProtocolException(ResponseError respError, String errorMessage)
            : this(respError.Key, errorMessage, null)
        {
        }

        public ProtocolException(int errorCode, String errorMessage, Object response)
                : base(errorMessage)
        {
            checkErrorCode(errorCode);
            this.errorCode = errorCode;
            this.response = response;
        }

        public int getCode() 
        {
            return errorCode;
        }

        public Object getResponse() 
        {
            return response;
        }

        private void checkErrorCode(int code) 
        {
            // 200 is not a legit use of ProtocolExceptions.
            Preconditions.checkArgument(code != (int)HttpStatusCode.OK,
            "May not use OK error code with ProtocolException");
        }
    }
}
