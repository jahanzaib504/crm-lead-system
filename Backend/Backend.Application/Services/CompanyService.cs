using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Company;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Services
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto> GetCompanyByIdAsync(int companyId);
        Task<CompanyResponseDto> GetCompanyByIdWithContactsAsync(int companyId);
        Task<IEnumerable<CompanyResponseDto>> GetAllCompaniesAsync();
        Task<PagedResponseDto<CompanyResponseDto>> GetPagedCompaniesAsync(int pageNumber, int pageSize);
        Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
        Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, UpdateCompanyDto updateCompanyDto);
        Task<bool> DeleteCompanyAsync(int companyId);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepositry _companyRepository;
        private readonly AuditLogService _auditLogService;
        private readonly IUserManager _userManager;

        public CompanyService(
            ICompanyRepositry companyRepository,
            AuditLogService auditLogService,
            IUserManager userManager)
        {
            _companyRepository = companyRepository;
            _auditLogService = auditLogService;
            _userManager = userManager;
        }

        public async Task<CompanyResponseDto> GetCompanyByIdAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"Company with ID {companyId} not found.");
            }
            return company.Adapt<CompanyResponseDto>();
        }

        public async Task<CompanyResponseDto> GetCompanyByIdWithContactsAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdWithContactsAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"Company with ID {companyId} not found.");
            }
            return company.Adapt<CompanyResponseDto>();
        }

        public async Task<IEnumerable<CompanyResponseDto>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return companies.Adapt<IEnumerable<CompanyResponseDto>>();
        }

        public async Task<PagedResponseDto<CompanyResponseDto>> GetPagedCompaniesAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _companyRepository.GetPagedAsync(pageNumber, pageSize);
            var dtos = items.Adapt<IEnumerable<CompanyResponseDto>>();
            return new PagedResponseDto<CompanyResponseDto> { Items = dtos, TotalCount = totalCount };
        }

        public async Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
        {
            var company = createCompanyDto.Adapt<Company>();
            var createdCompany = await _companyRepository.AddAsync(company);
            await _auditLogService.LogAsync(
                "Company",
                createdCompany.Id,
                "create",
                createCompanyDto,
                _userManager.GetCurrentUserId());
            return createdCompany.Adapt<CompanyResponseDto>();
        }

        public async Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, UpdateCompanyDto updateCompanyDto)
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
            {
                throw new NotFoundException($"Company with ID {companyId} not found.");
            }

            updateCompanyDto.Adapt(existingCompany);
            var updatedCompany = await _companyRepository.UpdateAsync(existingCompany);
            await _auditLogService.LogAsync(
                "Company",
                updatedCompany.Id,
                "update",
                updateCompanyDto,
                _userManager.GetCurrentUserId());
            return updatedCompany.Adapt<CompanyResponseDto>();
        }

        public async Task<bool> DeleteCompanyAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"Company with ID {companyId} not found.");
            }

            var deleted = await _companyRepository.DeleteAsync(companyId);
            if (deleted)
            {
                await _auditLogService.LogAsync(
                    "Company",
                    companyId,
                    "delete",
                    new { company.Name },
                    _userManager.GetCurrentUserId());
            }

            return deleted;
        }
    }
}