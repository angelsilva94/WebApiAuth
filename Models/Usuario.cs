using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApiAuth.Models {
    public class Usuario : IdentityUser {
        public Usuario (string username) : base (username) { }

        public Usuario () { }
        // public override string UserName { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public DateTime BirthDate { set; get; }

    }
}