using WishApp.Core.Models;

namespace WishApp.Core.ServicesWithInterface;
public interface IHttpContextService
{
    UserClaim GetCurrentUser();
}