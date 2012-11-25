
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookTest.Classes
{
    public static class ExtensionMethods
   { 

        public static string ToPublicUrl(this UrlHelper urlHelper, Uri relativeUri)
        {
            var httpContext = urlHelper.RequestContext.HttpContext;

            var uriBuilder = new UriBuilder
            {
                Host = httpContext.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = "http",
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }

    }
}