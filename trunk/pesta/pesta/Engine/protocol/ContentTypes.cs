using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta.Engine.protocol
{
    public class ContentTypes
    {
        [Flags]
        public enum AllowedContentTypes
        {
            JSON = 1,
            XML = 2,
            ATOM = 4,
            MULTIPART = 8,
            // the rest are combinations of the above
            REST = 7,
            RPC = 9,
            ALL = 15
        }

        public static readonly HashSet<String> ALLOWED_JSON_CONTENT_TYPES = new HashSet<string>{"application/json", "text/x-json", "application/javascript",
              "application/x-javascript", "text/javascript", "text/ecmascript"};

        public static readonly HashSet<String> ALLOWED_XML_CONTENT_TYPES =
                new HashSet<string>{"text/xml", "application/xml"};

        public static readonly HashSet<String> ALLOWED_ATOM_CONTENT_TYPES =
          new HashSet<string>{"application/atom+xml"};

        public static readonly HashSet<String> FORBIDDEN_CONTENT_TYPES =
            new HashSet<string>{
              "application/x-www-form-urlencoded" // Not allowed because of OAuth body signing issues
            };

        public const String MULTIPART_FORM_CONTENT_TYPE = "multipart/form-data";

        public static readonly HashSet<String> ALLOWED_MULTIPART_CONTENT_TYPES =
          new HashSet<string>{MULTIPART_FORM_CONTENT_TYPE};

        public const String OUTPUT_JSON_CONTENT_TYPE = "application/json";
        public const String OUTPUT_XML_CONTENT_TYPE = "application/xml";
        public const String OUTPUT_ATOM_CONTENT_TYPE = "application/atom+xml";
        public const String OUTPUT_TEXT_CONTENT_TYPE = "text/html";

        /**
        * Extract the mime part from an Http Content-Type header
        */
        public static String extractMimePart(String contentType) 
        {
            contentType = contentType.Trim();
            int separator = contentType.IndexOf(';');
            if (separator != -1) 
            {
                contentType = contentType.Substring(0, separator);
            }
            return contentType.Trim().ToLower();
        }

        public static IEnumerable<String> getContentTypes(AllowedContentTypes types)
        {
            HashSet<String> total = new HashSet<string>();
            if ((types & AllowedContentTypes.XML) != 0)
            {
                total.UnionWith(ALLOWED_XML_CONTENT_TYPES);
            }
            if ((types & AllowedContentTypes.ATOM) != 0)
            {
                total.UnionWith(ALLOWED_ATOM_CONTENT_TYPES);
            }
            if ((types & AllowedContentTypes.JSON) != 0)
            {
                total.UnionWith(ALLOWED_JSON_CONTENT_TYPES);
            }
            if ((types & AllowedContentTypes.MULTIPART) != 0)
            {
                total.UnionWith(ALLOWED_MULTIPART_CONTENT_TYPES);
            }
            return total;
        }
  public static void checkContentTypes(IEnumerable<String> allowedContentTypes,
      String contentType, bool disallowUnknownContentTypes)
  {

    if (string.IsNullOrEmpty(contentType)) {
       if (disallowUnknownContentTypes) {
        throw new InvalidContentTypeException(
            "No Content-Type specified. One of "
                + string.Join( ", ", allowedContentTypes.ToArray()) + " is required");
       } else {
         // No content type specified, we can fail in other ways later.
         return;
       }
    }

    contentType = ContentTypes.extractMimePart(contentType);

    if (ContentTypes.FORBIDDEN_CONTENT_TYPES.Contains(contentType)) {
      throw new InvalidContentTypeException(
          "Cannot use disallowed Content-Type " + contentType);
    }
    if (allowedContentTypes.Contains(contentType)) {
      return;
    }
    if (disallowUnknownContentTypes) {
      throw new InvalidContentTypeException(
          "Unsupported Content-Type "
              + contentType
              + ". One of "
              + string.Join(", ", allowedContentTypes.ToArray())
              + " is required"); 
    } else {
      
    }
  }

  public class InvalidContentTypeException : Exception {
    public InvalidContentTypeException(String message)
    :base(message){
      
    }
  }
    }
}
