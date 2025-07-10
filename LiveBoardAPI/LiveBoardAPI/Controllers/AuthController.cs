using LiveBoardAPI.Constants;
using LiveBoardAPI.Models;
using LiveBoardAPI.Models.DTOs;
using LiveBoardAPI.Services;
using LiveBoardAPI.User.Commands.RefreshTokenCommand;
using LiveBoardAPI.User.Commands.RegisterUserCommand;
using LiveBoardAPI.User.Commands.RevokeTokenCommand;
using LiveBoardAPI.User.Queries.LoginUserQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            return await _mediator.Send(new LoginUserQuery(model));
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
