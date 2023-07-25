using MediaApi.Models;
using MediaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace MediaApi.Controllers;


[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService) => _userService = userService;
   
    [HttpGet]
    // I want to only return Usernames, not password or email
    public async Task<List<string>> Get(){
        var users = await _userService.GetAsync();
        var userNames = new List<string>{};
        foreach(var user in users)
            userNames.Add(user.Name);
        return userNames;
    }

    [HttpPost("login")]
    public async Task<ActionResult<dynamic>> Authenticate([FromBody] User user)
    {
        var registeredUser = await _userService.PostAsync(user.Email);

        if(registeredUser == null)
            return NotFound("User does not exist");
            
        if (user.Password != registeredUser.Password)
            return NoContent();

        var token = TokenService.GenerateToken(registeredUser);
        return new {user = registeredUser, token = token};
    }

    [HttpPost("register")]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        bool userExists = await _userService.PostAsync(newUser.Email) != null;


        if(userExists)
            return BadRequest("User already exists");

        await _userService.CreateAsync(newUser);

        var token = TokenService.GenerateToken(newUser);      
         
        return CreatedAtAction(nameof(Get), new { user = newUser, token = token });
    }

    
    [HttpPut("alter")]
    public async Task<IActionResult> Update([FromHeader] string token,[FromBody] User updatedUser)
    {
        int id = TokenService.DecodeToken(token);

        if(id != updatedUser.Id)
            return Forbid("Operation is not Valid");

        var user = await _userService.GetAsync(id);

        if (user is null)
            return NotFound();

        await _userService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromHeader] string token, [FromBody] int userId)
    {
        int id = TokenService.DecodeToken(token);

        if(id != userId)
            return BadRequest("Operation is not Valid");

        var user = await _userService.GetAsync(id);

        if (user is null)
            return NotFound();

        await _userService.RemoveAsync(id);

        return NoContent();
    }
}