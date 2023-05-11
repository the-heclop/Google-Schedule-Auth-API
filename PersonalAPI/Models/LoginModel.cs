using BCrypt.Net;

namespace PersonalAPI.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string passwordHash { get; set; }
    }


}
