using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public record LoginResult
{
    public required string Token { get; set; }
    public required UserClaim User { get; set; }
    public required bool IsNew { get; set; }
}
