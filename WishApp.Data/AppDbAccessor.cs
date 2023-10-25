using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishApp.Data.Models.Auth;
using WishApp.Data.Models.Master;
using WishApp.Data.Models.Transaction;

namespace WishApp.Data;

[Obsolete("The Servicee classes now access DbContext directly")]
public interface IAppDbAccessor
{
    IQueryable<User> Users { get; }

    IQueryable<DailySubWish> DailySubWishes { get; }
    IQueryable<DailyWish> DailyWishes { get; }
    IQueryable<SubWish> SubWishes { get; }
    IQueryable<Wish> Wishes { get; }
    IQueryable<WishOption> WishOptions { get; }

    Task<int> SaveChangesAsync();

    void Add<T>(T entity);
    void Update<T>(T entity);
    void Remove<T>(T entity);

    void AddRange<T>(IEnumerable<T> entities) where T : class;
    void UpdateRange<T>(IEnumerable<T> entities) where T : class;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class;
}

public class AppDbAccessor : IAppDbAccessor
{
    public AppDbContext _dbContext;

    public AppDbAccessor(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    public IQueryable<User> Users => _dbContext.Users;

    public IQueryable<Wish> Wishes => _dbContext.Wishes;
    public IQueryable<SubWish> SubWishes => _dbContext.SubWishes;
    public IQueryable<WishOption> WishOptions => _dbContext.WishOptions;
    public IQueryable<DailyWish> DailyWishes => _dbContext.DailyWishes;
    public IQueryable<DailySubWish> DailySubWishes => _dbContext.DailySubWishes;

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

    public void Add<T>(T entity) => _dbContext.Add(entity!);
    public void Update<T>(T entity) => _dbContext.Update(entity!);
    public void Remove<T>(T entity) => _dbContext.Remove(entity!);

    public void AddRange<T>(IEnumerable<T> entities) where T : class => _dbContext.AddRange(entities);
    public void UpdateRange<T>(IEnumerable<T> entities) where T : class => _dbContext.UpdateRange(entities);
    public void RemoveRange<T>(IEnumerable<T> entities) where T : class  => _dbContext.RemoveRange(entities);
}
