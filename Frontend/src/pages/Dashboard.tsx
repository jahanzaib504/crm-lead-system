import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useUser } from "../store/userStore";
import { getDashboardAnalytics } from "../services/analyticsService";
import { getPendingUsers, getManagers, approveUser } from "../services/userService";
import { getOpenPoolLeads, acquireLead } from "../services/leadService";
import { getMyPendingFollowUps, updateFollowUpStatus } from "../services/followUpService";
import { useState } from "react";
import { Link } from "react-router";
import toast from "react-hot-toast";
import { 
    Users, 
    Layers, 
    DollarSign, 
    Clock, 
    ArrowRight, 
    Calendar, 
    Activity,
    Check,
    X
} from "lucide-react";
import { 
    BarChart, 
    Bar, 
    XAxis, 
    YAxis, 
    Tooltip, 
    ResponsiveContainer, 
    Cell,
    PieChart,
    Pie
} from "recharts";
import { UserAccessModal } from "../components/UserAccessModel";

const STAGE_COLORS: Record<string, string> = {
    "New": "#3b82f6",
    "Contacted": "#f59e0b",
    "Qualified": "#8b5cf6",
    "Proposal": "#6366f1",
    "Closed Won": "#10b981",
    "Closed Lost": "#ef4444"
};

const PIE_COLORS = ["#3b82f6", "#10b981", "#f59e0b", "#8b5cf6", "#ec4899", "#06b6d4"];

export const Dashboard = () => {
    const user = useUser((state) => state.user);
    const roleName = user?.role?.name || "Sales Rep";
    const queryClient = useQueryClient();
    const [selectedPendingUser, setSelectedPendingUser] = useState<any>(null);

    // Queries
    const { data: analytics, isPending: analyticsLoading } = useQuery({
        queryKey: ["dashboard-analytics"],
        queryFn: getDashboardAnalytics,
    });

    const { data: pendingUsers } = useQuery({
        queryKey: ["pending-users"],
        queryFn: getPendingUsers,
        enabled: roleName === "Admin",
    });


    const { data: openPoolLeads } = useQuery({
        queryKey: ["open-pool-leads"],
        queryFn: getOpenPoolLeads,
        enabled: roleName === "Sales Manager" || roleName === "Admin",
    }); // only works if the current role is sales manager or a admin.

    const { data: myFollowUps } = useQuery({
        queryKey: ["my-followups"],
        queryFn: getMyPendingFollowUps,
        enabled: roleName === "Sales Rep",
    }); // only works for sales reps.


    const acquireMutation = useMutation({
        mutationFn: (leadId: number) => acquireLead(leadId),
        onSuccess: () => {
            toast.success("Lead acquired successfully!");
            queryClient.invalidateQueries({ queryKey: ["open-pool-leads"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
            queryClient.invalidateQueries({ queryKey: ["leads"] });
        },
        onError: (err: any) => {
            toast.error(err.response?.data?.detail || "Failed to acquire lead.");
        }
    });

    const completeFollowUpMutation = useMutation({
        mutationFn: (id: number) => updateFollowUpStatus(id, "Completed"),
        onSuccess: () => {
            toast.success("Follow-up marked as completed!");
            queryClient.invalidateQueries({ queryKey: ["my-followups"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        }
    });


    if (analyticsLoading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 border-b border-gray-200">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 tracking-tight">
                        {roleName === "Admin" && "Executive Dashboard"}
                        {roleName === "Sales Manager" && "Sales Manager Dashboard"}
                        {roleName === "Sales Rep" && "Representative Workspace"}
                    </h1>
                    <p className="text-sm text-gray-500 mt-1">
                        {roleName === "Admin" && "System overview, pending staff approvals, and conversion metrics."}
                        {roleName === "Sales Manager" && "Manage team workload, track leads, and claim opportunities from the open pool."}
                        {roleName === "Sales Rep" && "Track your assigned leads, upcoming follow-ups, and customer touchpoints."}
                    </p>
                </div>

                <div className="flex items-center gap-2">
                    {roleName !== "Sales Rep" && (
                        <Link
                            to="/pool"
                            className="inline-flex items-center gap-1.5 px-3.5 py-2 text-xs font-semibold rounded-lg bg-indigo-50 text-indigo-700 hover:bg-indigo-100 border border-indigo-200 transition-colors"
                        >
                            <Layers className="w-4 h-4" />
                            Open Pool ({analytics?.openPoolLeads ?? 0})
                        </Link>
                    )}
                    <Link
                        to="/leads"
                        className="inline-flex items-center gap-1.5 px-3.5 py-2 text-xs font-semibold rounded-lg bg-blue-600 text-white hover:bg-blue-700 shadow-xs transition-colors"
                    >
                        View All Leads
                        <ArrowRight className="w-3.5 h-3.5" />
                    </Link>
                </div>
            </div>

            {/* ========================================================== */}
            {/* 1. ADMIN DASHBOARD VIEW */}
            {/* ========================================================== */}
            {roleName === "Admin" && (
                <div className="space-y-8">
                    {/* Stat Cards */}
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Total Leads</span>
                                <div className="w-8 h-8 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center">
                                    <Layers className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.totalLeads ?? 0}</div>
                            <div className="text-xs text-gray-400 mt-1">
                                {analytics?.openPoolLeads ?? 0} in open unassigned pool
                            </div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Pipeline Value</span>
                                <div className="w-8 h-8 rounded-lg bg-emerald-50 text-emerald-600 flex items-center justify-center">
                                    <DollarSign className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">
                                ${(analytics?.totalPipelineValue ?? 0).toLocaleString()}
                            </div>
                            <div className="text-xs text-emerald-600 font-medium mt-1">
                                {analytics?.convertedLeads ?? 0} deals won ({analytics?.conversionRate ?? 0}% win rate)
                            </div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Total Staff</span>
                                <div className="w-8 h-8 rounded-lg bg-purple-50 text-purple-600 flex items-center justify-center">
                                    <Users className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.totalUsers ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Registered users across all roles</div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Pending Approvals</span>
                                <div className="w-8 h-8 rounded-lg bg-amber-50 text-amber-600 flex items-center justify-center">
                                    <Clock className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-amber-600">{analytics?.pendingApprovals ?? 0}</div>
                            <div className="text-xs text-amber-700 mt-1">
                                {analytics?.pendingApprovals ? "Requires administrator action" : "All users reviewed"}
                            </div>
                        </div>
                    </div>

                    {/* Pending Approvals Widget */}
                    {pendingUsers && pendingUsers.length > 0 && (
                        <div className="bg-white rounded-xl border border-amber-200 shadow-xs overflow-hidden">
                            <div className="bg-amber-50/70 px-6 py-4 border-b border-amber-100 flex items-center justify-between">
                                <div className="flex items-center gap-2">
                                    <Clock className="w-5 h-5 text-amber-600" />
                                    <h3 className="font-semibold text-gray-900">Pending User Access Requests ({pendingUsers.length})</h3>
                                </div>
                                <span className="text-xs text-amber-800 font-medium">New registrations requiring role & manager assignment</span>
                            </div>
                            <div className="divide-y divide-gray-100">
                                {pendingUsers.map((pUser) => (
                                    <div key={pUser.id} className="p-4 sm:px-6 flex flex-col sm:flex-row sm:items-center justify-between gap-3 hover:bg-gray-50/50">
                                        <div>
                                            <p className="font-semibold text-gray-900">{pUser.fullName}</p>
                                            <p className="text-xs text-gray-500">{pUser.email}</p>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <button
                                                onClick={() => {
                                                    setSelectedPendingUser(pUser);
                                                    
                                                }}
                                                className="px-3.5 py-1.5 bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold rounded-lg shadow-xs transition"
                                            >
                                                Assign Role & Approve
                                            </button>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {/* Analytics Charts */}
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                        {/* Pipeline Stage Distribution */}
                        <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-xs">
                            <h3 className="font-bold text-gray-900 mb-4 text-base">Pipeline Stage Distribution</h3>
                            {analytics?.stageBreakdown && analytics.stageBreakdown.length > 0 ? (
                                <div className="h-64">
                                    <ResponsiveContainer width="100%" height="100%">
                                        <BarChart data={analytics.stageBreakdown} margin={{ top: 10, right: 10, left: -20, bottom: 20 }}>
                                            <XAxis dataKey="stageName" tick={{ fontSize: 11 }} interval={0} angle={-20} textAnchor="end" />
                                            <YAxis allowDecimals={false} tick={{ fontSize: 11 }} />
                                            <Tooltip />
                                            <Bar dataKey="count" radius={[4, 4, 0, 0]}>
                                                {analytics.stageBreakdown.map((entry, index) => (
                                                    <Cell key={`cell-${index}`} fill={STAGE_COLORS[entry.stageName] || "#3b82f6"} />
                                                ))}
                                            </Bar>
                                        </BarChart>
                                    </ResponsiveContainer>
                                </div>
                            ) : (
                                <div className="h-64 flex items-center justify-center text-sm text-gray-400">No stage data available</div>
                            )}
                        </div>

                        {/* Leads by Source */}
                        <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-xs">
                            <h3 className="font-bold text-gray-900 mb-4 text-base">Leads by Inbound Source</h3>
                            {analytics?.sourceBreakdown && analytics.sourceBreakdown.length > 0 ? (
                                <div className="h-64 flex items-center">
                                    <ResponsiveContainer width="50%" height="100%">
                                        <PieChart>
                                            <Pie
                                                data={analytics.sourceBreakdown}
                                                dataKey="count"
                                                nameKey="source"
                                                cx="50%"
                                                cy="50%"
                                                innerRadius={45}
                                                outerRadius={75}
                                                paddingAngle={4}
                                            >
                                                {analytics.sourceBreakdown.map((_, index) => (
                                                    <Cell key={`slice-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />
                                                ))}
                                            </Pie>
                                            <Tooltip />
                                        </PieChart>
                                    </ResponsiveContainer>
                                    <div className="w-1/2 space-y-2 pr-4">
                                        {analytics.sourceBreakdown.map((s, idx) => (
                                            <div key={s.source} className="flex items-center justify-between text-xs">
                                                <div className="flex items-center gap-2">
                                                    <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: PIE_COLORS[idx % PIE_COLORS.length] }}></span>
                                                    <span className="text-gray-600 font-medium">{s.source}</span>
                                                </div>
                                                <span className="font-bold text-gray-800">{s.count}</span>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            ) : (
                                <div className="h-64 flex items-center justify-center text-sm text-gray-400">No source data available</div>
                            )}
                        </div>
                    </div>
                </div>
            )}

            {/* ========================================================== */}
            {/* 2. SALES MANAGER DASHBOARD VIEW */}
            {/* ========================================================== */}
            {roleName === "Sales Manager" && (
                <div className="space-y-8">
                    {/* Stat Cards */}
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Team Leads</span>
                                <div className="w-8 h-8 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center">
                                    <Layers className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.teamLeadsCount ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Leads acquired by your team</div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Open Pool Leads</span>
                                <div className="w-8 h-8 rounded-lg bg-amber-50 text-amber-600 flex items-center justify-center">
                                    <Clock className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-amber-600">{analytics?.openPoolLeads ?? 0}</div>
                            <div className="text-xs text-amber-700 font-medium mt-1">Ready to be acquired & assigned</div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Team Pipeline</span>
                                <div className="w-8 h-8 rounded-lg bg-emerald-50 text-emerald-600 flex items-center justify-center">
                                    <DollarSign className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">
                                ${(analytics?.teamPipelineValue ?? 0).toLocaleString()}
                            </div>
                            <div className="text-xs text-emerald-600 mt-1">
                                {analytics?.convertedLeads ?? 0} Closed Won deals
                            </div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Active Reps</span>
                                <div className="w-8 h-8 rounded-lg bg-purple-50 text-purple-600 flex items-center justify-center">
                                    <Users className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.activeRepsCount ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Representatives reporting to you</div>
                        </div>
                    </div>

                    {/* Open Pool Preview */}
                    <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                        <div className="px-6 py-4 border-b border-gray-100 flex items-center justify-between bg-slate-50/50">
                            <div>
                                <h3 className="font-semibold text-gray-900">Open Leads Pool Available for Acquisition</h3>
                                <p className="text-xs text-gray-500">Claim these inbound leads to allocate to your representatives</p>
                            </div>
                            <Link to="/pool" className="text-xs font-semibold text-blue-600 hover:underline">
                                View Full Pool &rarr;
                            </Link>
                        </div>
                        {openPoolLeads && openPoolLeads.length > 0 ? (
                            <div className="divide-y divide-gray-100">
                                {openPoolLeads.slice(0, 5).map((opLead) => (
                                    <div key={opLead.id} className="p-4 sm:px-6 flex flex-col sm:flex-row sm:items-center justify-between gap-3 hover:bg-gray-50/50">
                                        <div className="space-y-1">
                                            <div className="flex items-center gap-2">
                                                <span className="font-semibold text-gray-900 text-sm">{opLead.title}</span>
                                                <span className="text-xs px-2 py-0.5 rounded-full bg-blue-50 text-blue-700 border border-blue-200 font-medium">
                                                    {opLead.source}
                                                </span>
                                            </div>
                                            <p className="text-xs text-gray-500">
                                                Contact: <span className="font-medium text-gray-700">{opLead.contactName || "Direct Inquiry"}</span>
                                                {opLead.contactEmail && ` • ${opLead.contactEmail}`}
                                                {opLead.estimatedValue ? ` • Est. Budget: $${opLead.estimatedValue.toLocaleString()}` : ""}
                                            </p>
                                        </div>
                                        <button
                                            onClick={() => acquireMutation.mutate(opLead.id)}
                                            disabled={acquireMutation.isPending}
                                            className="px-3.5 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-semibold rounded-lg shadow-xs transition shrink-0 cursor-pointer"
                                        >
                                            Acquire Lead
                                        </button>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="p-8 text-center text-sm text-gray-400">
                                No open leads currently in the pool.
                            </div>
                        )}
                    </div>

                    {/* Team Rep Performance */}
                    <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                        <div className="px-6 py-4 border-b border-gray-100">
                            <h3 className="font-semibold text-gray-900">Team Workload & Representatives</h3>
                            <p className="text-xs text-gray-500">Monitor deal allocation across your sales representatives</p>
                        </div>
                        {analytics?.repPerformance && analytics.repPerformance.length > 0 ? (
                            <table className="min-w-full divide-y divide-gray-200 text-sm">
                                <thead className="bg-gray-50">
                                    <tr>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Representative</th>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Assigned Leads</th>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Pipeline Value</th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white divide-y divide-gray-100">
                                    {analytics.repPerformance.map((rep) => (
                                        <tr key={rep.repId} className="hover:bg-gray-50">
                                            <td className="px-6 py-4 font-medium text-gray-900">{rep.repName}</td>
                                            <td className="px-6 py-4 text-gray-700 font-semibold">{rep.leadCount}</td>
                                            <td className="px-6 py-4 text-emerald-600 font-semibold">${(rep.totalValue ?? 0).toLocaleString()}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <div className="p-8 text-center text-sm text-gray-400">
                                No active sales representatives assigned to your team yet.
                            </div>
                        )}
                    </div>
                </div>
            )}

            {/* ========================================================== */}
            {/* 3. SALES REPRESENTATIVE DASHBOARD VIEW */}
            {/* ========================================================== */}
            {roleName === "Sales Rep" && (
                <div className="space-y-8">
                    {/* Stat Cards */}
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">My Leads</span>
                                <div className="w-8 h-8 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center">
                                    <Layers className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.myLeadsCount ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Opportunities assigned to you</div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">My Pipeline</span>
                                <div className="w-8 h-8 rounded-lg bg-emerald-50 text-emerald-600 flex items-center justify-center">
                                    <DollarSign className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">
                                ${(analytics?.myPipelineValue ?? 0).toLocaleString()}
                            </div>
                            <div className="text-xs text-emerald-600 font-medium mt-1">
                                {analytics?.convertedLeads ?? 0} Closed Won deals
                            </div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Pending Follow-ups</span>
                                <div className="w-8 h-8 rounded-lg bg-amber-50 text-amber-600 flex items-center justify-center">
                                    <Calendar className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-amber-600">{analytics?.pendingFollowUpsCount ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Tasks scheduled for action</div>
                        </div>

                        <div className="bg-white p-5 rounded-xl border border-gray-200 shadow-xs">
                            <div className="flex items-center justify-between text-gray-500 mb-2">
                                <span className="text-xs font-semibold uppercase tracking-wider">Touchpoints Logged</span>
                                <div className="w-8 h-8 rounded-lg bg-purple-50 text-purple-600 flex items-center justify-center">
                                    <Activity className="w-4 h-4" />
                                </div>
                            </div>
                            <div className="text-2xl font-bold text-gray-900">{analytics?.completedActivitiesCount ?? 0}</div>
                            <div className="text-xs text-gray-500 mt-1">Calls, meetings, & demos</div>
                        </div>
                    </div>

                    {/* Upcoming Follow-ups Widget */}
                    <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                        <div className="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
                            <div>
                                <h3 className="font-semibold text-gray-900">Upcoming Follow-ups & Tasks</h3>
                                <p className="text-xs text-gray-500">Action items to keep client communications moving forward</p>
                            </div>
                            <Link to="/followups" className="text-xs font-semibold text-blue-600 hover:underline">
                                View All &rarr;
                            </Link>
                        </div>
                        {myFollowUps && myFollowUps.length > 0 ? (
                            <div className="divide-y divide-gray-100">
                                {myFollowUps.slice(0, 5).map((fu) => (
                                    <div key={fu.id} className="p-4 sm:px-6 flex items-center justify-between gap-3 hover:bg-gray-50/50">
                                        <div className="flex items-start gap-3">
                                            <button
                                                onClick={() => completeFollowUpMutation.mutate(fu.id)}
                                                className="w-5 h-5 mt-0.5 rounded-full border border-gray-300 hover:border-emerald-500 hover:bg-emerald-50 text-white hover:text-emerald-600 flex items-center justify-center transition"
                                                title="Mark as completed"
                                            >
                                                <Check className="w-3.5 h-3.5 opacity-0 hover:opacity-100" />
                                            </button>
                                            <div>
                                                <p className="font-semibold text-gray-900 text-sm">{fu.title}</p>
                                                <p className="text-xs text-gray-500 mt-0.5">
                                                    Due: <span className="font-medium text-gray-700">{new Date(fu.dueDate).toLocaleDateString()}</span>
                                                </p>
                                            </div>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full uppercase ${
                                                fu.priority === "High" ? "bg-red-50 text-red-700 border border-red-200" :
                                                fu.priority === "Medium" ? "bg-amber-50 text-amber-700 border border-amber-200" :
                                                "bg-slate-50 text-slate-700 border border-slate-200"
                                            }`}>
                                                {fu.priority}
                                            </span>
                                            <Link
                                                to={`/leads/${fu.leadId}`}
                                                className="text-xs text-blue-600 hover:underline font-medium"
                                            >
                                                Open Lead
                                            </Link>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="p-8 text-center text-sm text-gray-400">
                                All caught up! No pending follow-ups due right now.
                            </div>
                        )}
                    </div>
                </div>
            )}

            {selectedPendingUser && <UserAccessModal selectedUser={selectedPendingUser} setSelectedUser={setSelectedPendingUser}/>}
        </div>
    );
};