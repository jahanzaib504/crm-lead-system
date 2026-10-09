using Backend.Application.Dtos.Contact;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Common;

// If 
public class CreateLeadWithContactDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;

    [Required]
    public string JobTitle { get; set; } = null!;


    // Lead info also
    [Required]
    public string Title { get; set; } = null!;


    [Required]
    public string Source { get; set; } = null!;

    public decimal? EstimatedValue { get; set; }



}
