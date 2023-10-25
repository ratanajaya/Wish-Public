using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishApp.Core.Models;
using WishApp.Core.Services;

namespace WishApp.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MasterController : ControllerBase
{
    MasterDataService _masterData;

    public MasterController(MasterDataService masterData) {
        _masterData = masterData;
    }

    [HttpGet("GetWish")]
    public IActionResult GetWish(string id) {
        return Ok(_masterData.GetWish(id));
    }

    [HttpGet("GetWishes")]
    public IActionResult GetWishes() {
        return Ok(_masterData.GetWishes());
    }

    [HttpPost("InsertWish")]
    public async Task<IActionResult> InsertWish(WishCrudModel param) {
        return Ok(await _masterData.InsertWish(param));
    }

    [HttpPost("UpdateWish")]
    public async Task<IActionResult> UpdateWish(WishCrudModel param) {
        return Ok(await _masterData.UpdateWish(param));
    }

    [HttpDelete("DeleteWish")]
    public async Task<IActionResult> DeleteWish(string id) {
        return Ok(await _masterData.DeleteWish(id));
    }

    [HttpPost("CreateSampleWishes")]
    public async Task<IActionResult> CreateSampleWishes() {
        return Ok(await _masterData.CreateSampleWishes());
    }
}
