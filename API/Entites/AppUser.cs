using System.IO.Pipes;
using System.Xml.Schema;
using System;
using API.Entites;
using System.Collections.Generic;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Entites
{
    public class AppUser:IdentityUser<int>
    {
         public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public DateTime LastActive { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        public ICollection<Photo> Photos { get; set; }

        public ICollection<UserLike> LikedByUsers {get; set;}
        public ICollection<UserLike> LikedUsers { get; set;}

        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageReceived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
  
    }

}