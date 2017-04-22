using System;

namespace Tabi.DataObjects
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        //public UserType Type { get; set; }

    }

    //public enum UserType : int { Regular = 1, Developer = 2 }
}
