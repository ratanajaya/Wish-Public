using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishApp.Core.Models;
using WishApp.Core.Services;

namespace WishApp.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    TransactionDataService _transactionData;

    public TransactionController(TransactionDataService transactionData) {
        _transactionData = transactionData;
    }

    [HttpGet("GetDailyWishesByDate")]
    public IActionResult GetDailyWishesByDate(DateTime? date) {
        return Ok(_transactionData.GetDailyWishesByDate(date));
    }

    [HttpPost("InsertDailyWish")]
    public async Task<IActionResult> InsertDailyWish(DailyWishInsertModel param) {
        return Ok(await _transactionData.InsertDailyWish(param));
    }

    [HttpGet("GetDailyWishSummaries")]
    public IActionResult GetDailyWishSummaries() {
        return Ok(_transactionData.GetDailyWishSummaries());
    }
}
