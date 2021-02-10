using Entites;

namespace API.Entites
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUSerId { get; set; }

        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}