using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public record DailyWishInsertModel
{
    public DateTime PkDate { get; set; }

    public required string WishId { get; set; }
    public required string OptionId { get; set; }
    public string? Remark { get; set; }

    public required List<SubWishOption> SubWishOptions { get; set; }
}

public record SubWishOption
{
    public required string SubWishId { get; set; }
    public required string OptionId { get; set; }
    public string? Remark { get; set; }
}