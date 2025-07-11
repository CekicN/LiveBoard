using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.Commands.RevokeTokenCommand
{
    public record RevokeTokenCommand() : IRequest<IActionResult>;
}
