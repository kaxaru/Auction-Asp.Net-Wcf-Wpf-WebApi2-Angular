using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace Auction.Presentation.Infrastructure.Filters
{
    public class LangFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            object lang;
            if (filterContext.RouteData.Values.TryGetValue("lang", out lang))
            {
                var langName = (string)lang;

                try
                {
                    var culture = new CultureInfo(langName);
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                catch (Exception)
                {
                    // TODO: LOG
                }
            }
        }
    }
}