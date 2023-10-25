using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;
using WishApp.Data;
using System.Runtime.InteropServices;
using WishApp.Core.ServicesWithInterface;

namespace WishApp.Core.Services;

public class UtilityService
{
    AppDbContext _db;
    IIdentityService _identityService;

    public UtilityService(AppDbContext db, IIdentityService identityService) {
        _db = db;
        _identityService = identityService;
    }

    public async Task InitializeData() {
        //var user = new User {
        //    Id = "DR",
        //    UserName = "DR",
        //};
        //await _identityService.CreateAsync(user, password);
    }
}
