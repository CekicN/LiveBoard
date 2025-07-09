using Microsoft.AspNetCore.Identity;

namespace LiveBoardAPI.Models
{
    public class User:IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
