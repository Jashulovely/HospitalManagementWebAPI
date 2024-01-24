using BusinessLayer.Interfaces;
using ModelLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepo userRepo;

        public UserBusiness(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public UserModel UserRegistration(UserModel user)
        {
            return this.userRepo.UserRegistration(user);
        }

        public bool IsRegisteredAlready(string email)
        {
            return this.userRepo.IsRegisteredAlready(email);
        }

        public string UpdateUser(UserModel user)
        {
            return userRepo.UpdateUser(user);
        }

        public string DeleteUser(int id)
        {
            return this.userRepo.DeleteUser(id);
        }

        public string UserLogin(UserLoginModel login)
        {
            return this.userRepo.UserLogin(login);
        }

        public string ForgetPassword(string EmailId)
        {
            return this.userRepo.ForgetPassword(EmailId);
        }

        public string ResetNewPassword(string Email, string password, string confirmPassword)
        {
            return this.userRepo.ResetNewPassword(Email, password, confirmPassword);
        }
    }
}
