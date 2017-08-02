using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuth.Models;

namespace WebApiAuth.Controllers {
    [Route ("api/[controller]")]
    public class StoryController : Controller {
        private readonly DBContext _db;
        private readonly UserManager<Usuario> _userManager;
        private readonly IRolesService _rolesService;
        public StoryController (DBContext db, UserManager<Usuario> userManager, IRolesService rolesService) {
            this._db = db;
            this._userManager = userManager;
            this._rolesService = rolesService;
        }

        [HttpGet ("ping")]
        public IActionResult GetPing () {
            return Ok (new { message = "Ok" });
        }

        [HttpGet]
        public async Task<IActionResult> Get () {
            try {
                var stories = await this._db.Stories.ToListAsync ();
                // RolesService rolesService = new RolesService (this._userManager, this._db);

                foreach (var story in stories) {
                    var usr = await this._rolesService.GetUser (story.UsuariosId);
                    story.Usuarios = usr;
                }
                return Ok (stories);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpGet ("{id}", Name = "GetStoryById")]
        public async Task<IActionResult> GetStoryById (int id) {
            try {
                var story = await this._db.Stories.Include (x => x.Usuarios).ThenInclude (x => x.Roles).FirstOrDefaultAsync (x => x.Id == id);
                return Ok (story);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostStory ([FromBody] Story model) {
            if (!ModelState.IsValid) {
                return BadRequest (model);
            }
            try {
                model.CreationDate = DateTime.Now;
                await this._db.AddAsync (model);
                await this._db.SaveChangesAsync ();
                return CreatedAtRoute ("GetStoryById", new { id = model.Id }, model);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
                return BadRequest ();
            }
        }

    }
}