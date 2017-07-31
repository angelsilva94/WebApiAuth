using System;

namespace WebApiAuth.Models {
    public class Register {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public DateTime BirthDate { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public string VerifyPwd { set; get; }
    }
}