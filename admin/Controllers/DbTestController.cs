using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("[controller]")]
public class DbTestController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DbTestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("test")]
    public IActionResult TestConnection()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connectionString);
            conn.Open(); // Try to open the connection
            return Ok("✅ Connection succeeded!");
        }
        catch (Exception ex)
        {
            return BadRequest($"❌ Connection failed: {ex.Message}");
        }
    }
}
