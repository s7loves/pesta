using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Pesta
{
    public class RenderingResults
    {
        private readonly Status status;
        private readonly String content;
        private readonly String errorMessage;
        private readonly Uri redirect;

        private RenderingResults(Status status, String content, String errorMessage, Uri redirect) 
        {
            this.status = status;
            this.content = content;
            this.errorMessage = errorMessage;
            this.redirect = redirect;
        }

        public static RenderingResults ok(String content) 
        {
            return new RenderingResults(Status.OK, content, null, null);
        }

        public static RenderingResults error(String errorMessage)
        {
            return new RenderingResults(Status.ERROR, null, errorMessage, null);
        }

        public static RenderingResults mustRedirect(Uri redirect)
        {
            return new RenderingResults(Status.MUST_REDIRECT, null, null, redirect);
        }

        /**
        * @return The status of the rendering operation.
        */
        public Status getStatus() 
        {
            return status;
        }

        /**
        * @return The content to render. Only available when status is OK.
        */
        public String getContent()
        {
            Debug.Assert(status == Status.OK, "Only available when status is OK.");
            return content;
        }

        /**
        * @return The error message for rendering. Only available when status is ERROR.
        */
        public String getErrorMessage()
        {
            Debug.Assert(status == Status.ERROR, "Only available when status is ERROR.");
            return errorMessage;
        }

        /**
        * @return The error message for rendering. Only available when status is ERROR.
        */
        public Uri getRedirect()
        {
            Debug.Assert(status == Status.MUST_REDIRECT, "Only available when status is MUST_REDIRECT.");
            return redirect;
        }

        public enum Status 
        {
            OK, MUST_REDIRECT, ERROR
        }
    }
}
