using Microsoft.EntityFrameworkCore;

namespace WebApiEFCore.Models {
    public class DBContext : DbContext {
        public DBContext (DbContextOptions<DBContext> options) : base (options) {

        }
    }
}