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

    [HttpPost]
    [Authorize]
    public async Task<LeadResponseDto> CreateLead(CreateLeadDto leadDto) {
        // Check if the company exists for this lead
        var res = await _leadService.RegisterAsync(leadDto);
        return res;
    }
    // For unauthorize user contact must be created
    //public async Task<> CreateLeadWithContact()
    //{

    //}
    

};
