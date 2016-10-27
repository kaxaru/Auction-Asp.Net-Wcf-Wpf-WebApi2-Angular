using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UserService.Model
{
    [DataContract]
    public class Permission : JsonModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public int Role { get; set; }

        [DataMember]
        public string AuctionId { get; set; }

        [DataMember]
        public List<Guid> CategoriesId { get; set; }
    }
}