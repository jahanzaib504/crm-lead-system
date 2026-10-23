using Backend.Application.Dtos.Common;
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
    public async Task<CompanyResponseDto> GetById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        return company;
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet("{id}/contacts")]
    public async Task<CompanyResponseDto> GetByIdWithContacts(int id)
    {
        var company = await _companyService.GetCompanyByIdWithContactsAsync(id);
        return company;
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet]
    public async Task<IEnumerable<CompanyResponseDto>> GetAll()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return companies;
    }

    [Authorize(Roles = "Admin, Sales Manager, Sales Rep")]
    [HttpGet("paged")]
    public async Task<PagedResponseDto<CompanyResponseDto>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await _companyService.GetPagedCompaniesAsync(pageNumber, pageSize);
        return pagedResult;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<CompanyResponseDto> Create([FromBody] CreateCompanyDto createCompanyDto)
    {
        var createdCompany = await _companyService.CreateCompanyAsync(createCompanyDto);
        return createdCompany;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<CompanyResponseDto> Update(int id, [FromBody] UpdateCompanyDto updateCompanyDto)
    {
        var updatedCompany = await _companyService.UpdateCompanyAsync(id, updateCompanyDto);
        return updatedCompany;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<bool> Delete(int id)
    {
        await _companyService.DeleteCompanyAsync(id);
        return true;
    }
}