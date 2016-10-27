namespace Auction.DataAccess.Models
{
    public class Category : JsonModel
    {     
        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.json", this.GetType().Name);
        }
    }
}
