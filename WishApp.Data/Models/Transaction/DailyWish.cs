using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;
using WishApp.Data.Models.Master;

namespace WishApp.Data.Models.Transaction;

public class DailyWish
{
    public required string Id { get; set; }
    public DateTime PkDate { get; set; }

    [ForeignKey("Wish")]
    public required string WishId { get; set; }
    public Wish? Wish { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }

    [ForeignKey("SelectedOption")]
    public required string SelectedOptionId { get; set; }
    public WishOption? SelectedOption { get; set; }

    public string? Remark { get; set; }

    public ICollection<DailySubWish> DailySubWishes { get; set; } = new List<DailySubWish>();
}
