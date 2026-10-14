using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.Note;

public class CreateNoteDto
{

    public int LeadId { get; set; }

    public string Content { get; set; } = null!;

}
