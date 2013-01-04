using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FacebookTest.Services.Interfaces;
using FacebookTest.Services.Classes;
using System.Net.Mail;

namespace FacebookTest.Models.ViewModels.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {

        public IAccountService AccountService { get; set; }


        public const int minCharacters = 6;
        
        public RegisterModelValidator()
        {

            //init accountservice
            if (AccountService == null) { AccountService = new AccountService(); }

            //check rules
            RuleFor(m => m.FirstName).NotEmpty().WithMessage("Please enter your first name");
            RuleFor(m => m.LastName).NotEmpty().WithMessage("Please enter your last name");
            RuleFor(m => m.Email).NotEmpty().WithMessage("Please enter your email")
                                 .Must(BeValidemail).WithMessage("Please enter a valid email")
                                 .Must(BeAvailableEmail).WithMessage("Sorry, this email has already been used");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required!")
                .Must(BeValidPasswordLength).WithMessage("Please use at least " + minCharacters + " characters");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required!")
                .Must(BeMatchingPassword).WithMessage("The Password and Confirm Password do not match!");
        }


        private bool BeValidPasswordLength(string value)
        {
            if (value != null && value.Length < minCharacters)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool BeValidemail(string value)
        {
            try
            {
                MailAddress m = new MailAddress(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool BeAvailableEmail(string value)
        {
            if (AccountService.IsAvailableUser(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool BeMatchingPassword(RegisterModel instance, string junk)
        {
            if (instance.Password != null && instance.ConfirmPassword != null && instance.Password != instance.ConfirmPassword)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}