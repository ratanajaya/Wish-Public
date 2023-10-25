using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;
using WishApp.Data.Utils;

namespace WishApp.Data.Models.Master;

public class Wish
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }

    public ICollection<SubWish> SubWishes { get; set; } = new List<SubWish>();
    public ICollection<WishOption> WishOptions { get; set; } = new List<WishOption>();

    public AppEntityState EntityState { get; set; }
}    

