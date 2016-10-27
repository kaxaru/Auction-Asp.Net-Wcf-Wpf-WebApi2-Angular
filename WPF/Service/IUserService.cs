using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.Service
{
    public interface IUserService
    {
        Task<User> Authorize(string login, string password);

        Task<bool> LogOut();
    }
}
