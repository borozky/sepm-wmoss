using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMoSS.Entities 
{
    public class ApplicationUser 
    {
        public int Id {get; set;}
        public string FullName {get;set;}
        public string Email {get;set;}
        public string PasswordEncrypted {get;set;}
        public string Role {get;set;}
    }
}