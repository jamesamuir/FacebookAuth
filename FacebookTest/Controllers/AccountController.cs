using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FacebookTest.Models.ViewModels;
using Facebook;
using System.Configuration;
using FacebookTest.Classes;
using FacebookTest.Services.Classes;
using FacebookTest.Services.Interfaces;
using FacebookTest.Security;
using System.Web.Script.Serialization;

namespace FacebookTest.Controllers
{
    public class AccountController : Controller
    {


        
        public IAccountService AccountService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (AccountService == null) { AccountService = new AccountService(); }
            base.Initialize(requestContext);
        }


        #region Login
        // GET: /Account/Register
        public ActionResult LogIn()
        {
            return View();
        }


        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult LogIn(LogInModel model)
        {
            if (AccountService.ValidateUser(model.Email, model.Password))
            {
                //Get the user
                var user = AccountService.GetUser(model.Email);

                //Call the authenticate 
                AuthenticateUser(user.Id, user.FirstName, user.LastName, user.Email, user.FacebookId, user.AccessToken);

                //Redirect to profile info
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("*", "Sorry, the email or password is incorrect. Please try again.");
                return View(model);
            }
        }

        #endregion

        public ActionResult SignUp()
        {
            return View();
        }


        #region Register
            // GET: /Account/Register
            public ActionResult Register()
            {
                return View();
            }

            //
            // POST: /Account/Register
            [HttpPost]
            public ActionResult Register(RegisterModel model)
            {
                if (ModelState.IsValid  && AccountService.IsAvailableUser(model.Email))
                {
                    //Create user
                    var user = AccountService.CreateUser(model);

                    //Call the authenticate 
                    AuthenticateUser(user.Id, user.FirstName, user.LastName, user.Email, user.FacebookId, user.AccessToken);

                    //Redirect to profile info
                    return RedirectToAction("Index", "Home");

                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
        #endregion

        
        #region Facebook


            public ActionResult SignUpFacebook()
            {
                // Build the Return URI form the Request Url
                var redirectUri = new UriBuilder(Request.Url);
                redirectUri.Path = Url.Action("FbAuth", "Account");

                //Get the Public Uri due to apphabor getting all "cloudy" with ports
                var urlHelper = new UrlHelper(Request.RequestContext);
                var publicUrl = urlHelper.ToPublicUrl(redirectUri.Uri);


                var client = new FacebookClient();


                #region Facebook OAuth URL example
                // Generate the Facebook OAuth URL
                // Example: https://www.facebook.com/dialog/oauth?
                //                client_id=YOUR_APP_ID
                //               &redirect_uri=YOUR_REDIRECT_URI
                //               &scope=COMMA_SEPARATED_LIST_OF_PERMISSION_NAMES
                //               &state=SOME_ARBITRARY_BUT_UNIQUE_STRING
                #endregion

                //Create the Facebook Oauth URL
                var uri = client.GetLoginUrl(new
                {
                    client_id = ConfigurationManager.AppSettings["FacebookAppId"],
                    redirect_uri = publicUrl,
                    scope = "email",
                });

                return Redirect(uri.ToString());
            }


            public ActionResult LogInFacebook()
            {
                // Build the Return URI form the Request Url
                var redirectUri = new UriBuilder(Request.Url);
                redirectUri.Path = Url.Action("FbAuth", "Account");

                //Get the Public Uri due to apphabor getting all "cloudy" with ports
                var urlHelper = new UrlHelper(Request.RequestContext);
                var publicUrl = urlHelper.ToPublicUrl(redirectUri.Uri);
            

                var client = new FacebookClient();



                #region Facebook OAuth URL example
                // Generate the Facebook OAuth URL
                // Example: https://www.facebook.com/dialog/oauth?
                //               client_id=YOUR_APP_ID
                //               &redirect_uri=YOUR_REDIRECT_URI
                //               &scope=COMMA_SEPARATED_LIST_OF_PERMISSION_NAMES
                //               &state=SOME_ARBITRARY_BUT_UNIQUE_STRING

                #endregion

                //Create the Facebook Oauth URL
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
                        AccountService.AddOrUpdateFacebookUser(facebookId, firstName, lastName, email, accessToken, expires);

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

        #region ForgotPassword
            // GET: /Account/ForgotPassword
            public ActionResult ForgotPassword()
            {
                ForgotPasswordModel model = new ForgotPasswordModel();
                return View(model);
            }
            //
            // POST: /Account/Register
            [HttpPost]
            public ActionResult ForgotPassword(ForgotPasswordModel model)
            {

                //Check valid email, process password reset
                if (ModelState.IsValid)
                {


                    return RedirectToAction("Content", "EmailSent", new { });
                }

                return View(model);
            }
        #endregion
        
        #region Logoff
            // GET: /Account/Register
            public ActionResult Logoff()
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home", new { });
            }
        #endregion

        #region Authenticate User
        private void AuthenticateUser(string userId, string firstName, string lastName, string email, long facebookId, string accessToken)
        {

            AccountPrincipalSerializeModel serializeModel = new AccountPrincipalSerializeModel();
            serializeModel.UserId = userId;
            serializeModel.FirstName = firstName;
            serializeModel.LastName = lastName;
            serializeModel.Email = email;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(serializeModel);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        email,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(15),
                        false,
                        userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }
        #endregion
    }
}
