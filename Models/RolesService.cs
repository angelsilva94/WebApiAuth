using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiAuth.Models {
    public class RolesService : IRolesService {
        private readonly UserManager<Usuario> _userManager;
        private readonly DBContext _db;
        public RolesService (UserManager<Usuario> userManager, DBContext db) {
            this._userManager = userManager;
            this._db = db;
        }
        // public RolesService () {

        // }
        public async Task<Usuario> GetUser (string id) {
            var user = await this._db.Users.FindAsync (id);
            var roles = await this._userManager.GetRolesAsync (user);
            foreach (var role in roles) {
                user.Roles.Add (new IdentityUserRole<string> { RoleId = role });
            }
            return user;

        }
        public async Task<List<Usuario>> GetListUser () {
            var users = await this._db.Users.Include (x => x.UserStory).ToListAsync ();
            foreach (var user in users) {
                var roles = await this._userManager.GetRolesAsync (user);
                foreach (var role in roles) {
                    user.Roles.Add (new IdentityUserRole<string> { RoleId = role });
                }
            }
            return users;
        }

    }
}