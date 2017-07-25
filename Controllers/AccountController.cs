using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ApiAuth.Controllers {
    [Route ("api/[controller]")]

    public class AccountController : Controller {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMessageService _messageService;

        public AccountController (
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IMessageService messageService) {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._messageService = messageService;
        }

        [HttpPost]
        [Route ("register")]
        public async Task<JsonResult> Register (string email, string password, string confirmPassword) {
            if (string.IsNullOrWhiteSpace (email) || string.IsNullOrWhiteSpace (password)) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "email or password is null"
                });
            }

            if (password != confirmPassword) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Passwords don't match!"
                });
            }

            var newUser = new IdentityUser {
                UserName = email,
                Email = email
            };

            IdentityResult userCreationResult = null;
            try {
                userCreationResult = await _userManager.CreateAsync (newUser, password);
            } catch (SqlException) {
                return Json (new Response (HttpStatusCode.InternalServerError) {
                    Message = "Error communicating with the database, see logs for more details"
                });
            }

            if (!userCreationResult.Succeeded) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "An error occurred when creating the user, see nested errors",
                        Errors = userCreationResult.Errors.Select (x => new Response (HttpStatusCode.BadRequest) {
                            Message = $"[{x.Code}] {x.Description}"
                        })
                });
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync (newUser);
            var tokenVerificationUrl = Url.Action (
                "VerifyEmail", "Account",
                new {
                    Id = newUser.Id,
                        token = emailConfirmationToken
                },
                Request.Scheme);

            await _messageService.Send (email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");

            return Json (new Response (HttpStatusCode.OK) {
                Message = $"Registration completed, please verify your email - {email}"
            });
        }

        public async Task<IActionResult> VerifyEmail (string id, string token) {
            var user = await _userManager.FindByIdAsync (id);
            if (user == null)
                throw new InvalidOperationException ();

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync (user, token);
            if (!emailConfirmationResult.Succeeded) {
                return new RedirectResult (" http://localhost:5000/registration.html");
            }

            return new RedirectResult (" http://localhost:5000/");
        }

        [HttpPost]
        [Route ("login")]
        public async Task<JsonResult> Login (string email, string password) {
            if (string.IsNullOrWhiteSpace (email) || string.IsNullOrWhiteSpace (password)) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "email or password is null"

                });
            }

            var user = await _userManager.FindByEmailAsync (email);
            if (user == null) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Invalid Login and/or password"
                });
            }

            if (!user.EmailConfirmed) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Email not confirmed, please check your email for confirmation link"
                });
            }

            var passwordSignInResult = await _signInManager.PasswordSignInAsync (user, password, isPersistent : true, lockoutOnFailure : false);
            if (!passwordSignInResult.Succeeded) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Invalid Login and/or password"
                });
            }

            Response response = new Response (HttpStatusCode.OK) { Message = "Cookie created" };
            return Json (response);
        }

        [HttpPost]
        [Route ("logout")]
        public async Task<JsonResult> Logout () {
            await _signInManager.SignOutAsync ();

            return Json (new Response (HttpStatusCode.OK) {
                Message = "You have been successfully logged out"
            });
        }

    }
}