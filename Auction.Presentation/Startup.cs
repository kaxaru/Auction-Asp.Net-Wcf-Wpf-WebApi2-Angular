using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Auction.Presentation.Startup))]

namespace Auction.Presentation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}