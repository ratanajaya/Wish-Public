using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniGuids;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WishApp.Core.Models;
using WishApp.Core.ServicesWithInterface;
using WishApp.Data;
using WishApp.Data.Models.Auth;
using WishApp.Data.Models.Master;
using WishApp.Data.Models.Transaction;

namespace WishApp.Test;

internal static class Factory
{
    public const string AuditUserId = "DR";

    public static readonly AppSetting AppSetting = new AppSetting {
        GoogleSsoClientId = "[SSOCLIENTID]",
        GoogleSsoClientSecret = "",
        JwtIssuerSigningKey = "0b2ecbbc-013f-4e8f-a9c4-4f9b3497019a"
    };

    public static AppDbContext GetInMemoryDbContext() {
        #region Initialize DbContext
        //Note: It appears that TestInitialize is executed every time a test is run (not at class level)
        //Every single TestMethod has its own instance of AppDbContext, IHttpContextService, etc.
        //Therefore it's necessary for the Data Source of the in memory db's connection string to be unique to avoid "collision" when multiple instances are instantiated by multiple tests
        var keepAliveConnection = new SqliteConnection($"Data Source={MiniGuid.NewGuid()};Mode=Memory;Cache=Shared");
        keepAliveConnection.Open();

        var option = new DbContextOptionsBuilder<AppDbContext>();
        option.UseSqlite(keepAliveConnection);

        var db = new AppDbContext(option.Options);

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        #endregion

        #region Seed Auth Data
        var users = new List<User> {
            new() {
                Id = "DR",
                UserName = "DR",
                Email = "DR@mail.com",
                UserType = UserType.Admin,
                PasswordHash = "AQAAAAEAACcQAAAAEDPXDVRDHUMMARTMJnX0kOvvu7bdsGXQJQkwwRu3ML6TuTIfzYINIWmBngLgvIxfjQ=="
            },
            new() {
                Id = "Member1",
                UserName = "Member1",
                Email = "Member1@mail.com",
                UserType = UserType.Member
            }
        };

        db.Users.AddRange(users);
        #endregion

        #region Seed Master Data
        var defaultWishOptions = Enumerable.Range(0, 5).Select(a =>
            new WishOption {
                Id = $"o{a}",
                Name = $"Option {a}",
                Value = a,
                WishId = string.Empty,
            });

        var getDefaultWishOption = new Func<int, string, List<WishOption>>((n, wishId) => {
            return Enumerable.Range(0, n).Select(a =>
                new WishOption {
                    Id = $"{wishId}o{a}",
                    Name = $"Option {a}",
                    Value = a,
                    WishId = wishId,
                }).ToList();
        });

        var wish1 = new Wish() {
            Id = "w1",
            Name = "Own a VR Device",
            UserId = AuditUserId,
            SubWishes = new List<SubWish> {
                    new() {
                        WishId = "w1",
                        Id = "w1s1",
                        Name = "Buy Meta Quest 2",
                    },
                    new() {
                        WishId = "w1",
                        Id = "w1s2",
                        Name = "Buy Valve Index"
                    },
                    new() {
                        WishId = "w1",
                        Id = "w1s3",
                        Name = "Wait For Meta Quest 3"
                    }
                },
            WishOptions = getDefaultWishOption(5, "w1")
        };

        var wish2 = new Wish() {
            Id = "w2",
            Name = "Own A Car",
            UserId = AuditUserId,
            SubWishes = new List<SubWish> {
                    new() {
                        WishId = "w2",
                        Id = "w2s1",
                        Name = "Need to be new"
                    },
                    new() {
                        WishId = "w2",
                        Id = "w2s2",
                        Name = "Need to have space for 6 people"
                    }
                },
            WishOptions = getDefaultWishOption(3, "w2")
        };

        var wishG = new Wish() {
            Id = "Guid1",
            Name = "Wish with Guid1 id",
            UserId = AuditUserId
        };

        var wishQ = new List<Wish> {
            wish1,
            wish2,
            wishG
        };

        db.Wishes.AddRange(wishQ);
        #endregion

        #region Seed Transaction Data
        var dailyWishes = new List<DailyWish> {
            new() {
                Id = "dw1",
                UserId = AuditUserId,
                SelectedOptionId = wish1.WishOptions.First().Id,
                SelectedOption = wish1.WishOptions.First(),
                WishId = wish1.Id,
                Wish = wish1,
                PkDate = new (2023, 3, 8)
            }
        };

        db.DailyWishes.AddRange(dailyWishes);
        #endregion

        db.SaveChanges();

        return db;
    }

    public static Mock<IHttpContextService> GetHttpContextServiceMock() {
        var result = new Mock<IHttpContextService>();
        result.Setup(a => a.GetCurrentUser()).Returns(new Core.Models.UserClaim {
            Id = AuditUserId,
            UserName = "DR",
        });

        return result;
    }

    //Assumes test is performed at 7AM, 8 March 2023 Western Indonesia time
    public static Mock<IStaticProvider> GetStaticProviderMock() {
        var result = new Mock<IStaticProvider>();
        result.Setup(a => a.UtcNow).Returns(new DateTime(2023, 3, 8, 0, 0, 0));
        result.Setup(a => a.Now).Returns(new DateTime(2023, 3, 8, 7, 0, 0));

        result.Setup(a => a.NewGuid).Returns(Guid.NewGuid().ToString());

        return result;
    }

    public static Mock<IIdentityService> GetIdentityServiceMock() {
        var result = new Mock<IIdentityService>();
        result.Setup(a => a.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        result.Setup(a => a.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(Task.FromResult(IdentityResult.Success));

        return result;
    }

    private static readonly string _googleOauthResponse = "{\r\n  \"iss\": \"https://accounts.google.com\",\r\n  \"azp\": \"[SSOCLIENTID]\",\r\n  \"aud\": \"[SSOCLIENTID]\",\r\n  \"sub\": \"102965554365258750299\",\r\n  \"email\": \"loremipsum@gmail.com\",\r\n  \"email_verified\": \"true\",\r\n  \"nbf\": \"1691466548\",\r\n  \"name\": \"DR\",\r\n  \"picture\": \"\",\r\n  \"given_name\": \"DR\",\r\n  \"family_name\": \"DR\",\r\n  \"locale\": \"en\",\r\n  \"iat\": \"1691466848\",\r\n  \"exp\": \"1691470448\",\r\n  \"jti\": \"6854b1b6693de33461bcece21c14c65e73b1d8de\",\r\n  \"alg\": \"RS256\",\r\n  \"kid\": \"911e39e27928ae9f1e9d1e21646de92d19351b44\",\r\n  \"typ\": \"JWT\"\r\n}";

    public static Mock<IHttpClientService> GetHttpClientServiceMock() {
        var result = new Mock<IHttpClientService>();
        result.Setup(a => a.GetAsync(It.Is<string>(str => str.StartsWith("https://oauth2.googleapis.com/tokeninfo?id_token="))))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(_googleOauthResponse, System.Text.Encoding.UTF8, "application/json")
            });

        return result;
    }
}
