using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;

namespace WishApp.Core.ServicesWithInterface;

/// <summary>
/// Thin abstraction over RoleManager & UserManager
/// </summary>
public class IdentityService : IIdentityService
{
    UserManager<User> _userManager;
    RoleManager<IdentityRole> _roleManager;

    public IdentityService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public Task<IdentityResult> CreateAsync(User user, string password) => _userManager.CreateAsync(user, password);
    public Task<bool> CheckPasswordAsync(User user, string password) => _userManager.CheckPasswordAsync(user, password);
}
