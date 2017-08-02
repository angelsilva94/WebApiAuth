using System;
using System.Collections.Generic;

namespace WebApiAuth.Models {
    public class Story {
        public string Owner { set; get; }
        public int Id { set; get; }
        public DateTime CreationDate { set; get; }
        public string UsuariosId { set; get; }
        public virtual Usuario Usuarios { set; get; }
    }
}