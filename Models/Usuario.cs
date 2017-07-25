using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApiAuth.Models {
    public class Usuario : IdentityUser<string> {
        public Usuario (string username) : base (username) {

        }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public DateTime BirthDate { set; get; }

    }
}