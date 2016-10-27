using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPF.Service
{
    public class ServiceBase
    {
        public async Task<IEnumerable<T>> ApiGetAsync<T>(string controller, string[] actions, Dictionary<string, string> parameters = null)
        {
            using (var client = new HttpClient())
            {
                StringBuilder actionsSb = new StringBuilder();
                foreach (var action in actions.Where(a => !string.IsNullOrEmpty(a)))
                {
                    actionsSb.Append(action + '/');
                }

                StringBuilder sb = new StringBuilder();
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        sb.Append($"{param.Key}={param.Value}&");  
                    }
                }

                client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                var url = $"{controller}/{actionsSb}?{sb.ToString()}";
                var result = client.GetAsync(url).Result;
                var content = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
            }
        }      
    }
}
