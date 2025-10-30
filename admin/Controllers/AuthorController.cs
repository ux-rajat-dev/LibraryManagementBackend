using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AuthorCommandModel model)
        {
            await _authorService.CreateAsync(model);
            return Ok(new { message = "Author created successfully" });
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] AuthorCommandModel model)
        {
            var result = await _authorService.UpdateAsync(model);
            if (!result) return NotFound(new { message = "Author not found" });
            return Ok(new { message = "Author updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Author not found" });
            return Ok(new { message = "Author deleted successfully" });
        }
    }
}
