using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBaseApp.Helpers
{
    public static class SecurityHelper
    {
        public static void LogError(Exception ex, string msg)
        {
            // log error to Elmah
            if (msg != null)
            {
                // log exception with contextual information that's visible when 
                // clicking on the error in the Elmah log
                var annotatedException = new Exception(msg, ex);
                ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
            }
            else
            {
                ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
            }
        }
    }
}