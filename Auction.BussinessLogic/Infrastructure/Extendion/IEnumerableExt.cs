using System.Collections.Generic;
using System.Linq;

namespace Auction.BussinessLogic.Infrastructure.Extendion
{
   public static class IEnumerableExt
    {    
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
