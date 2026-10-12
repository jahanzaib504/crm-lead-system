using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("leads")]
public class LeadController : ControllerBase
{
    private readonly ILeadService _leadService;
    

    public LeadController(ILeadService leadService) {
        _leadService = leadService;
    }

    // For every new lead we must be create a contact 

    [HttpPost("create")]
    public async Task<bool> CreateLeadWithContact(CreateLeadWithContactDto dto)
    {
        await _leadService.RegisterWithContactAsync(dto);

        return true;
    }

    [HttpPost]
    [Authorize]
    // Create a lead for existant contact
    public async Task<LeadResponseDto> CreateLead(CreateLeadDto leadDto) {
        // Check if the contact exists for this lead
        var res = await _leadService.RegisterAsync(leadDto);
        return res;
    }
    [HttpGet("{id}")]
    [Authorize]
    public async Task<LeadResponseDto?> GetLead(int id)
    {
        return await _leadService.GetByIdAsync(id);
    }

    [HttpGet]
    [Authorize]
    public async Task<(IEnumerable<LeadResponseDto>, int)> GetLeadPaged(int pageNumber, int pageSize) {
        return await _leadService.GetPagedAsync(pageNumber, pageSize);
    }

    //[HttpGet("{id}")]
    //[Authorize]
    //public async Task<LeadResponseDto> GetLeadById(int Id)
    //{
        
    //}
};
