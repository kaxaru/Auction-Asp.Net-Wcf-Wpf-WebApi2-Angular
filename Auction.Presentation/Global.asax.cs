using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure.СustomSettings;

namespace Auction.Presentation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;        
        private string _dataFolder = HostingEnvironment.MapPath("~/App_Data");

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);            
            RouteConfig.RegisterRoutes(RouteTable.Routes);          
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ////RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes); //debugger for routing , webconfig =>RouteDebugger value="true"
            ////IocConfigurator.ConfigureIocUnityContainer();
            SemanticUIStart.PostStart();       
                 
            ////DataInit();
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Request.IsLocal)
            {
                ////fix bug with timer when page loading           
                var stopwatch = new Stopwatch();
                HttpContext.Current.Items["Stopwatch"] = stopwatch;
                stopwatch.Start();
            }
        }

        protected void Application_EndRequest()
        {            
            if (Context.Response.StatusCode >= 400)
            {
                ////creating customize errors for different situations
                Response.Clear();
                Server.ClearError();
                var route = new RouteData();
                IController errormanagerController = new Auction.Presentation.Controllers.ErrorsController();
                HttpContextWrapper wrapper = new HttpContextWrapper(Context);
                route.Values["controller"] = "Errors";
                if (Context.Response.StatusCode == 404)
                {
                    route.Values["action"] = "NotFound";
                }          
                else if (Context.Response.StatusCode == 401)
                {
                    route.Values["action"] = "Error401";
                }
                else if (Context.Response.StatusCode == 400)
                {
                    route.Values["action"] = "NotFound";
                }

                var rc = new RequestContext(wrapper, route);
                errormanagerController.Execute(rc);
            }

            if (HttpContext.Current.Request.IsLocal)
            {
                Stopwatch stopwatch =
                  (Stopwatch)HttpContext.Current.Items["Stopwatch"];
                stopwatch.Stop();

                TimeSpan ts = stopwatch.Elapsed;

                ////HttpContext.Current.Response.SetCookie(new HttpCookie("Stopwatch", Convert.ToString(ts.Seconds)));
            }
        }

        private void DataInit()
        {
            ////initialize data for all auctions
            TypeAuction typeAuction;
            List<Auction.BussinessLogic.Models.Auction> auctions = new List<Auction.BussinessLogic.Models.Auction>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                try
                {
                    typeAuction = (TypeAuction)Enum.Parse(typeof(TypeAuction), auction.Type);
                }
                catch
                {
                    throw new Exception("type not valid");
                }

                auctions.Add(new Auction.BussinessLogic.Models.Auction() { Name = auction.Name, Location = auction.Location, Type = typeAuction });
            }   
                                    
                    DataInitializerService dataInit = new DataInitializerService(_dataFolder, auctions);
                    dataInit.Initialize();               
            }
        }
}
