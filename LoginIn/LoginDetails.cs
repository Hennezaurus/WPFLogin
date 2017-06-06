using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginIn
{
    class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }


        public LoginDetails(string user, string pass)
        {
            Username = user;
            Password = pass;
        }
    }
}
