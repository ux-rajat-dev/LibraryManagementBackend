using LibraryManagementSystem.admin.CommandModels;
using LibraryManagementSystem.admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _genreService.GetAllAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();
            return Ok(genre);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] GenreCommandModel model)
        {
            await _genreService.CreateAsync(model);
            return Ok(new { message = "Genre created successfully" });
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] GenreCommandModel model)
        {
            var result = await _genreService.UpdateAsync(model);
            if (!result) return NotFound(new { message = "Genre not found" });
            return Ok(new { message = "Genre updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _genreService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Genre not found" });
            return Ok(new { message = "Genre deleted successfully" });
        }
    }
}
