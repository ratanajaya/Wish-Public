namespace WishApp.Core.ServicesWithInterface;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string url);
}