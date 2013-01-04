using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookTest.Models.ViewModels
{
    public class EmailModel
    {
        public string ToAddress { get; set; }
        public string FirstName { get; set; }
        public string ReturnUrl { get; set; }
       
    }
}