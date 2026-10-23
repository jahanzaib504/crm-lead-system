using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Services;

public interface IIntelligenceService
{
    public Task<PagedResponseDto<LeadResponseDto>> GetRecommendations(
        int pageNumber,
        int pageSize
    );
};
public class IntelligenceService : IIntelligenceService
{
    private readonly ILeadService _leadService;
    public IntelligenceService(ILeadService leadService)
    {
        _leadService = leadService ?? throw new ArgumentNullException(nameof(leadService));
    }
    private async Task<int> CalculateLeadScoreAsync(LeadResponseDto lead)
    {
        // Caculate score based on past leads by a contact, company, or similar leads
        int score = 0;
        if (lead.EstimatedValue.HasValue)
        {
            score += (int)(lead.EstimatedValue.Value / 1000); // Example: 1 point for every $1000
        }
        score += lead.LeadStageId * 10; // Example: 10 points for each lead stage
        // Get contact info of lead
        if (!string.IsNullOrEmpty(lead.ContactEmail))
        {
            score += 5; // Example: 5 points for having a contact email
        }

        return score;
    }
    public async Task<PagedResponseDto<LeadResponseDto>> GetRecommendations(
    int pageNumber,
    int pageSize
    )
    {
        var leads = await _leadService.GetPagedAsync(pageNumber, pageSize);
        // Calculate leads score based on some criteria
        foreach (var lead in leads.Items)
        {
            lead.CurrentScore = await CalculateLeadScoreAsync(lead);
        }
        return new PagedResponseDto<LeadResponseDto>{ Items = leads.Items, TotalCount = leads.TotalCount };
    }
};
