using Application.Common.Interfaces.Publisher;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetUser;
using Application.Features.Common.Models;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Commands.DeleteUser;
using Application.Features.Users.Commands.UpdateUser;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetUserById;
using Application.Features.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly IMessageBus _messageBus;

    public UsersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    // GET /api/users?role=Admin&page=1&pageSize=10&search=john
    [HttpGet]
    //[Authorize(Roles = "Admin")] // Only admin can list users
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryDtoRequest request)
    {
        var getUsersQuery = request.ToGetUsersQuery();
        var users = await _messageBus.SendAsync<GetUsersQuery, PagedResult<UserDtoResponse>>(getUsersQuery);
        return Ok(users);
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    //[Authorize(Roles = "Admin")] // Admin or self-check can be implemented
    public async Task<IActionResult> GetUserById(string id)
    {
        var command = new GetUserByIdQuery(id);
        var user = await _messageBus.SendAsync< GetUserByIdQuery, UserDtoResponse>(command);
        return Ok(user);
    }

    // POST /api/users
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateUserDtoResponse>> CreateUser(CreateUserDtoRequest request)
    {
        var command = request.ToCreateUserCommand();
        var createdUser = await _messageBus.SendAsync<CreateUserCommand, CreateUserDtoResponse>(command);
        return Ok(createdUser);
    }

    // PUT /api/users/{id}
    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDtoRequest request)
    {
        var command = request.ToUpdateUserCommand(id);
        await _messageBus.SendAsync<UpdateUserCommand>(command);
        return NoContent();
    }

    // DELETE /api/users/{id} (soft delete)
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand(id);
        await _messageBus.SendAsync<DeleteUserCommand>(command);
        return NoContent();
    }

    //[HttpPost("import")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> ImportUsers([FromForm] IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("CSV file is required");

    //    //var command = new ImportUsersCommand { CsvFile = file };
    //    //var results = await _mediator.Send(command);

    //    //return Ok(results);
    //    throw new NotImplementedException();
    //}
}
