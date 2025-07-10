using LiveBoardAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.User.Commands.RegisterUserCommand
{
    public record SignupUserCommand(SignupModel SignupModel) : IRequest<IActionResult>;
}
