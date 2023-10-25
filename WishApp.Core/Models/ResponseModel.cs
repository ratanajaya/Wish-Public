using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishApp.Core.Models;

public record Response
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public record ResponseResult<T> : Response
{
    public T? Result { get; set; }
}