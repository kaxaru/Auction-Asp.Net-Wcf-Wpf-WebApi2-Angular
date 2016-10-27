using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPF.Helpers;
using WPF.Models;

namespace WPF.Service
{
    public class UserService : IUserService
    {
        public async Task<User> Authorize(string login, string password)
        {
            var paswordHash = PasswordHelper.Hash(password);
            var user = new LoginModel()
            {
                UserName = login,
                Password = password
            };

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                var result = await client.PostAsync("login/logIn", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));               
                if (!result.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseCookies = cookies.GetCookies(new Uri(string.Format("{0}/login/logIn", Properties.Settings.Default.BaseUrl))).Cast<Cookie>();

                var userCookie = responseCookies.LastOrDefault().Value;
                if (userCookie == string.Empty)
                {
                    return null;
                }

                userCookie = WebUtility.UrlDecode(userCookie);

                var userModel = JsonConvert.DeserializeObject<User>(userCookie);
                userModel.Login = login;
                userModel.Password = paswordHash;

                return userModel;
            }
        }
        
        public async Task<bool> LogOut()
        {
            return await Task<bool>.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                    var result = await client.PostAsync("login/logOut", new StringContent(string.Empty));
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            });
        }
    }
}
