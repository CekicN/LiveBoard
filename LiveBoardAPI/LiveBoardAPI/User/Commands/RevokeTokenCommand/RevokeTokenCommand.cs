using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.User.Commands.RevokeTokenCommand
{
    public record RevokeTokenCommand() : IRequest<IActionResult>;
}
