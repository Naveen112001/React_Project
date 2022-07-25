namespace api_backend.Models
{
    public class DisplayBook
    {
        public int BookId { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; }
        public int count { get; set; }
    }
    public class Login
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
