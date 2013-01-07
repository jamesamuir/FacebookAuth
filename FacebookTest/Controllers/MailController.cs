using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using System.Configuration;
using FacebookTest.Models.ViewModels;

namespace FacebookTest.Controllers
{
    public class MailController : MailerBase
    {
        

        public EmailResult VerificationEmail(EmailModel emailModel)
        {
            To.Add(emailModel.ToAddress);
            From = ConfigurationManager.AppSettings["AdminEmail"].ToString();
            Subject = "Please confirm your email";
            return Email("VerificationEmail", emailModel);
        }

        public EmailResult ForgotPasswordEmail(EmailModel emailModel)
        {
            To.Add(emailModel.ToAddress);
            From = ConfigurationManager.AppSettings["AdminEmail"].ToString();
            Subject = "Reset your password";
            return Email("ForgotPasswordEmail", emailModel);
        }

    }
}
