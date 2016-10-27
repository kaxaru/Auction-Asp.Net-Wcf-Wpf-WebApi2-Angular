using System.Web.Http;
using Auction.Presentation;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Auction.Presentation
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "Auction.Presentation");
                    })
                .EnableSwaggerUi(c =>
                    {                      
                    });
        }
    }
}
