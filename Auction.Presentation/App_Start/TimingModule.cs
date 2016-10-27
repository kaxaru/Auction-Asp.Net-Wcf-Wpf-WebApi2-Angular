using System;
using System.Diagnostics;
using System.Web;

namespace Auction.Presentation.App_Start
{
    public class TimingModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        public void OnBeginRequest(object sender, System.EventArgs e)
        {
            if (HttpContext.Current.Request.IsLocal)
            {
                var stopwatch = new Stopwatch();
                HttpContext.Current.Items["Stopwatch"] = stopwatch;
                stopwatch.Start();
            }
        }

        public void OnEndRequest(object sender, System.EventArgs e)
        {
            if (HttpContext.Current.Request.IsLocal)
            {
                Stopwatch stopwatch =
                  (Stopwatch)HttpContext.Current.Items["Stopwatch"];
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                string elapsedTime = string.Format("{0}ms", ts.TotalMilliseconds);
                HttpContext.Current.Response.Write("<p>" + elapsedTime + "</p>");
            }
        }
    }
}