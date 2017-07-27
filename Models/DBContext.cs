using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiAuth.Models {
    public class DBContext : IdentityDbContext<Usuario> {
        public DBContext (DbContextOptions<DBContext> options) : base (options) { }
        public DbSet<Story> Stories { set; get; }

        // public DbSet<Popo> RolesPopo { set; get; }
        protected override void OnModelCreating (ModelBuilder builder) {
            base.OnModelCreating (builder);
        }

    }
}