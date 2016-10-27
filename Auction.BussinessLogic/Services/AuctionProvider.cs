using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Models;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class AuctionProvider : IAuctionProvider
    {
        public AuctionProvider()
        {
        }

        public static Models.Auction _Auction { get; set; }

        public DataAccess.Models.Auction GetAuction()
        {
            var type = (DataAccess.Models.TypeAuction)Enum.Parse(typeof(DataAccess.Models.TypeAuction), _Auction.Type.ToString());

            return new DataAccess.Models.Auction()
            {
                Location = _Auction.Location,
                Name = _Auction.Name,
                Type = type
            };
        }

        public void Create(Models.Auction auction)
        {
            _Auction = auction;
        }
    }
}
