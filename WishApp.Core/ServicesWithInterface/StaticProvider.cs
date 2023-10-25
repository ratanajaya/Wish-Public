using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.ServicesWithInterface;

public class StaticProvider : IStaticProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;

    public string NewGuid => Guid.NewGuid().ToString();
}
