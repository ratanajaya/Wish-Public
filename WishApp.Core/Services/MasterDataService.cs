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
using WishApp.Data.Utils;

namespace WishApp.Core.Services;

public class MasterDataService
{
    AppDbContext _db;
    IHttpContextService _httpContext;
    IStaticProvider _st;

    public MasterDataService(AppDbContext db, IHttpContextService httpContext, IStaticProvider st) {
        _db = db;
        _httpContext = httpContext;
        _st = st;
    }

    private static WishCrudModel MapWish(Wish wish) {
        return new WishCrudModel {
            Id = wish.Id,
            Name = wish.Name,
            EntityState = wish.EntityState,
            SubWishes = wish.SubWishes.Select(a => new SubWishCrudModel {
                Id = a.Id,
                Name = a.Name,
                EntityState = a.EntityState
            }).ToList(),
            WishOptions = wish.WishOptions.Select(a => new WishOptionCrudModel {
                Id = a.Id,
                Name = a.Name,
                Value = a.Value
            }).ToList()
        };
    }

    public WishCrudModel GetWish(string id) {
        var user = _httpContext.GetCurrentUser();

        var wish = _db.Wishes
            .AsNoTracking()
            .Include(a => a.SubWishes.Where(sw => sw.EntityState != AppEntityState.Deleted).OrderBy(sw => sw.Name))
            .Include(a => a.WishOptions.Where(wo => wo.EntityState != AppEntityState.Deleted).OrderBy(wo => wo.Value))
            .First(a => a.Id == id && a.UserId == user.Id && a.EntityState != AppEntityState.Deleted);

        return MapWish(wish);
    }

    public List<WishCrudModel> GetWishes() {
        var user = _httpContext.GetCurrentUser();

        var wishes = _db.Wishes
            .AsNoTracking()
            .Include(a => a.SubWishes.Where(sw => sw.EntityState != AppEntityState.Deleted).OrderBy(sw => sw.Name))
            .Include(a => a.WishOptions.Where(wo => wo.EntityState != AppEntityState.Deleted).OrderBy(wo => wo.Value))
            .Where(a => a.UserId == user.Id && a.EntityState != AppEntityState.Deleted)
            .Select(a => MapWish(a))
            .ToList();

        return wishes;
    }

    public async Task<ResponseResult<WishCrudModel>> InsertWish(WishCrudModel param) {
        var user = _httpContext.GetCurrentUser();
        var utcNow = _st.UtcNow;

        try {
            ValidateInsertUpdateParam(param);

            var wish = new Wish {
                Id = _st.NewGuid,
                Name = param.Name,
                UserId = user.Id,
            };

            var subWishes = param.SubWishes.Select(a => new SubWish {
                Id = _st.NewGuid,
                Name = a.Name,
                WishId = wish.Id,
                EntityState = a.EntityState,
            }).ToList();

            var wishOptions = param.WishOptions.Select(a => new WishOption {
                Id = _st.NewGuid,
                Name = a.Name,
                Value = a.Value,
                WishId = wish.Id
            }).ToList();

            _db.Add(wish);
            _db.AddRange(subWishes);
            _db.AddRange(wishOptions);

            await _db.SaveChangesAsync();

            var savedWish = GetWish(wish.Id);

            return new ResponseResult<WishCrudModel> {
                Success = true,
                Result = savedWish
            };
        }
        catch(Exception ex) {
            return new ResponseResult<WishCrudModel> {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ResponseResult<WishCrudModel>> UpdateWish(WishCrudModel param) {
        var user = _httpContext.GetCurrentUser();
        var utcNow = _st.UtcNow;

        try {
            ValidateInsertUpdateParam(param);

            var paramSwUpdateIds = param.SubWishes.Where(a => a.Id != null).Select(a => a.Id).ToList();
            var paramWoUpdateIds = param.WishOptions.Where(a => a.Id != null).Select(a => a.Id).ToList();

            var wish = _db.Wishes.First(a => a.Id == param.Id);

            #region Update
            wish.Name = param.Name;
            wish.EntityState = param.EntityState;

            var toUpdateSws = _db.SubWishes.Where(a => a.WishId == wish.Id).ToList();
            var toUpdateWos = _db.WishOptions.Where(a => a.WishId == wish.Id).ToList();

            foreach(var sw in toUpdateSws) {
                var swParam = param.SubWishes.FirstOrDefault(a => a.Id == sw.Id);
                if(swParam != null) {
                    sw.Name = swParam.Name;
                    sw.EntityState = swParam.EntityState;
                }
                else
                    sw.EntityState = AppEntityState.Deleted;
            }

            foreach(var wo in toUpdateWos) {
                var woParam = param.WishOptions.FirstOrDefault(a => a.Id == wo.Id);
                if(woParam != null) {
                    wo.Name = woParam.Name;
                    wo.Value = woParam.Value;
                }
                else
                    wo.EntityState = AppEntityState.Deleted;
            }

            _db.Update(wish);
            _db.UpdateRange(toUpdateSws);
            _db.UpdateRange(toUpdateWos);
            #endregion

            #region Insert
            var toInsertSw = param.SubWishes.Where(a => a.Id == null)
                .Select(a => new SubWish {
                    WishId = wish.Id,
                    Id = _st.NewGuid,
                    Name = a.Name,
                    EntityState= a.EntityState,
                }).ToList();

            var toInsertWo = param.WishOptions.Where(a => a.Id == null)
                .Select(a => new WishOption {
                    WishId = wish.Id,
                    Id = _st.NewGuid,
                    Name = a.Name,
                    Value = a.Value
                }).ToList();

            _db.AddRange(toInsertSw);
            _db.AddRange(toInsertWo);
            #endregion

            await _db.SaveChangesAsync();

            var savedWish = GetWish(wish.Id);

            return new ResponseResult<WishCrudModel> {
                Success = true,
                Result = savedWish
            };
        }
        catch(Exception ex) {
            return new ResponseResult<WishCrudModel> {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<Response> DeleteWish(string id) {
        var user = _httpContext.GetCurrentUser();
        var utcNow = _st.UtcNow;

        try {
            var wish = _db.Wishes.First(a => a.Id == id);

            wish.EntityState = AppEntityState.Deleted;

            _db.Update(wish);

            await _db.SaveChangesAsync();

            return new Response {
                Success = true
            };
        }
        catch(Exception ex) {
            return new Response {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task TransferDataOwnership(string srcUserId, string dstUserId) {
        var wishes = _db.Wishes.Where(a => a.UserId == srcUserId).ToList();

        wishes.ForEach(a => {
            a.UserId = dstUserId;
        });

        _db.UpdateRange(wishes);
        await _db.SaveChangesAsync();
    }

    private void ValidateInsertUpdateParam(WishCrudModel param) {
        if(string.IsNullOrWhiteSpace(param.Name)) {
            throw new Exception("Wish name can't be empty");
        }
        if(param.SubWishes.Any(a => string.IsNullOrWhiteSpace(a.Name))) {
            throw new Exception("Sub-wish name can't be empty");
        }
        if(param.WishOptions.Any(a => string.IsNullOrWhiteSpace(a.Name))) {
            throw new Exception("Option name can't be empty");
        }
        if(!param.WishOptions.Any()) {
            throw new Exception("Wish must contains at least one option");
        }
        if(param.WishOptions.Count != param.WishOptions.Select(a => a.Value).Count()) {
            throw new Exception("Param value(s) can't contain duplicate");
        }
    }

    public async Task<Response> CreateSampleWishes() {
        var user = _httpContext.GetCurrentUser();
        var utcNow = _st.UtcNow;

        try {
            var wish1 = new Wish {
                Id = _st.NewGuid,
                Name = "Buy A Video Game Console",
                UserId = user.Id,
            };

            var wish1Subwishes = new List<SubWish> {
                new() {
                    Id = _st.NewGuid,
                    Name = "Nintendo Switch",
                    WishId = wish1.Id,
                    EntityState = AppEntityState.Active
                },
                new() {
                    Id = _st.NewGuid,
                    Name = "Playstation 5",
                    WishId = wish1.Id,
                    EntityState = AppEntityState.Active
                },
                new() {
                    Id = _st.NewGuid,
                    Name = "Xbox Series X",
                    WishId = wish1.Id,
                    EntityState = AppEntityState.Active
                },
            };

            var wish1Options = new List<WishOption> {
                new() {
                    Id = _st.NewGuid,
                    WishId = wish1.Id,
                    Value = -2,
                    Name = "Very Negative"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish1.Id,
                    Value = -1,
                    Name = "Negative"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish1.Id,
                    Value = 0,
                    Name = "Neutral"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish1.Id,
                    Value = 1,
                    Name = "Positive"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish1.Id,
                    Value = 2,
                    Name = "Very Positive"
                }
            };

            _db.Add(wish1);
            _db.AddRange(wish1Subwishes);
            _db.AddRange(wish1Options);

            var wish2 = new Wish {
                Id = _st.NewGuid,
                Name = "Buy A Car",
                UserId = user.Id,
            };

            var wish2Subwishes = new List<SubWish> {
                new() {
                    Id = _st.NewGuid,
                    Name = "Need to be new",
                    WishId = wish2.Id,
                    EntityState = AppEntityState.Active
                },
                new() {
                    Id = _st.NewGuid,
                    Name = "Buy it by the end of the year",
                    WishId = wish2.Id,
                    EntityState = AppEntityState.Active
                },
            };

            var wish2Options = new List<WishOption> {
                new() {
                    Id = _st.NewGuid,
                    WishId = wish2.Id,
                    Value = -1,
                    Name = "Negative"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish2.Id,
                    Value = 0,
                    Name = "Neutral"
                },
                new() {
                    Id = _st.NewGuid,
                    WishId = wish2.Id,
                    Value = 1,
                    Name = "Positive"
                },
            };

            _db.Add(wish2);
            _db.AddRange(wish2Subwishes);
            _db.AddRange(wish2Options);

            await _db.SaveChangesAsync();

            return new Response {
                Success = true,
                Message = "Items has been created"
            };
        }
        catch(Exception ex) {
            return new Response {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
