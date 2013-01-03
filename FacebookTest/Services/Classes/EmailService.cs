using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacebookTest.Services.Interfaces;

namespace FacebookTest.Services.Classes
{
    public static class EmailService
    {

        public static void SendMail(dynamic EmailProperties)
        {
            

        }
    }

    public enum EmailType
    {
        ForgotPassword = 1,
        Register = 2
    }
}