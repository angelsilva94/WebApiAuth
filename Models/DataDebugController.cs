using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApiAuth.Models {
    [Route ("api/[controller]")]
    public class DataDebugController : Controller {
        private readonly DBContext _db;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRolesService _rolesService;

        public DataDebugController (DBContext db, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IRolesService rolesService) {
            this._db = db;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._rolesService = rolesService;

        }

        [HttpGet ("getUsers")]
        public async Task<IActionResult> GetUsers () {
            try {
                var users = await this._rolesService.GetListUser ();

                if (users.Count < 1) {
                    return NoContent ();
                }
                return Ok (users);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet ("GetUserById/{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById (string id) {
            try {
                // var rw = await this._userManager.GetRolesAsync (await this._userManager.FindByIdAsync (id));
                // var role = await this._db.Users.Where (x => x.Id == id).Select (
                //     x => new {
                //         Roles = rw,
                //             x.BirthDate,
                //             x.Email,
                //             x.Id,
                //             x.LastName,
                //             x.UserName,
                //     }
                // ).FirstOrDefaultAsync ();

                // var role = await this._db.Users.Where (x => x.Id == id).Include (x => x.Roles).FirstOrDefaultAsync ();
                // var user = await this._db.Users.Include (x => x.Roles).Where (x => x.Id == id).FirstOrDefaultAsync ();
                // var userRoleId = this._db.UserRoles.Select (x => x.UserId);
                // var role = this._db.Users.Where (x => userRoleId.Contains (x.Id));

                var user = await this._userManager.FindByIdAsync (id);
                var roles = await this._userManager.GetRolesAsync (user);
                foreach (var role in roles) {
                    user.Roles.Add (new IdentityUserRole<string> { RoleId = role });

                }
                if (roles == null) {
                    return NotFound ();
                }
                return Ok (user);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet ("GetRoles")]
        public async Task<IActionResult> GetRoles () {
            try {
                //"EL CHIDO"
                var role = await this._roleManager.Roles.Select (x => x.Name).ToListAsync ();
                // var role = await this._db.Roles.Select (x => x.Name).ToListAsync ();

                if (role.Count < 1) {
                    return NoContent ();
                }
                return Ok (role);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet ("GetRoleById/{id}", Name = "GetRoleById")]
        public async Task<IActionResult> GetRoleById (string id) {
            try {
                var role = await this._db.Roles.Where (x => x.Id == id).Include (x => x.Users).FirstOrDefaultAsync ();
                if (role == null) {
                    return NotFound (id);
                }
                return Ok (role);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet ("GetUserRoleById/{id}")]
        public async Task<IActionResult> GetUserRoleById (string id) {
            try {
                var userRole = await this._db.UserRoles.Where (x => x.UserId == id).FirstOrDefaultAsync ();
                return Ok (userRole);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

    }
}