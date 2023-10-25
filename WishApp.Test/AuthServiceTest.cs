#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Core.Models;
using WishApp.Core.Services;
using WishApp.Core.ServicesWithInterface;
using WishApp.Data;

namespace WishApp.Test;

[TestClass]
public class AuthServiceTest
{
    Mock<IStaticProvider> _staticMock;
    Mock<IIdentityService> _identityMock;
    Mock<IHttpContextService> _httpContextServiceMock;

    AuthService _auth;
    AppDbContext _db;

    [TestInitialize]
    public void Initialize() {
        _db = Factory.GetInMemoryDbContext();
        _staticMock = Factory.GetStaticProviderMock();
        _httpContextServiceMock = Factory.GetHttpContextServiceMock();
        _identityMock = Factory.GetIdentityServiceMock();

        var httpClientServiceMock = Factory.GetHttpClientServiceMock();

        var masterDataService = new MasterDataService(_db, _httpContextServiceMock.Object, _staticMock.Object);

        _auth = new AuthService(_identityMock.Object, _db, _staticMock.Object, masterDataService, Factory.AppSetting, httpClientServiceMock.Object);
    }

    [TestMethod]
    public async Task Login() {
        var res1 = await _auth.Login(new() {
            UserName = "DR",
            Password = "[Password Not Tested]"
        });

        Assert.IsTrue(res1.Success);
    }

    [TestMethod]
    public async Task LoginAGuest() {
        var res1 = await _auth.LoginAGuest();

        Assert.IsTrue(res1.Success);
    }

    [TestMethod]
    public async Task LoginAsGoogleAccount() {
        var res1 = await _auth.LoginAsGoogleAccount("[google id token]", null);

        Assert.IsTrue(res1.Success);
    }
}
