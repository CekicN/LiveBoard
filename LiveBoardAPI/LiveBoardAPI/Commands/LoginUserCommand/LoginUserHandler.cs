using LiveBoardAPI.Models.DTOs;
using LiveBoardAPI.Models;
using LiveBoardAPI.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace LiveBoardAPI.Commands.LoginUserQuery
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, IActionResult>
    {
        private readonly LiveBoardDbContext _context;
        private readonly UserManager<Models.User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginUserHandler> _logger;

        public LoginUserHandler(UserManager<Models.User> userManager, ITokenService tokenService, LiveBoardDbContext context, ILogger<LoginUserHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.LoginModel.Username);
                if (user == null)
                {
                    return new BadRequestObjectResult("User with this username is not registered with us.");
                }
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, request.LoginModel.Password);
                if (isValidPassword == false)
                {
                    return new UnauthorizedObjectResult("Pasword is incorrect");
                }

                List<Claim> authClaims = [
                        new (ClaimTypes.Name, user.UserName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims);

                string refreshToken = _tokenService.GenerateRefreshToken();

                var tokenInfo = _context.TokenInfos.
                            FirstOrDefault(a => a.Username == user.UserName);

                if (tokenInfo == null)
                {
                    var ti = new TokenInfo
                    {
                        Username = user.UserName,
                        RefreshToken = refreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7)
                    };
                    _context.TokenInfos.Add(ti);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
                }

                await _context.SaveChangesAsync();

                return new OkObjectResult(new TokenModel
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
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
