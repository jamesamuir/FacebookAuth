using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Bundles.Authentication;

namespace FacebookTest.Models.DataDocuments
{
    public class AccountUserDocument : AuthenticationUser
    {

        public string FacebookEmail { get; set; }
        public long FacebookId { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AccountHash { get; set; }
        
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}