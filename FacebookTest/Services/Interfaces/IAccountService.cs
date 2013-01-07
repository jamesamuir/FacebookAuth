using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookTest.Models.DataDocuments;
using FacebookTest.Models.ViewModels;

namespace FacebookTest.Services.Interfaces
{
    public interface IAccountService
    {
        bool IsAvailableUser(string email);
        AccountUserDocument ActivateFacebookAccount(FbModel model);
        AccountUserDocument AddOrUpdateFacebookUser(FbModel model);
        FbModel GetFbModelByAccountHash(string accountHash);
        AccountUserDocument CreateUser(RegisterModel model);
        void DeleteUser(string Id);
        AccountUserDocument GetUser(string email);
        bool ValidateUser(string email, string password);
        bool ValidateUserEmail(string validateEmailCode, string userId);
        void ProcessForgotPassword(ForgotPasswordModel model);
        bool IsValidResetUrl(string documentHash);
        void ResetPassword(ResetPasswordModel model);
        void ChangePassword(string email, string newPassword);

    }
}
