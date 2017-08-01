using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApiAuth.Models;

namespace WebApiAuth.Controllers {
    [Route ("api/[controller]")]
    public class RolesController : Controller {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController (UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager) {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpPost ("addUserToRole")]
        public async Task<IActionResult> Post (string username, string role) {
            try {
                IdentityResult result = null;
                var user = await this._userManager.FindByEmailAsync (username);
                result = await this._userManager.AddToRoleAsync (user, role.ToUpper ());
                if (!result.Succeeded) {
                    return BadRequest ();
                }
                var aux = await this._userManager.FindByEmailAsync (username);
                return Ok (aux);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpPost ("addRole")]
        public async Task<IActionResult> Post (string roleName) {
            try {
                var rol = new IdentityRole ();
                rol.Name = roleName.ToUpper ();
                await this._roleManager.CreateAsync (rol);
                return Ok (new { message = $"rol: {roleName}  successfully created" });
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet]
        public IActionResult ping () {
            return Ok (new { message = "Everything is OK" });
        }
    }
}