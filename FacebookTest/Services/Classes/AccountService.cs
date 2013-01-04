using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacebookTest.Services.Interfaces;
using Raven.Client;
using FacebookTest.Raven;
using Raven.Bundles.Authentication;
using FacebookTest.Models.DataDocuments;
using FacebookTest.Models.ViewModels;
using Raven.Client.Linq;
using System.Dynamic;
using System.Configuration;
using FacebookTest.Controllers;


namespace FacebookTest.Services.Classes
{
    public class AccountService : IAccountService
    {

        public bool IsAvailableUser(string email)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                return Session.Query<AccountUserDocument>().Where(x => x.Name == email).SingleOrDefault() == null;
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

                //Persist the new user
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

                //Retrieve user from the document session
                var user = Session.Load<AccountUserDocument>(String.Format("FacebookTest/Users/{0}", userId));


                //Send Validation Email
                //Create return email object
                BCryptService crypto = new BCryptService();
                var identifier = crypto.GenerateToken();
                var returnEmailDocument = new ReturnEmailDocument
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Identifier = identifier,
                    Hash = crypto.Hash(identifier)

                };

                //Creste reset Url
                returnEmailDocument.ResetUrl = ConfigurationManager.AppSettings["BaseUrl"] + "Account/Verify?vac=" + System.Uri.EscapeDataString(returnEmailDocument.Hash);

                //Persist reset object
                Session.Store(returnEmailDocument);
                Session.SaveChanges();



                //Send the email
                if (user != null)
                {
                    EmailModel emailProperties = new EmailModel();
                    emailProperties.ToAddress = user.Email;
                    emailProperties.FirstName = user.FirstName;
                    emailProperties.ReturnUrl = returnEmailDocument.ResetUrl;
                    new MailController().VerificationEmail(emailProperties).Deliver();

                }
                else
                {
                    throw new Exception("User not found by specified email address");
                }

                



                return user;
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

                var user = Session.Query<AccountUserDocument>().Where(x => x.Name == email).SingleOrDefault();


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

        public bool ValidateUserEmail(string validateEmailCode, string userId)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                //Find matching return email document
                var returnEmailDocument = Session.Query<ReturnEmailDocument>().Where(x => x.Hash == System.Uri.UnescapeDataString(validateEmailCode)).Where(x => x.UserId == userId).SingleOrDefault();

                if (returnEmailDocument != null)
                {
                    //Get user based on id
                    var user = Session.Load<AccountUserDocument>(returnEmailDocument.UserId);

                    //Set the flag
                    user.EmailVerified = true;

                    //Delete the returnEmailDocument
                    Session.Advanced.DocumentStore.DatabaseCommands.Delete(returnEmailDocument.Id, null);

                    Session.SaveChanges();

                    //Good to go
                    return true;
                }
                else
                {
                    //Couldnt find the document
                    return false;
                }


            }
        }

        public void ProcessForgotPassword(string email)
        {
            using (IDocumentSession Session = DataDocumentStore.Instance.OpenSession())
            {
                //Get user info
                var user = Session.Query<AccountUserDocument>().Where(x => x.Name == email).SingleOrDefault();

                //Create return email object
                BCryptService crypto = new BCryptService();
                var identifier = crypto.GenerateToken();
                var resetDocument = new ReturnEmailDocument
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Identifier = identifier,
                    Hash = crypto.Hash(identifier)

                };

                //Creste reset Url
                resetDocument.ResetUrl = ConfigurationManager.AppSettings["BaseUrl"] + "Account/ResetPassword?prc=" + System.Uri.EscapeDataString(resetDocument.Hash);

                //Persist reset object
                Session.Store(resetDocument);
                Session.SaveChanges();
                
                

                //Send the email
                if (user != null)
                {
                    EmailModel emailProperties = new EmailModel();
                    emailProperties.ToAddress = user.Email;
                    emailProperties.FirstName = user.FirstName;
                    emailProperties.ReturnUrl = resetDocument.ResetUrl;
                    new MailController().VerificationEmail(emailProperties).Deliver();
                    
                }
                else
                {
                    throw new Exception("User not found by specified email address");
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