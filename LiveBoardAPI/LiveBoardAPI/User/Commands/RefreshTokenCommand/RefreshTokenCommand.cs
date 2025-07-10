using LiveBoardAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.User.Commands.RefreshTokenCommand
{
    public record RefreshTokenCommand(TokenModel TokenModel) : IRequest<IActionResult>;
}
