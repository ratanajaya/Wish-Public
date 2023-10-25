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
public class MasterDataServiceTest
{
    Mock<IHttpContextService> _httpContextServiceMock;
    Mock<IStaticProvider> _staticMock;

    MasterDataService _masterData;
    AppDbContext _db;

    [TestInitialize]
    public void Initialize() {
        _db = Factory.GetInMemoryDbContext();
        _httpContextServiceMock = Factory.GetHttpContextServiceMock();
        _staticMock = Factory.GetStaticProviderMock();

        _masterData = new MasterDataService(
            db: _db,
            httpContext: _httpContextServiceMock.Object,
            st: _staticMock.Object
        );
    }

    [TestMethod]
    public void GetWish() {
        var res1 = _masterData.GetWish("w1");

        Assert.IsTrue(res1 != null);

        Assert.ThrowsException<InvalidOperationException>(() => _masterData.GetWish("Random text"));
    }

    [TestMethod]
    public void GetWishes() {
        var res1 = _masterData.GetWishes();
        Assert.IsTrue(res1.Any());
    }

    [TestMethod]
    public async Task InsertWish() {
        var param = new WishCrudModel {
            Name = "Test Wish",
            SubWishes = new List<SubWishCrudModel> {
                new() {
                    Name = "Sub Wish 1"
                }
            },
            WishOptions = new List<WishOptionCrudModel> {
                new() {
                    Name = "Option 1",
                    Value = 1
                }
            }
        };

        var res1 = await _masterData.InsertWish(param);

        Assert.IsTrue(res1!.Success);
    }

    [TestMethod]
    public async Task UpdateWish() {
        var param = new WishCrudModel {
            Id = "w1",
            Name = "Test Wish",
            SubWishes = new List<SubWishCrudModel> {
                new() {
                    Id = "w1s1",
                    Name = "Sub Wish 1"
                }
            },
            WishOptions = new List<WishOptionCrudModel> {
                new() {
                    Id = "w1o1",
                    Name = "Option 1 Updated",
                    Value = 1
                },
                new() {
                    Name = "Option 99",
                    Value = 99
                }
            }
        };

        var res1 = await _masterData.UpdateWish(param);

        Assert.IsTrue(res1!.Success);
    }

    [TestMethod]
    public async Task DeleteWish() {
        var res1 = await _masterData.DeleteWish("w1");

        Assert.IsTrue(res1.Success);

        Assert.IsTrue(_db.Wishes.Any(a => a.Id == "w1" && a.EntityState == Data.Utils.AppEntityState.Deleted));
    }

    [TestMethod]
    public async Task TransferDataOwnership() {
        await _masterData.TransferDataOwnership("DR", "Member1");

        Assert.IsFalse(_db.Wishes.Any(a => a.UserId == "DR"));
        Assert.IsTrue(_db.Wishes.Any(a => a.UserId == "Member1"));
    }
}
