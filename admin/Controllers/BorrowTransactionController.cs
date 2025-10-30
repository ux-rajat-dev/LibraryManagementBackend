using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.QueryModel;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "admin,user")]
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowTransactionController : ControllerBase
    {
        private readonly IBorrowTransactionService _service;
        private readonly IHubContext<NotificationHub> _hubContext; 

        // Constructor to inject services
        public BorrowTransactionController(IBorrowTransactionService service, IHubContext<NotificationHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;  // Assign the HubContext to a private field
        }

        // Get all borrowing transactions
        [HttpGet]
        public async Task<ActionResult<List<BorrowTransactionQueryModel>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // Get borrowing transaction by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowTransactionQueryModel>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound("Transaction not found.");
            return Ok(result);
        }

        // Borrow a book
        [HttpPost("borrow")]
        public async Task<ActionResult> BorrowBook([FromBody] BorrowTransactionCommandModel model)
        {
            var success = await _service.BorrowAsync(model);
            if (!success) return BadRequest("Borrow failed. Book may be unavailable.");

            // Notify all clients that a book has been borrowed
            await _hubContext.Clients.All.SendAsync("ReceiveBookUpdate", "A book has been borrowed!");

            return Ok("Book borrowed successfully.");
        }

        // Return a book
        [HttpPut("return")]
        public async Task<ActionResult> ReturnBook([FromBody] BorrowTransactionReturnModel model)
        {
            var success = await _service.ReturnAsync(model);
            if (!success) return BadRequest("Return failed. Transaction may not exist or book already returned.");

            // Notify all clients that a book has been returned
            await _hubContext.Clients.All.SendAsync("ReceiveBookUpdate", "A book has been returned!");

            return Ok("Book returned successfully.");
        }
    }
}
