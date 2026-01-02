using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Common.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public long Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}