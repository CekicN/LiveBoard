using LiveBoardAPI.Models.DTOs;
using LiveBoardAPI.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.Commands.RefreshTokenCommand
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, IActionResult>
    {
        private readonly LiveBoardDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(LiveBoardDbContext context, ITokenService tokenService, ILogger<RefreshTokenHandler> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.TokenModel.AccessToken);
                var username = principal.Identity.Name;

                var tokenInfo = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (tokenInfo == null
                    || tokenInfo.RefreshToken != request.TokenModel.RefreshToken
                    || tokenInfo.ExpiredAt <= DateTime.UtcNow)
                {
                    return new BadRequestObjectResult("Invalid refresh token. Please login again.");
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken;
                await _context.SaveChangesAsync();

                return new OkObjectResult(new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
