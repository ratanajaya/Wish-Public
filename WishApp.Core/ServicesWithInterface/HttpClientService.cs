using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.ServicesWithInterface;

public class HttpClientService : IHttpClientService
{
    public async Task<HttpResponseMessage> GetAsync(string url) {
        using(var client = new HttpClient()) {
            return await client.GetAsync(url);
        }
    }
}
