using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApiAuth.Models;

namespace ApiAuth.Controllers {
    [Route ("api/[controller]")]

    public class AccountController : Controller {

        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IMessageService _messageService;

        public AccountController (
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IMessageService messageService) {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._messageService = messageService;
        }

        [HttpGet ("ping"), Authorize (Roles = "ADMIN")]
        public IActionResult Ping () {
            return Ok (new { msg = "Everything is Ok" });
        }

        [HttpPost ("register"), AllowAnonymous]
        public async Task<JsonResult> Register ([FromBody] Register model) {
            if (string.IsNullOrWhiteSpace (model.Email) || string.IsNullOrWhiteSpace (model.Password)) {
                return Json (new Response (HttpStatusCode.BadRequest));
            }

            if (model.Password != model.VerifyPwd) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Passwords don't match!"
                });
            }
            if (!ModelState.IsValid) {
                return Json (new Response (HttpStatusCode.BadRequest) { Message = "Error in the model" });
            }

            var user = new Usuario (model.Email) {
                BirthDate = model.BirthDate,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.Password

            };

            var pwd = new PasswordHasher<Usuario> ();
            user.PasswordHash = pwd.HashPassword (user, model.Password);

            IdentityResult userCreationResult = null;
            try {
                userCreationResult = await this._userManager.CreateAsync (user);
                if (userCreationResult.Succeeded) {
                    Console.WriteLine ("SUCCESS");
                }
            } catch (Exception e) {
                return Json (new Response (HttpStatusCode.InternalServerError) {
                    Message = "Error communicating with the database, see logs for more details",
                        Errors = new List<Response> () { new Response (HttpStatusCode.InternalServerError) { Message = e.Message } }

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

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync (user);
            var tokenVerificationUrl = Url.Action (
                "VerifyEmail", "Account",
                new {
                    Id = user.Id,
                        token = emailConfirmationToken
                },
                Request.Scheme);

            await _messageService.Send (model.Email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");

            return Json (new Response (HttpStatusCode.OK) {
                Message = $"Registration completed, please verify your email - {model.Email}"
            });
        }

        public async Task<IActionResult> VerifyEmail (string id, string token) {
            var user = await _userManager.FindByIdAsync (id);
            if (user == null) {
                throw new InvalidOperationException ();
            }
            var emailConfirmationResult = await _userManager.ConfirmEmailAsync (user, token);
            if (!emailConfirmationResult.Succeeded) {
                return new RedirectResult (" http://localhost:5000/registration.html");
            }
            return new RedirectResult (" http://localhost:5000/");
        }

        [HttpPost ("login")]
        public async Task<JsonResult> Login ([FromBody] Login model) {
            if (string.IsNullOrWhiteSpace (model.Username) || string.IsNullOrWhiteSpace (model.Password)) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "email or password is null"

                });
            }

            var user = await _userManager.FindByEmailAsync (model.Username);
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

            var passwordSignInResult = await _signInManager.PasswordSignInAsync (user, model.Password, isPersistent : true, lockoutOnFailure : false);
            if (!passwordSignInResult.Succeeded) {
                return Json (new Response (HttpStatusCode.BadRequest) {
                    Message = "Invalid Login and/or password"
                });
            }

            Response response = new Response (HttpStatusCode.OK) { Message = "Cookie created" };
            return Json (response);
        }

        [HttpPost ("logout")]
        public async Task<JsonResult> Logout () {
            await _signInManager.SignOutAsync ();

            return Json (new Response (HttpStatusCode.OK) {
                Message = "You have been successfully logged out"
            });
        }

    }
}