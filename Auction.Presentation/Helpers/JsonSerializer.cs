using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Auction.Presentation.Helpers
{
    public class JsonSerialize
    {
        public static IHtmlString SerializeObject(object value)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new JsonSerializer
                {                                   
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                jsonWriter.QuoteName = false;
                serializer.Serialize(jsonWriter, value);

                return new HtmlString(stringWriter.ToString());
            }
        }
    }
}