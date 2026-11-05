using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Api;
using Backend.Application.Dtos.Company;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }
    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponseDto>>> GetById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        return Ok(new ApiResponse<CompanyResponseDto>(StatusCodes.Status200OK, "Company retrieved successfully", company));
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet("{id}/contacts")]
    public async Task<ActionResult<ApiResponse<CompanyResponseDto>>> GetByIdWithContacts(int id)
    {
        var company = await _companyService.GetCompanyByIdWithContactsAsync(id);
        return Ok(new ApiResponse<CompanyResponseDto>(StatusCodes.Status200OK, "Company and contacts retrieved successfully", company));
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyResponseDto>>>> GetAll()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(new ApiResponse<IEnumerable<CompanyResponseDto>>(StatusCodes.Status200OK, "Companies retrieved successfully", companies));
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<CompanyResponseDto>>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await _companyService.GetPagedCompaniesAsync(pageNumber, pageSize);
        return Ok(new ApiResponse<PagedResponseDto<CompanyResponseDto>>(StatusCodes.Status200OK, "Companies retrieved successfully", pagedResult));
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CompanyResponseDto>>> Create([FromBody] CreateCompanyDto createCompanyDto)
    {
        var createdCompany = await _companyService.CreateCompanyAsync(createCompanyDto);
        var response = new ApiResponse<CompanyResponseDto>(StatusCodes.Status201Created, "Company created successfully", createdCompany);
        return CreatedAtAction(nameof(GetById), new { id = createdCompany.Id }, response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponseDto>>> Update(int id, [FromBody] UpdateCompanyDto updateCompanyDto)
    {
        var updatedCompany = await _companyService.UpdateCompanyAsync(id, updateCompanyDto);
        return Ok(new ApiResponse<CompanyResponseDto>(StatusCodes.Status200OK, "Company updated successfully", updatedCompany));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var deleted = await _companyService.DeleteCompanyAsync(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Company deleted successfully", deleted));
    }
}