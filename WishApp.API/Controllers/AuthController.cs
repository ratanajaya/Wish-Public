using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishApp.Core.Models;
using WishApp.Core.Services;

namespace WishApp.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    AuthService _authService;

    public AuthController(AuthService authService) {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginParam param) {
        return Ok(await _authService.Login(param));
    }

    [HttpPost("LoginAsGuest")]
    public async Task<IActionResult> LoginAsGuest() {
        return Ok(await _authService.LoginAGuest());
    }

    [HttpPost("LoginAsGoogleAccount")]
    public async Task<IActionResult> LoginAsGoogleAccount(string idToken, string? guestId) {
        return Ok(await _authService.LoginAsGoogleAccount(idToken, guestId));
    }
}
