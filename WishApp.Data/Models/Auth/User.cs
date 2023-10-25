using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Data.Models.Auth;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public UserType UserType { get; set; }

    public string? PictureUrl { get; set; }
}

public enum UserType {
    Guest = 0,
    Member = 1,
    Admin = 2
}