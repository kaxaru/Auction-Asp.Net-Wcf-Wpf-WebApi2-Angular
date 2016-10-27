using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.ViewModels
{
    public class ProductViewModel 
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public TimeSpan Duration { get; set; }

        public State State { get; set; }

        public int StartPrice { get; set; }

        public Guid CategoryID { get; set; }

        public string Picture { get; set; }

        public Guid UserId { get; set; }
    }
}
