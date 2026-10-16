using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Common;

public class PaginationRequestDto
{
    [Required, Range(1, int.MaxValue)]
    public int PageNumber { get; set; }
    [Required, Range(1, int.MaxValue)]
    public int PageSize { get; set; }
}
