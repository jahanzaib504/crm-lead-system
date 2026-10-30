export interface StageCount {
    stageName: string;
    count: number;
}

export interface SourceCount {
    source: string;
    count: number;
}

export interface RepPerformance {
    repId: number;
    repName: string;
    leadCount: number;
    totalValue: number;
    status: boolean; // true for active, false for inactive
}

export interface DashboardAnalytics {
    role: string;
    totalLeads: number;
    openPoolLeads: number;
    convertedLeads: number;
    totalPipelineValue: number;
    conversionRate: number;
    totalUsers: number;
    pendingApprovals: number;

    // Manager
    teamLeadsCount: number;
    activeRepsCount: number;
    teamPipelineValue: number;
    repPerformance: RepPerformance[];

    // Rep
    myLeadsCount: number;
    myPipelineValue: number;
    pendingFollowUpsCount: number;
    completedActivitiesCount: number;

    stageBreakdown: StageCount[];
    sourceBreakdown: SourceCount[];
}
