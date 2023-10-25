using Microsoft.AspNetCore.Identity;
using WishApp.Data.Models.Auth;

namespace WishApp.Core.ServicesWithInterface;
public interface IIdentityService
{
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<IdentityResult> CreateAsync(User user, string password);
}