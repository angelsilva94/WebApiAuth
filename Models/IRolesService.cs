using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiAuth.Models {
    public interface IRolesService {
        Task<Usuario> GetUser (string id);
        Task<List<Usuario>> GetListUser ();

    }
}