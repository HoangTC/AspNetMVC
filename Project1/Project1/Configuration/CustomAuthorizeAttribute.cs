using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Configuration
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        public CustomAuthorizeAttribute()
        {
            //dsgfdsf
            var so = 1;
            so++;
        }
    }
}