using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UserService.Model
{
    [DataContract]
    public class JsonModel 
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime UpdateOn
        {
            get { return DateTime.Now; }

            set { value = UpdateOn; }
        }
    }
}