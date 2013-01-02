using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacebookTest.Services.Interfaces;
using Raven.Client;
using FacebookTest.Raven;
using Raven.Bundles.Authentication;
using FacebookTest.Raven;
using FacebookTest.Models;
using Raven.Client.Linq;


namespace FacebookTest.Services.Classes
{
    public class AccountService : IAccountService
    {

        public bool IsAvailableUser(string email)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                return Session.Query<AccountUserDocument>().Customize(x => x.WaitForNonStaleResults()).Where(x => x.Name == email).SingleOrDefault() == null;
            }
        }

        public AccountUserDocument AddOrUpdateFacebookUser(long facebookId, string firstName, string lastName, string email, string accessToken, DateTime expires)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {

                //Get the user by their facebook Id
                var user = Session.Query<AccountUserDocument, AccountUser_ByFacebookId>().Where(x => x.FacebookId == facebookId).SingleOrDefault();
                


                if (user != null)
                {
                    //User exists, update it
                    user.Name = email;
                    user.AllowedDatabases = new[] { "*" };
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.AccessToken = accessToken;
                    user.FacebookId = facebookId;
                    user.Expires = expires;


                    //Save Changes
                    Session.SaveChanges();
                    return user;
                }
                else
                {
                    //No user, create a new one
                    Guid userId = Guid.NewGuid();
                    Session.Store(new AccountUserDocument
                    {
                        
                        Name = email,
                        Id = String.Format("FacebookTest/Users/{0}", userId.ToString()),
                        AllowedDatabases = new[] { "*" },
                        FirstName = firstName,
                        LastName = lastName,
                        AccessToken = accessToken,
                        FacebookId = facebookId,
                        Expires = expires
                    });

                    //Save Changes
                    Session.SaveChanges();
                    return Session.Load<AccountUserDocument>(String.Format("FacebookTest/Users/{0}", userId));
                    
                }

                

                
            }

        }

        public AccountUserDocument CreateUser(RegisterModel model)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                Guid userId = Guid.NewGuid();

                Session.Store(new AccountUserDocument
                {
                    Name = model.Email,
                    Id = String.Format("FacebookTest/Users/{0}", userId),
                    AllowedDatabases = new[] { "*" },

                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FacebookId = 0,
                    Expires = DateTime.Now.AddMonths(1),
                    AccessToken = string.Empty

                }.SetPassword(model.Password));
                Session.SaveChanges();

                return Session.Load<AccountUserDocument>(String.Format("FacebookTest/Users/{0}", userId));
            }

        }

        public void DeleteUser(string Id)
        {
            throw new NotImplementedException();
        }

        public AccountUserDocument GetUser(string email)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                var user = Session.Query<AccountUserDocument>().Where(x => x.Email == email).SingleOrDefault();
                if (user != null)
                {
                    return user;
                }
                else
                {
                    throw new Exception("User with email " + email + " does not exist in the system");
                }
            }
        }

        

        public bool ValidateUser(string email, string password)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                RavenQueryStatistics stats;
                var user = Session.Query<AccountUserDocument>().Statistics(out stats).Where(x => x.Name == email).SingleOrDefault();

                if (stats.IsStale)
                {
                    Console.Write("stale");
                }


                if (user == null)
                {
                    //User not in database
                    return false;
                }
                else
                {
                    if (user.ValidatePassword(password))
                    {
                        //User validated
                        return true;
                    }
                    else
                    {
                        //User password incorrect
                        return false;
                    }
                }
            }
        }

        public void ChangePassword(string id, string newPassword)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                Session.Load<AccountUserDocument>(String.Format("FacebookTest/Users/{0}", id)).SetPassword(newPassword);
            }
        }



        
    }
}