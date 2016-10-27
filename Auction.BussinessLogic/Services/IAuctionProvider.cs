using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.BussinessLogic.Services
{
    public interface IAuctionProvider
    {
        void Create(Models.Auction auction);

        DataAccess.Models.Auction GetAuction();
    }
}
