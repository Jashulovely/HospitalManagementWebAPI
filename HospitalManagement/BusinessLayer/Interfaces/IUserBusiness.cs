using ModelLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IUserBusiness
    {
        public UserModel UserRegistration(UserModel user);
        public bool IsRegisteredAlready(string email);
        public string UpdateUser(UserModel user);
        public string DeleteUser(int id);
        public string UserLogin(UserLoginModel login);
        public string ForgetPassword(string EmailId);
        public string ResetNewPassword(string Email, string password, string confirmPassword);
    }
}
