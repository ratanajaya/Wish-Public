namespace WishApp.Core.ServicesWithInterface;

public interface IStaticProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    string NewGuid { get; }
}