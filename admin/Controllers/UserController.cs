using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.Auth;
using LibraryManagementSystem.AuthModels;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var result = await _service.RegisterAsync(model);
        if (!result) return BadRequest("User already exists.");
        return Ok("Registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var token = await _service.LoginAsync(model);
        if (token == null) return Unauthorized("Invalid credentials");
        return Ok(new { Token = token });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        if (email == null) return Unauthorized();

        var profile = await _service.GetProfileAsync(email);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(UserCommandModel model)
    {
        var result = await _service.CreateAsync(model);
        return result ? Ok("User created") : BadRequest("Email already exists");
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UserCommandModel model)
    {
        var result = await _service.UpdateAsync(id, model);
        return result ? Ok("User updated") : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? Ok("User deleted") : NotFound();
    }
}
