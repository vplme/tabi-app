using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {

            LoginCommand = new Command(async (object obj) => { });
        }

        public ICommand LoginCommand { protected set; get; }

        public string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

    }
}
