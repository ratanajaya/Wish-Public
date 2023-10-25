using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WishApp.Core.Models;
using WishApp.Core.Utils;
using WishApp.Data.Models.Auth;

namespace WishApp.Core.ServicesWithInterface;

public class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserClaim GetCurrentUser() {
        if(_httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated) {
            var claims = _httpContextAccessor.HttpContext.User.Claims;

            return new UserClaim {
                Id = claims.First(a => a.Type == AppConst.Claim.UserId).Value,
                UserName = claims.First(a => a.Type == AppConst.Claim.UserName).Value,
                UserType = (UserType)int.Parse(claims.First(a => a.Type == AppConst.Claim.UserType).Value),
                Email = claims.First(a => a.Type == AppConst.Claim.Email).Value,
            };
        }

        throw new Exception("GetUserClaim | Attempted to access user claims while unauthenticated");
    }
}
