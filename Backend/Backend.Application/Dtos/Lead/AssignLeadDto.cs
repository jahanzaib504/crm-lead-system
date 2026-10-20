namespace Backend.Application.Dtos.Lead;

public class AssignLeadDto
{
    public int? SalesRepId { get; set; }
    public int? ManagerId { get; set; }
    public int? UserId { get; set; }
}

public class UpdateLeadStageDto
{
    public int StageId { get; set; }
}

