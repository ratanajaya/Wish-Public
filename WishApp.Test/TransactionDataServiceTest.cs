#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
using Microsoft.AspNetCore.Http;
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
public class TransactionDataServiceTest
{
    Mock<IHttpContextService> _httpContextService;
    Mock<IStaticProvider> _staticMock;

    TransactionDataService _transactionData;

    [TestInitialize]
    public void Initialize() {
        var db = Factory.GetInMemoryDbContext();
        _httpContextService = Factory.GetHttpContextServiceMock();
        _staticMock = Factory.GetStaticProviderMock();

        _transactionData = new TransactionDataService(
            db: db,
            httpContext: _httpContextService.Object,
            _staticMock.Object
        );
    }

    [TestMethod]
    public void GetDailyWishByDate() {
        var res1 = _transactionData.GetDailyWishesByDate(new (2023, 3, 9));

        Assert.IsFalse(res1.Any());

        var res2 = _transactionData.GetDailyWishesByDate(null);

        Assert.IsTrue(res2.Any());
    }

    [TestMethod]
    public async Task InsertDailyWish() {
        var res1 = await _transactionData.InsertDailyWish(new() {
            PkDate = new DateTime(2023, 3, 8),
            WishId = "w1",
            OptionId = "w1o1",
            SubWishOptions = new()
        });

        Assert.IsFalse(res1.Success);

        var res2 = await _transactionData.InsertDailyWish(new() {
            PkDate = new DateTime(2023, 3, 9),
            WishId = "w1",
            OptionId = "w1o1",
            SubWishOptions = new()
        });

        Assert.IsTrue(res2.Success);
    }
}
