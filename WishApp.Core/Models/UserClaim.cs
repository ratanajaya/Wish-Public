using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;

namespace WishApp.Core.Models;

public record UserClaim
{
    public required string Id { get; set; }
    public required string UserName { get; set; }

    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PictureUrl { get; set; }
    public UserType UserType { get; set; }
}
