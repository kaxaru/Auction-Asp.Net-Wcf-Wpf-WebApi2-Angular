using System.Windows.Controls;
using DevExpress.Mvvm.DataAnnotations;
using WPF.Models;
using WPF.Service;

namespace WPF.ViewModels
{
    public enum CurrentViewEnum
    {
        TryLoginView = 1,
        ProductsView = 2,
        WrongEnter = 3
    }

    [POCOViewModel]
    public class AuthViewModel
    {
        private IUserService _userService;

        public AuthViewModel()
        {                   
            CurrentView = CurrentViewEnum.TryLoginView;
        }

        public virtual CurrentViewEnum CurrentView { get; set; }

        public virtual User CurrentUser { get; set; }

        public virtual string Login { get; set; }

        public async void Authorize(PasswordBox args)
        {
            _userService = new UserService();
            CurrentUser = await _userService.Authorize(Login, args.Password);
            if (CurrentUser != null)
            {
                CurrentView = CurrentViewEnum.ProductsView;
            }
            else
            {
                CurrentView = CurrentViewEnum.WrongEnter;
            }
        }

        public async void Logout()
        {
            CurrentView = CurrentViewEnum.TryLoginView;
            var check = await _userService.LogOut();
            if (check)
            {
                CurrentUser = null;
            }
        }
    }
}
