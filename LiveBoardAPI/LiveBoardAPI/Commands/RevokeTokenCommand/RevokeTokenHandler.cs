using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiveBoardAPI.Commands.RevokeTokenCommand
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, IActionResult>
    {
        private readonly LiveBoardDbContext _context;
        private readonly ILogger<RevokeTokenHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RevokeTokenHandler(
            LiveBoardDbContext context,
            ILogger<RevokeTokenHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

                if (string.IsNullOrEmpty(username))
                {
                    return new BadRequestObjectResult("User is not authenticated");
                }

                var user = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return new BadRequestObjectResult("User does not exist");
                }

                user.RefreshToken = string.Empty;
                await _context.SaveChangesAsync();

                return new OkObjectResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
