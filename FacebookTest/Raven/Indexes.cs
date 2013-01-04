using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using Raven.Bundles.Authentication;
using FacebookTest.Models.DataDocuments;

namespace FacebookTest.Raven
{
    
        //**************************
        //  USER INDEXES
        //**************************
        public class AuthUser_ByEmail : AbstractIndexCreationTask<AuthenticationUser>
        {
            public AuthUser_ByEmail()
            {
                Map = users => from user in users
                               select new { user.Id, user.Name };

            }
        }

        public class AccountUser_ByFacebookId : AbstractIndexCreationTask<AccountUserDocument>
        {
            public AccountUser_ByFacebookId()
            {
                Map = users => from user in users
                               select new { user.Id, user.FacebookId, user.FirstName, user.LastName, user.Name, user.AccessToken, user.Expires };

            }
        }

     
}