using MediaApi.Models;
using MediaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediaApi.Controllers;

[ApiController]
[Route("api/Comments")]
public class CommentsController : ControllerBase
{
    private readonly CommentsService _commentsService;

    public CommentsController(CommentsService commentsService) => _commentsService = commentsService;

    [HttpGet]
    public async Task<List<Comments>> Get() => await _commentsService.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Comments>> Get(int id)
    {
        var comment = await _commentsService.GetAsync(id);

        return (comment is null) ? NotFound() : comment;
    }

    [HttpGet("user/{email}")]
    public async Task<ActionResult<List<Comments>>> Get(string email)
    {
        var comment = await _commentsService.GetAsync(email);

        return (comment is null) ? NotFound() : comment;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromHeader] string token, [FromBody] Comments newComment)
    {
        int userId = TokenService.DecodeToken(token);

        // If the user doesn't have a token (is not logged) can't post
        if(userId == -1)
            return BadRequest("Operation is not Valid");

        newComment.userId = userId;
        await _commentsService.CreateAsync(newComment);
        return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromHeader] string token, int id, Comments updatedComment)
    {
        int userId = TokenService.DecodeToken(token);

        if(userId == -1)
            return BadRequest("Operation is not Valid");

        var comment = await _commentsService.GetAsync(id);

        if (comment is null)
            return NotFound();

         if(userId != comment.userId)
            return BadRequest("You are not the comment creator");

        updatedComment.Id = comment.Id;
        updatedComment.alteredDate = DateTime.UtcNow;
        updatedComment.userId = userId;
        
        await _commentsService.UpdateAsync(id, updatedComment);

        return Ok("Alteration Made");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromHeader] string token, int id)
    {
        int userId = TokenService.DecodeToken(token);

        if(userId == -1)
            return BadRequest("Operation is not Valid");

        var comments = await _commentsService.GetAsync(id);

        if (comments is null)
            return NotFound();

        if(userId != comments.userId)
            return BadRequest("You are not the comment creator");

        await _commentsService.RemoveAsync(id);

        return Ok("Message Deleted");
    }
}