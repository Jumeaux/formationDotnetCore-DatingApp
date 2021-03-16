using System.ComponentModel.DataAnnotations.Schema;
using Entites;

namespace API.Entites
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool isMain { get;set; }
        
        public string PublicId { get; set; }

        public AppUser AppUser { get; set; }
        
        public int AppUserId { get; set; }

        public bool isApproved {get; set;}
    }
}