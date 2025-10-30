using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateCommandModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _bookService.CreateBookAsync(model);
            if (success)
                return Ok(new { message = "Book created successfully!" });

            return StatusCode(500, "Something went wrong");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
                return NotFound(new { message = "Book not found" });

            return Ok(book);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateBook([FromBody] BookUpdateCommandModel model)
        {
            var success = await _bookService.UpdateBookAsync(model);

            if (!success)
                return NotFound(new { message = "Book not found" });

            return Ok(new { message = "Book updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);

            if (!result)
                return NotFound(new { message = "Book not found" });

            return Ok(new { message = "Book deleted successfully" });
        }


    }


}
