#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public class GoogleIdClaim
{
    // These six fields are included in all Google ID Tokens.
    public string iss { get; set; }
    public string sub { get; set; }
    public string azp { get; set; }
    public string aud { get; set; }
    public string iat { get; set; }
    public string exp { get; set; }

    // These seven fields are only included when the user has granted the "profile" and
    // "email" OAuth scopes to the application.
    public string email { get; set; }
    public string email_verified { get; set; }
    public string name { get; set; }
    public string picture { get; set; }
    public string given_name { get; set; }
    public string family_name { get; set; }
    public string locale { get; set; }
}
