using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Services;

public interface IIntelligenceService
{
    Task<PagedResponseDto<LeadResponseDto>> GetRecommendations(int pageNumber, int pageSize);
}

public class IntelligenceService : IIntelligenceService
{
    private readonly ILeadService _leadService;

    public IntelligenceService(ILeadService leadService)
    {
        _leadService = leadService ?? throw new ArgumentNullException(nameof(leadService));
    }

    public async Task<PagedResponseDto<LeadResponseDto>> GetRecommendations(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var leads = await _leadService.GetPagedAsync(pageNumber, pageSize, null, null, true);

        return new PagedResponseDto<LeadResponseDto>
        {
            Items = leads.Items
                .OrderByDescending(lead => lead.CurrentScore ?? 0m)
                .ThenByDescending(lead => lead.EstimatedValue ?? 0m)
                .ToList(),
            TotalCount = leads.TotalCount
        };
    }
}
