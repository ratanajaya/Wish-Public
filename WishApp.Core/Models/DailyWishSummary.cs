using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public class DailyWishSummary
{
    public required string WishId { get; set; }
    public required string Name { get; set; }
    public int Count { get; set; }
    public double Sum { get; set; }
    public double Mean { get; set; }

    public required List<RemarkSummary> Remarks { get; set; }
    public required List<DailySubWishSummary> DailySubWishes { get; set; }
    public required List<WishOptionCrudModel> WishOptions { get; set; }
}

public class DailySubWishSummary 
{
    public required string SubWishId { get; set; }
    public required string Name { get; set; }
    public int Count { get; set; }
    public double Sum { get; set; }
    public double Mean { get; set; }

    //public required List<RemarkSummary> Remarks { get; set; }
}

public class RemarkSummary 
{
    public required DateTime PkDate { get; set; }
    public required string Remark { get; set; }
}