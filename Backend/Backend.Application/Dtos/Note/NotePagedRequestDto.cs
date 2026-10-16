
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Note;

public class NotePagedRequestDto
{
    [Required]
    public int LeadId { get; set; }


    [Required, Range(1, int.MaxValue)]
    public int PageNumber { get; set; }


    [Required, Range(1, int.MaxValue)]
    public int PageSize { get; set; }
}
