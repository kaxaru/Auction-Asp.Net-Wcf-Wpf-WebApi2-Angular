using System;
using System.Collections.Generic;
using Auction.BussinessLogic.Models;

namespace Auction.BussinessLogic.Services
{
   public interface IUserService
    {
        IEnumerable<UserDTO> ShowAwalaibleUsers();

        void EditUser(UserDTO user);

        UserDTO GetUser(Guid? userId);

        void AddUser(UserDTO user);

        void RemoveUser(Guid? id);

        bool IsUnigueLogin(string login);
    }
}
