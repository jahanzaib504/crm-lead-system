import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { getLeadsPaged, acquireLead } from "../services/leadService";
import { useUser } from "../store/userStore";
import { Link } from "react-router";
import toast from "react-hot-toast";
import { 
    Search, 
    Layers, 
    User, 
    Building2, 
    PlusCircle
} from "lucide-react";
import { usePagination } from "../hooks/usePagination";
import type { Lead } from "../types/Lead";
import { getAuditLogsPaged } from "../services/auditLogsService";

const STAGES = [
    { id: 0, name: "All Stages" },
    { id: 1, name: "New" },
    { id: 2, name: "Contacted" },
    { id: 3, name: "Qualified" },
    { id: 4, name: "Proposal" },
    { id: 5, name: "Closed Won" },
    { id: 6, name: "Closed Lost" },
];

const STAGE_BADGES: Record<string, string> = {
    "New": "bg-blue-50 text-blue-700 border-blue-200",
    "Contacted": "bg-amber-50 text-amber-700 border-amber-200",
    "Qualified": "bg-purple-50 text-purple-700 border-purple-200",
    "Proposal": "bg-indigo-50 text-indigo-700 border-indigo-200",
    "Closed Won": "bg-emerald-50 text-emerald-700 border-emerald-200",
    "Closed Lost": "bg-red-50 text-red-700 border-red-200",
};

interface LeadsPageProps {
    openPoolOnly?: boolean;
}

export const LeadsPage = ({ openPoolOnly = false }: LeadsPageProps) => {
    
    const [searchTerm, setSearchTerm] = useState("");
    const [selectedStage, setSelectedStage] = useState(0);
    const [poolFilter, setPoolFilter] = useState<boolean | undefined>(openPoolOnly ? true : undefined);

    const currentUser = useUser((state) => state.user);
    const roleName = currentUser?.role?.name;
    const queryClient = useQueryClient();

    const { data, isPending, isError, totalCount, nextButton, prevButton, pageSize, page } = usePagination<Lead>({
    keys: ["leads", searchTerm, selectedStage, poolFilter],
    fn: (page, pageSize) => getLeadsPaged(page, pageSize, searchTerm, selectedStage, openPoolOnly) // Pass page and pageSize correctly
})

    const acquireMutation = useMutation({
        mutationFn: (leadId: number) => acquireLead(leadId),
        onSuccess: () => {
            toast.success("Lead claimed from open pool!");
            queryClient.invalidateQueries({ queryKey: ["leads"] });
            queryClient.invalidateQueries({ queryKey: ["open-pool-leads"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        },
        onError: (err: any) => {
            toast.error(err.response?.data?.detail || "Failed to acquire lead.");
        }
    });

    const leads = data?.items ?? [];
    const totalPages = Math.ceil(totalCount / pageSize) || 1;

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 border-b border-gray-200">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 tracking-tight">
                        {openPoolOnly ? "Open Leads Pool" : "Leads Directory"}
                    </h1>
                    <p className="text-sm text-gray-500 mt-1">
                        {openPoolOnly 
                            ? "Unassigned inbound opportunities ready for manager acquisition and allocation" 
                            : "Browse, filter, and track sales opportunities across your pipeline"}
                    </p>
                </div>

                <div className="flex items-center gap-2">
                    <Link
                        to="/create-lead"
                        className="inline-flex items-center gap-1.5 px-3.5 py-2 text-xs font-semibold rounded-lg bg-blue-600 text-white hover:bg-blue-700 shadow-xs transition"
                    >
                        <PlusCircle className="w-3.5 h-3.5" />
                        Create New Lead
                    </Link>
                </div>
            </div>

            {/* Filter Bar */}
            <div className="bg-white p-4 rounded-xl border border-gray-200 shadow-xs flex flex-col md:flex-row gap-4 items-center justify-between">
                <div className="flex flex-1 flex-col sm:flex-row gap-3 w-full">
                    {/* Search */}
                    <div className="relative flex-1">
                        <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                        <input
                            type="text"
                            value={searchTerm}
                            onChange={(e) => {
                                setSearchTerm(e.target.value);
                                
                            }}
                            placeholder="Search by title, contact, company..."
                            className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg outline-none focus:border-blue-500"
                        />
                    </div>

                    {/* Stage Filter */}
                    <div className="sm:w-48">
                        <select
                            value={selectedStage}
                            onChange={(e) => {
                                setSelectedStage(Number(e.target.value));
                               
                            }}
                            className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg outline-none focus:border-blue-500 text-gray-700"
                        >
                            {STAGES.map((s) => (
                                <option key={s.id} value={s.id}>{s.name}</option>
                            ))}
                        </select>
                    </div>

                    {/* Pool filter toggle (if on main leads page and manager/admin) */}
                    {!openPoolOnly && (roleName === "Sales Manager" || roleName === "Admin") && (
                        <div className="flex items-center rounded-lg border border-gray-200 p-0.5 bg-slate-50 text-xs font-medium">
                            <button
                                type="button"
                                onClick={() => { setPoolFilter(undefined);  }}
                                className={`px-2.5 py-1.5 rounded-md transition ${poolFilter === undefined ? "bg-white text-blue-600 font-semibold shadow-xs" : "text-gray-500 hover:text-gray-800"}`}
                            >
                                All
                            </button>
                            <button
                                type="button"
                                onClick={() => { setPoolFilter(false);  }}
                                className={`px-2.5 py-1.5 rounded-md transition ${poolFilter === false ? "bg-white text-blue-600 font-semibold shadow-xs" : "text-gray-500 hover:text-gray-800"}`}
                            >
                                Assigned
                            </button>
                            <button
                                type="button"
                                onClick={() => { setPoolFilter(true); }}
                                className={`px-2.5 py-1.5 rounded-md transition ${poolFilter === true ? "bg-white text-blue-600 font-semibold shadow-xs" : "text-gray-500 hover:text-gray-800"}`}
                            >
                                Open Pool
                            </button>
                        </div>
                    )}
                </div>

                <span className="text-xs font-medium text-gray-500 whitespace-nowrap self-end md:self-center">
                    Showing <span className="font-bold text-gray-800">{totalCount}</span> results
                </span>
            </div>

            {/* Error Banner */}
            {isError && (
                <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-sm">
                    Failed to load leads directory. Please try again.
                </div>
            )}

            {/* Loading Skeleton */}
            {isPending && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {[...Array(4)].map((_, i) => (
                        <div key={i} className="p-5 border border-gray-200 rounded-xl bg-white shadow-xs animate-pulse space-y-3">
                            <div className="h-5 bg-gray-200 rounded w-1/3"></div>
                            <div className="h-4 bg-gray-100 rounded w-3/4"></div>
                            <div className="h-4 bg-gray-100 rounded w-1/2"></div>
                        </div>
                    ))}
                </div>
            )}

            {/* Leads Grid */}
            {!isPending && !isError && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {leads.length > 0 ? (
                        leads.map((lead:any) => {
                            const isOpenLead = lead.managerId == null && lead.assignedToUserId == null;
                            const canClaim = isOpenLead && (roleName === "Sales Manager" || roleName === "Admin");

                            return (
                                <div
                                    key={lead.id}
                                    className="bg-white border border-gray-200 rounded-xl p-5 shadow-xs hover:shadow-md transition duration-200 flex flex-col justify-between space-y-4"
                                >
                                    <div className="space-y-2.5">
                                        {/* Title & Stage */}
                                        <div className="flex items-start justify-between gap-3">
                                            <Link
                                                to={`/leads/${lead.id}`}
                                                className="font-bold text-gray-900 text-base hover:text-blue-600 transition leading-snug"
                                            >
                                                {lead.title}
                                            </Link>
                                            <span className={`px-2.5 py-0.5 rounded-full text-xs font-semibold border whitespace-nowrap ${
                                                STAGE_BADGES[lead.leadStage?.name || ""] || "bg-gray-100 text-gray-700 border-gray-200"
                                            }`}>
                                                {lead.leadStage?.name || "New"}
                                            </span>
                                        </div>

                                        {/* Query Snippet */}
                                        {lead.query && (
                                            <p className="text-xs text-gray-600 line-clamp-2 leading-relaxed bg-slate-50 p-2.5 rounded-lg border border-slate-100">
                                                {lead.query}
                                            </p>
                                        )}

                                        {/* Contact & Company Info */}
                                        <div className="grid grid-cols-2 gap-2 text-xs pt-1">
                                            <div className="flex items-center gap-1.5 text-gray-600">
                                                <User className="w-3.5 h-3.5 text-gray-400 shrink-0" />
                                                <span className="truncate">{lead.contactName || "Direct Contact"}</span>
                                            </div>
                                            <div className="flex items-center gap-1.5 text-gray-600">
                                                <Building2 className="w-3.5 h-3.5 text-gray-400 shrink-0" />
                                                <span className="truncate">{lead.companyName || "Organization N/A"}</span>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Footer Details & Action */}
                                    <div className="pt-3 border-t border-gray-100 flex items-center justify-between gap-3 text-xs">
                                        <div className="space-y-0.5">
                                            <span className="text-gray-400 block text-[10px] uppercase font-semibold">Budget / Score</span>
                                            <div className="flex items-center gap-2">
                                                <span className="font-bold text-gray-900">
                                                    {lead.estimatedValue ? `$${lead.estimatedValue.toLocaleString()}` : "N/A"}
                                                </span>
                                                <span className="text-gray-300">•</span>
                                                <span className="font-semibold text-blue-600">
                                                    Score: {lead.currentScore ?? 0}
                                                </span>
                                            </div>
                                        </div>

                                        <div className="flex items-center gap-2">
                                            {canClaim && (
                                                <button
                                                    onClick={() => acquireMutation.mutate(lead.id)}
                                                    disabled={acquireMutation.isPending}
                                                    className="px-3 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-semibold shadow-xs transition cursor-pointer"
                                                >
                                                    Claim Lead
                                                </button>
                                            )}
                                            <Link
                                                to={`/leads/${lead.id}`}
                                                className="px-3 py-1.5 bg-gray-100 hover:bg-gray-200 text-gray-800 rounded-lg text-xs font-semibold transition"
                                            >
                                                Details &rarr;
                                            </Link>
                                        </div>
                                    </div>
                                </div>
                            );
                        })
                    ) : (
                        <div className="col-span-full py-16 text-center bg-white rounded-xl border border-dashed border-gray-300 text-gray-500 space-y-2">
                            <Layers className="w-8 h-8 mx-auto text-gray-300" />
                            <p className="font-semibold text-gray-700">No leads found matching your criteria.</p>
                            <p className="text-xs text-gray-400">Try adjusting your search filters or create a new lead.</p>
                        </div>
                    )}
                </div>
            )}

            {/* Pagination Controls */}
            {totalPages > 1 && (
                <div className="flex items-center justify-between border-t border-gray-200 pt-4">
                    <span className="text-xs text-gray-500">
                        Page <span className="font-bold text-gray-800">{page}</span> of{" "}
                        <span className="font-bold text-gray-800">{totalPages}</span>
                    </span>

                    <div className="flex gap-2">
                        <div
                        
                            className="px-3 py-1.5 text-xs font-semibold border rounded-lg bg-white text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
                        >
                            {prevButton}
                        </div>
                        <div
                            
                            className="px-3 py-1.5 text-xs font-semibold border rounded-lg bg-white text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
                        >
                            {nextButton}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};