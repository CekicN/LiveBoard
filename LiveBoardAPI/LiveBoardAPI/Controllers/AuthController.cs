using LiveBoardAPI.Commands.LoginUserQuery;
using LiveBoardAPI.Commands.RefreshTokenCommand;
using LiveBoardAPI.Commands.RegisterUserCommand;
using LiveBoardAPI.Commands.RevokeTokenCommand;
using LiveBoardAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            return await _mediator.Send(new SignupUserCommand(model));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return await _mediator.Send(new LoginUserCommand(model));
        }

        [HttpPost("token/refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            return await _mediator.Send(new RefreshTokenCommand(tokenModel));
        }

        [HttpPost("token/revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            return await _mediator.Send(new RevokeTokenCommand());
        }
    }
}
