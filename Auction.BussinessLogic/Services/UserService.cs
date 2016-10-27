using System;
using System.Collections.Generic;
using System.Linq;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Models;
using Auction.DataAccess.Repositories;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void AddUser(UserDTO userDTO)
        {
            _userRepository.Configure();
            if (userDTO == null)
            {
                throw new ArgumentException(nameof(userDTO));
            }

            var user = new User() { };
            user.InjectFrom(userDTO);

            _userRepository.AddUser(user);
        }

        public void EditUser(UserDTO userDTO)
        {
            _userRepository.Configure();
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO));
            }

            var user = new User() { };
            user.InjectFrom(userDTO);

            _userRepository.UpdateUser(user);
        }

        public UserDTO GetUser(Guid? userId)
        {
            _userRepository.Configure();
            var showingUser = _userRepository.GetUsers().FirstOrDefault(u => u.Id == userId);

            if (showingUser == null)
            {
                throw new ArgumentException("There is no categories with specific id", nameof(userId));
            }

            var userDTO = new UserDTO() { };
            userDTO.InjectFrom(showingUser);

            return userDTO;
        }

        public bool IsUnigueLogin(string login)
        {
            return ShowAwalaibleUsers().FirstOrDefault(u => u.Login == login) == null ? true : false;
        }

        public void RemoveUser(Guid? userId)
        {
            _userRepository.Configure();
            var userDTO = this.GetUser(userId);
            var user = new User() { };
            user.InjectFrom(userDTO);
            _userRepository.RemoveUser(user);
        }

        public IEnumerable<UserDTO> ShowAwalaibleUsers()
        {
            _userRepository.Configure();
            var allUsers = _userRepository.GetUsers().ToList();
            return allUsers.Select(u => Mapper.Map<UserDTO>(u));
        }
    }
}
