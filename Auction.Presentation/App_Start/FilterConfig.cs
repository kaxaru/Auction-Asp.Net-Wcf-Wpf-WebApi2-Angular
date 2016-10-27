using System.Web.Mvc;
using Auction.Presentation.Infrastructure.Filters;

namespace Auction.Presentation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LangFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
