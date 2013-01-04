using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookTest.Models.DataDocuments
{
    public class ReturnEmailDocument
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Identifier { get; set; }
        public string Hash { get; set; }
        public string ResetUrl { get; set; }
        

    }
}