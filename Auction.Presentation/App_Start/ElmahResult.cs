using System.Web;
using System.Web.Mvc;

namespace Auction.Presentation.App_Start
{
    public class ElmahResult : ActionResult
    {
        private string _resouceType;

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();

            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "." + _resouceType;
                HttpContext.Current.RewritePath(PathForStylesheet(), pathInfo, HttpContext.Current.Request.QueryString.ToString());
            }

            var httpHandler = factory.GetHandler(HttpContext.Current, null, null, null);
            httpHandler.ProcessRequest(HttpContext.Current);
        }

        private string PathForStylesheet()
        {
            return _resouceType != "stylesheet" ? HttpContext.Current.Request.Path.Replace(string.Format("/{0}", _resouceType), string.Empty) : HttpContext.Current.Request.Path;
        }
    }
}