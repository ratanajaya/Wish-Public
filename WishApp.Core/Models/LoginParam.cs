using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public record LoginParam
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
