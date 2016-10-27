using System;
using System.Text;
using System.Web.Mvc;
using Auction.Presentation.Models;

namespace Auction.Presentation.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
                                              PageInfo pageInfo, 
                                              Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pageInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pageInfo.PageNumber)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("ui button green");
                }

                tag.AddCssClass("ui button");
                result.Append(tag.ToString());
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}