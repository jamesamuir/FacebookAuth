using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FacebookTest.Models;
using Facebook;
using System.Configuration;
using FacebookTest.Classes;

namespace FacebookTest.Controllers
{
    public class AccountController : Controller
    {



       

        public ActionResult LogOn()
        {
            return View();
        }





        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }


        #region Facebook





        public ActionResult SignUpFacebook()
        {
            // Build the Return URI form the Request Url
            var redirectUri = new UriBuilder(Request.Url);
            redirectUri.Path = Url.Action("FbAuth", "Account");

            var client = new FacebookClient();




            // Generate the Facebook OAuth URL
            // Example: https://www.facebook.com/dialog/oauth?
            //                client_id=YOUR_APP_ID
            //               &redirect_uri=YOUR_REDIRECT_URI
            //               &scope=COMMA_SEPARATED_LIST_OF_PERMISSION_NAMES
            //               &state=SOME_ARBITRARY_BUT_UNIQUE_STRING
            var uri = client.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FacebookAppId"],
                redirect_uri = redirectUri.Uri.AbsoluteUri,
                scope = "email",
            });

            return Redirect(uri.ToString());
        }


        public ActionResult LogOnFacebook()
        {
            // Build the Return URI form the Request Url
            var redirectUri = new UriBuilder(Request.Url);
            redirectUri.Path = Url.Action("FbAuth", "Account");

            //Get the Public Uri due to apphabor getting all "cloudy" with ports
            var urlHelper = new UrlHelper(Request.RequestContext);
            var publicUrl = urlHelper.ToPublicUrl(redirectUri.Uri);
            

            


            var client = new FacebookClient();




            // Generate the Facebook OAuth URL
            // Example: https://www.facebook.com/dialog/oauth?
            //                client_id=YOUR_APP_ID
            //               &redirect_uri=YOUR_REDIRECT_URI
            //               &scope=COMMA_SEPARATED_LIST_OF_PERMISSION_NAMES
            //               &state=SOME_ARBITRARY_BUT_UNIQUE_STRING
            var uri = client.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FacebookAppId"],
                redirect_uri = publicUrl,
                scope = "email",
            });

            return Redirect(uri.ToString());
        }

        public ActionResult FbAuth(string returnUrl)
        {

            
                var client = new FacebookClient();
                try
                {
                    var oauthResult = client.ParseOAuthCallbackUrl(Request.Url);


                    // Build the Return URI form the Request Url
                    var redirectUri = new UriBuilder(Request.Url);
                    redirectUri.Path = Url.Action("FbAuth", "Account");

                    //Get the Public Uri due to apphabor getting all "cloudy" with ports
                    var urlHelper = new UrlHelper(Request.RequestContext);
                    var publicUrl = urlHelper.ToPublicUrl(redirectUri.Uri);






                // Exchange the code for an access token
                dynamic result = client.Get("/oauth/access_token", new
                {
                    client_id = ConfigurationManager.AppSettings["FacebookAppId"],
                    redirect_uri = publicUrl,
                    client_secret = ConfigurationManager.AppSettings["FacebookAppSecret"],
                    code = oauthResult.Code,
                });

                // Read the auth values
                string accessToken = result.access_token;
                DateTime expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(result.expires));

                // Get the user's profile information
                dynamic me = client.Get("/me",
                              new
                              {
                                  fields = "first_name,last_name,email",
                                  access_token = accessToken
                              });

                // Read the Facebook user values
                long facebookId = Convert.ToInt64(me.id);
                string firstName = me.first_name;
                string lastName = me.last_name;
                string email = me.email;

                //// Add the user to our persistent store
                //var userService = new UserService();
                //userService.AddOrUpdateUser(new User
                //{
                //    Id = facebookId,
                //    FirstName = firstName,
                //    LastName = lastName,
                //    Email = email,
                //    AccessToken = accessToken,
                //    Expires = expires
                //});

                // Set the Auth Cookie
                FormsAuthentication.SetAuthCookie(email, false);

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }


                // Redirect to the return url if availible
                if (String.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                    //return Redirect(returnUrl);
                }
            
        }

        #endregion
    }
}
