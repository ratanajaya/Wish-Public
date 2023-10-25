using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishApp.Core.Services;

namespace WishApp.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UtilityController : ControllerBase
{
    UtilityService _utility;

    public UtilityController(UtilityService utility) {
        _utility = utility;
    }

    [HttpGet("Initialize")]
    public async Task<IActionResult> InitializeData() {
        await _utility.InitializeData();
        return Ok();
    }
}
