export interface Company{
    id: string;
    name: string;
    industry: string;
    employeeCount: number;
    annualRevenue?: number;
    website?: string;
    phone?: string;
    createdAt: string;
}

/*
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Industry { get; set; }

    public int? EmployeeCount { get; set; }

    public decimal? AnnualRevenue { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }
*/