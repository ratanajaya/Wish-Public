using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public record AppSetting
{
    public required string JwtIssuerSigningKey { get; set; }
    public required string GoogleSsoClientSecret { get; set; }
    public required string GoogleSsoClientId { get; set; }
}
