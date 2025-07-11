using LiveBoardAPI.Constants;
using LiveBoardAPI.Controllers;
using LiveBoardAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LiveBoardAPI.Commands.RegisterUserCommand
{
    public class SignupUserHandler : IRequestHandler<SignupUserCommand, IActionResult>
    {
        private readonly UserManager<Models.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public SignupUserHandler(LiveBoardDbContext context, UserManager<Models.User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Handle(SignupUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(request.SignupModel.Email);
                if (existingUser != null)
                {
                    return new BadRequestObjectResult("User already exists");
                }

                if (await _roleManager.RoleExistsAsync(Roles.User) == false)
                {
                    var roleResult = await _roleManager
                          .CreateAsync(new IdentityRole(Roles.User));

                    if (roleResult.Succeeded == false)
                    {
                        var roleErros = roleResult.Errors.Select(e => e.Description);
                        _logger.LogError($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                        return new BadRequestObjectResult($"Failed to create user role. Errors : {string.Join(",", roleErros)}"); // Fix: Use BadRequestObjectResult
                    }
                }

                Models.User user = new()
                {
                    Email = request.SignupModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = request.SignupModel.Email,
                    Name = request.SignupModel.Name,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, request.SignupModel.Password);

                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    _logger.LogError(
                        $"Failed to create user. Errors: {string.Join(", ", errors)}"
                    );
                    return new BadRequestObjectResult($"Failed to create user. Errors: {string.Join(", ", errors)}");
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user: user, role: Roles.User);

                if (addUserToRoleResult.Succeeded == false)
                {
                    var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to add role to the user. Errors : {string.Join(",", errors)}");
                }
                return new OkObjectResult(new { Message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError }; 
            }
        }
    }
}
