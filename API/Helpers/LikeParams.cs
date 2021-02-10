namespace API.Helpers
{
    public class LikeParams:PaginationParams
    {
        public int userId { get; set; }
        public string predicat { get; set; }
    }
}