using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Utils;

namespace WishApp.Core.Models;

public record WishCrudModel
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public List<SubWishCrudModel> SubWishes { get; set; } = new List<SubWishCrudModel>();
    public List<WishOptionCrudModel> WishOptions { get; set; } = new List<WishOptionCrudModel>();
    public AppEntityState EntityState { get; set; }
}

public record SubWishCrudModel
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public AppEntityState EntityState { get; set; }
}

public record WishOptionCrudModel
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public double Value { get; set; }
}
