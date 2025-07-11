using LiveBoardAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.Commands.LoginUserQuery
{
    public record LoginUserCommand(LoginModel LoginModel) : IRequest<IActionResult>;
}
