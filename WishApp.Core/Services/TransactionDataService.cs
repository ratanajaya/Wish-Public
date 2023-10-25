using Microsoft.EntityFrameworkCore;
using MiniGuids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Core.ServicesWithInterface;
using WishApp.Core.Models;
using WishApp.Data;
using WishApp.Data.Models.Master;
using WishApp.Data.Models.Transaction;
using WishApp.Data.Utils;

namespace WishApp.Core.Services;

public class TransactionDataService
{
    AppDbContext _db;
    IHttpContextService _httpContext;
    IStaticProvider _st;

    public TransactionDataService(AppDbContext db, IHttpContextService httpContext, IStaticProvider st) {
        _db = db;
        _httpContext = httpContext;
        _st = st;
    }

    public List<DailyWish> GetDailyWishesByDate(DateTime? date) {
        var targetDate = date ?? _st.UtcNow;
        var user = _httpContext.GetCurrentUser();

        var result = _db.DailyWishes
            .AsNoTracking()
            .Include(a => a.Wish)
            .Include(a => a.SelectedOption)
            .Include(a => a.DailySubWishes)
            .ThenInclude(b => b.SelectedOption)
            .Where(a => a.UserId == user.Id && a.PkDate.Date == targetDate.Date)
            .ToList();

        return result;
    }

    public async Task<ResponseResult<DailyWish>> InsertDailyWish(DailyWishInsertModel param) {
        var user = _httpContext.GetCurrentUser();

        #region Validation
        var wish = _db.Wishes
            .AsNoTracking()
            .Include(a => a.SubWishes)
            .Include(a => a.WishOptions)
            .First(a => a.Id == param.WishId && a.UserId == user.Id);

        var validOptionIds = wish.WishOptions.Select(b => b.Id).ToList();

        if(param.SubWishOptions.Any(a => !wish.SubWishes.Select(b => b.Id).Contains(a.SubWishId)))
            return new() {
                Success = false,
                Message = "Invalid subwish(es) id"
            };

        if(param.SubWishOptions.Any(a => !validOptionIds.Contains(a.OptionId)) || !validOptionIds.Contains(param.OptionId))
            return new() {
                Success = false,
                Message = "Invalid option(s) id"
            };

        if(_db.DailyWishes.Any(a => a.PkDate.Date == param.PkDate))
            return new() {
                Success = false,
                Message = $"DailyWish with the same PkDate [{param.PkDate.ToString("yyyy/MM/dd")}] already exists"
            };
        #endregion

        var dailyWish = new DailyWish {
            Id = MiniGuid.NewGuid().ToString(),
            UserId = user.Id,
            PkDate = param.PkDate,
            WishId = param.WishId,
            SelectedOptionId = param.OptionId,
            Remark = param.Remark
        };

        var dailySubwishes = param.SubWishOptions.Select(a => new DailySubWish {
            Id = MiniGuid.NewGuid().ToString(),
            DailyWishId = dailyWish.Id,
            SubWishId = a.SubWishId,
            SelectedOptionId = a.OptionId,
            Remark = a.Remark
        }).ToList();

        _db.Add(dailyWish);
        _db.AddRange(dailySubwishes);
        await _db.SaveChangesAsync();

        var insertedDw = _db.DailyWishes
            .AsNoTracking()
            .Include(a => a.Wish)
            .Include(a => a.SelectedOption)
            .Include(a => a.DailySubWishes)
            .ThenInclude(b => b.SelectedOption)
            .First(a => a.Id == dailyWish.Id);

        return new ResponseResult<DailyWish> {
            Success = true,
            Message = "Success",
            Result = insertedDw
        };
    }

    public List<DailyWishSummary> GetDailyWishSummaries() {
        var user = _httpContext.GetCurrentUser();

        try {
            var allWishes = _db.Wishes
                .Include(a => a.SubWishes
                    .Where(sw => sw.EntityState == AppEntityState.Active))
                .Include(a => a.WishOptions)
                .Where(a => a.UserId == user.Id && a.EntityState == AppEntityState.Active)
                .ToList();

            var allDailyWishes = _db.DailyWishes
                .Include(a => a.SelectedOption)
                .Include(a => a.DailySubWishes)
                .ThenInclude(a => a.SelectedOption)
                .Where(a => a.UserId == user.Id)
                .ToList();

            var result = allWishes.Select(w => {
                var dailyWishes = allDailyWishes
                    .Where(dw => dw.WishId == w.Id)
                    .ToList();

                var dwCount = dailyWishes.Count;

                var dwSum = dailyWishes.DefaultIfEmpty()
                    .Sum(dw => dw?.SelectedOption?.Value ?? 0);

                var dwMean = dwCount > 0 ? dwSum / dwCount : 0;

                var dwWithRemarks = dailyWishes
                    .Where(dw => !string.IsNullOrEmpty(dw.Remark))
                    .ToList();

                return new DailyWishSummary {
                    WishId = w.Id,
                    Name = w.Name,
                    Count = dwCount,
                    Sum = dwSum,
                    Mean = dwMean,
                    Remarks = dwWithRemarks.Select(dw => new RemarkSummary {
                        PkDate = dw.PkDate,
                        Remark = dw.Remark!
                    }).ToList(),
                    WishOptions = w.WishOptions.Select(a => new WishOptionCrudModel {
                        Id = a.Id,
                        Value = a.Value,
                        Name = a.Name,
                    }).ToList(),
                    DailySubWishes = w.SubWishes.Select(sw => {
                        var dailySubWishes = dailyWishes
                            .SelectMany(dw => dw.DailySubWishes
                                .Where(dsw => dsw.SubWishId == sw.Id))
                            .ToList();

                        var dswCount = dailySubWishes.Count;

                        var dswSum = dailySubWishes.DefaultIfEmpty()
                            .Sum(dsw => dsw?.SelectedOption?.Value ?? 0);

                        var dswMean = dswCount > 0 ? dswSum / dswCount : 0;

                        return new DailySubWishSummary {
                            SubWishId = sw.Id,
                            Name = sw.Name,
                            Count = dswCount,
                            Sum = dswSum,
                            Mean = dswMean,
                        };
                    }).ToList()
                };
            }).ToList();

            return result;
        }
        catch (Exception ex) {
            //TODO add log
            throw;
        }
    }
}
