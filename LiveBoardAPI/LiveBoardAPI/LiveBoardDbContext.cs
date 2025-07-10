using LiveBoardAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiveBoardAPI
{
    public class LiveBoardDbContext : IdentityDbContext<Models.User>
    {

        public LiveBoardDbContext(DbContextOptions<LiveBoardDbContext> options) : base(options)
        {
        }
        public DbSet<TokenInfo>TokenInfos { get; set; }
    }
}
