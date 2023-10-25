using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WishApp.Data.Models.Master;

namespace WishApp.Data.Models.Transaction;

public class DailySubWish
{
    public required string Id { get; set; }

    public required string DailyWishId { get; set; }
    [JsonIgnore]
    public DailyWish? DailyWish { get; set; }

    public required string SubWishId { get; set; }
    public SubWish? SubWish { get; set; }

    [ForeignKey("SelectedOption")]
    public required string SelectedOptionId { get; set; }
    public WishOption? SelectedOption { get; set; }

    public string? Remark { get; set; }
}
