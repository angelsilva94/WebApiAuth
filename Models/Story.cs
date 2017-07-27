using System;

namespace WebApiAuth.Models {
    public class Story {
        public string Owner { set; get; }
        public int Id { set; get; }
        public DateTime CreationDate { set; get; }
    }
}