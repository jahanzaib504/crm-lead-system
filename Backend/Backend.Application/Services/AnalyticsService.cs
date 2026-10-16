using Backend.Application.Dtos.Analytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Services;

public interface IAnalyticsService
{
    public Task<AnalyticsResponseDto> GetAnalytics();
}

//public class AnalyticsService
//{
//    public Task<AnalyticsResponseDto> GetAnalytics()
//    {

//    }
//}
