using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.Note;

public class NoteResponseDto
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

}
