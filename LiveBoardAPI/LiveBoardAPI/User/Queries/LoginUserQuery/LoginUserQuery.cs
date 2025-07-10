using LiveBoardAPI.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.User.Queries.LoginUserQuery
{
    public record LoginUserQuery(LoginModel LoginModel) : IRequest<IActionResult>;
}
