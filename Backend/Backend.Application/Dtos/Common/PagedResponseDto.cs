using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.Common;

public class PagedResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
}
