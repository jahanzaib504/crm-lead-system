using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Lead;

public class CreateLeadDto
{

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public int ContactId { get; set; }


    [Required]
    public string Source { get; set; } = null!;

    [Required]
    public decimal? EstimatedValue { get; set; }

}
