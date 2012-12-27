using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookTest.Raven;
using FacebookTest.Models;

namespace FacebookTest.Services.Interfaces
{
    public interface IAccountService
    {
        bool IsAvailableUser(string email);
        AccountUserDocument AddOrUpdateFacebookUser(long facebookId, string firstName, string lastName, string email, string accessToken, DateTime expires);
        AccountUserDocument CreateUser(RegisterModel model);
        void DeleteUser(string Id);
        AccountUserDocument GetUser(string email);
        bool ValidateUser(string email, string password);
        void ChangePassword(string email, string newPassword);

    }
}
