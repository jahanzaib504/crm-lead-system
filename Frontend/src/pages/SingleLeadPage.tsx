import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useParams, Link } from "react-router";
import { getLead, updateLeadStage, assignLead, acquireLead } from "../services/leadService";
import { getNotesForLead, createNote } from "../services/noteService";
import { getFollowUpsByLead, createFollowUp, updateFollowUpStatus } from "../services/followUpService";
import { getActivitiesByLead, createActivity } from "../services/activityService";
import { getSalesReps, getManagers } from "../services/userService";
import { useUser } from "../store/userStore";
import { useState } from "react";
import toast from "react-hot-toast";
import {
    Mail,
    Phone,
    Briefcase,
    Building2,
    User,
    ArrowLeft,
    Plus,
    MessageSquare,
    CheckSquare,
    Activity as ActivityIcon,
    PhoneCall,
    Users as UsersIcon,
    Video,
    FileText,
    Check,
    X,
    UserPlus,
    Loader,
    LoaderCircle
} from "lucide-react";
import { getCurrentDate, getCurrentDateTime } from "../utils/timeManagement";

const STAGES = [
    { id: 1, name: "New", color: "bg-blue-50 text-blue-700 border-blue-200" },
    { id: 2, name: "Contacted", color: "bg-amber-50 text-amber-700 border-amber-200" },
    { id: 3, name: "Qualified", color: "bg-purple-50 text-purple-700 border-purple-200" },
    { id: 4, name: "Proposal", color: "bg-indigo-50 text-indigo-700 border-indigo-200" },
    { id: 5, name: "Closed Won", color: "bg-emerald-50 text-emerald-700 border-emerald-200" },
    { id: 6, name: "Closed Lost", color: "bg-red-50 text-red-700 border-red-200" },
];

export const SingleLeadPage = () => {
    const { id } = useParams<{ id: string }>();
    const leadId = Number(id);
    const queryClient = useQueryClient();
    const currentUser = useUser((state) => state.user);
    const roleName = currentUser?.role?.name;

    const [activeTab, setActiveTab] = useState<"notes" | "followups" | "activities">("notes");

    // Modal States
    const [isAssignModalOpen, setIsAssignModalOpen] = useState(false);
    const [selectedRepId, setSelectedRepId] = useState<number | "">("");

    const [isFollowUpModalOpen, setIsFollowUpModalOpen] = useState(false);
    const [followUpForm, setFollowUpForm] = useState({ title: "", dueDate: "", priority: "Medium" });

    const [isActivityModalOpen, setIsActivityModalOpen] = useState(false);
    const [activityForm, setActivityForm] = useState({ activityType: "Call", subject: "", summary: "", activityDate: new Date().toISOString().slice(0, 16) });

    const [noteContent, setNoteContent] = useState("");
    const [updatingStage, setUpdatingStage] = useState<number | null>(null);
    // Queries
    const { data: lead, isPending: leadLoading, isError: leadError } = useQuery({
        queryKey: ["lead", leadId],
        queryFn: () => getLead(leadId),
        enabled: !isNaN(leadId),
    });

    const { data: notes } = useQuery({
        queryKey: ["lead-notes", leadId],
        queryFn: () => getNotesForLead(leadId),
        enabled: !isNaN(leadId),
    });

    const { data: followUps } = useQuery({
        queryKey: ["lead-followups", leadId],
        queryFn: () => getFollowUpsByLead(leadId),
        enabled: !isNaN(leadId),
    });

    const { data: activities } = useQuery({
        queryKey: ["lead-activities", leadId],
        queryFn: () => getActivitiesByLead(leadId),
        enabled: !isNaN(leadId),
    });

    const { data: salesReps } = useQuery({
        queryKey: ["sales-reps"],
        queryFn: getSalesReps,
        enabled: isAssignModalOpen && roleName === "Sales Manager",
    });

    const { data: managers } = useQuery({
        queryKey: ["managers-list"],
        queryFn: getManagers,
        enabled: isAssignModalOpen && roleName === "Admin",
    });

    // Mutations
    const acquireMutation = useMutation({
        mutationFn: () => acquireLead(leadId),
        onSuccess: () => {
            toast.success("Lead acquired successfully!");
            queryClient.invalidateQueries({ queryKey: ["lead", leadId] });
            queryClient.invalidateQueries({ queryKey: ["leads"] });
            queryClient.invalidateQueries({ queryKey: ["open-pool-leads"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        },
        onError: (err: any) => toast.error(err.response?.data?.detail || "Failed to acquire lead.")
    });

    const stageMutation = useMutation({
        mutationFn: (newStageId: number) => updateLeadStage(leadId, newStageId),
        onSuccess: () => {
            toast.success("Lead stage updated!");
            queryClient.invalidateQueries({ queryKey: ["lead", leadId] });
            queryClient.invalidateQueries({ queryKey: ["leads"] });
        },
        onError: () => toast.error("Failed to update stage."),
        onSettled: () => setUpdatingStage(null)
    });

    const assignMutation = useMutation({
        mutationFn: (userId: number) => assignLead(leadId, userId),
        onSuccess: () => {
            toast.success(roleName === "Admin" ? "Lead assigned to manager successfully!" : "Lead assigned to representative successfully!");
            queryClient.invalidateQueries({ queryKey: ["lead", leadId] });
            queryClient.invalidateQueries({ queryKey: ["leads"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
            setIsAssignModalOpen(false);
        },
        onError: (err: any) => toast.error(err.response?.data?.detail || "Failed to assign lead.")
    });

    const addNoteMutation = useMutation({
        mutationFn: (content: string) => createNote({ leadId, content }),
        onSuccess: () => {
            toast.success("Note added!");
            setNoteContent("");
            queryClient.invalidateQueries({ queryKey: ["lead-notes", leadId] });
        },
        onError: () => toast.error("Failed to add note.")
    });

    const addFollowUpMutation = useMutation({
        mutationFn: (payload: { title: string; dueDate: string; priority: string }) =>
            createFollowUp({ leadId, ...payload }),
        onSuccess: () => {
            toast.success("Follow-up scheduled!");
            setIsFollowUpModalOpen(false);
            setFollowUpForm({ title: "", dueDate: "", priority: "Medium" });
            queryClient.invalidateQueries({ queryKey: ["lead-followups", leadId] });
        },
        onError: () => toast.error("Failed to schedule follow-up.")
    });

    const toggleFollowUpMutation = useMutation({
        mutationFn: ({ id, status }: { id: number; status: string }) =>
            updateFollowUpStatus(id, status),
        onSuccess: () => {
            toast.success("Follow-up status updated!");
            queryClient.invalidateQueries({ queryKey: ["lead-followups", leadId] });
        }
    });

    const addActivityMutation = useMutation({
        mutationFn: (payload: any) => createActivity({ leadId, ...payload }),
        onSuccess: () => {
            toast.success("Activity logged!");
            setIsActivityModalOpen(false);
            setActivityForm({ activityType: "Call", subject: "", summary: "", activityDate: new Date().toISOString().slice(0, 16) });
            queryClient.invalidateQueries({ queryKey: ["lead-activities", leadId] });
        },
        onError: () => toast.error("Failed to log activity.")
    });

    if (leadLoading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    if (leadError || !lead) {
        return (
            <div className="p-8 text-center space-y-4">
                <div className="p-4 bg-red-50 text-red-700 rounded-xl max-w-md mx-auto border border-red-200 text-sm">
                    Unable to load lead details. You may not be authorized to view this lead, or it may not exist.
                </div>
                <Link to="/leads" className="text-blue-600 hover:underline text-sm font-medium">
                    &larr; Back to Leads Overview
                </Link>
            </div>
        );
    }

    const getActivityIcon = (type: string) => {
        switch (type) {
            case "Call": return <PhoneCall className="w-4 h-4 text-blue-500" />;
            case "Meeting": return <UsersIcon className="w-4 h-4 text-emerald-500" />;
            case "Demo": return <Video className="w-4 h-4 text-purple-500" />;
            case "Email": return <Mail className="w-4 h-4 text-amber-500" />;
            default: return <FileText className="w-4 h-4 text-gray-500" />;
        }
    };

    return (
        <div className="space-y-6">
            {/* Top Bar / Back button */}
            <div className="flex items-center justify-between">
                <Link
                    to="/leads"
                    className="inline-flex items-center gap-1.5 text-xs font-semibold text-gray-500 hover:text-gray-800 transition"
                >
                    <ArrowLeft className="w-4 h-4" /> Back to Leads
                </Link>

                <div className="flex items-center gap-2">
                    {lead.managerId == null && (roleName === "Sales Manager" || roleName === "Admin") && (
                        <button
                            onClick={() => acquireMutation.mutate()}
                            disabled={acquireMutation.isPending}
                            className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg text-xs font-semibold shadow-xs transition cursor-pointer"
                        >
                            <Check className="w-3.5 h-3.5" />
                            {acquireMutation.isPending ? "Acquiring..." : "Acquire Lead"}
                        </button>
                    )}

                    {roleName === "Admin" && (
                        <button
                            onClick={() => {
                                setSelectedRepId(lead.managerId || "");
                                setIsAssignModalOpen(true);
                            }}
                            className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-semibold shadow-xs transition cursor-pointer"
                        >
                            <UserPlus className="w-3.5 h-3.5" />
                            {lead.managerId ? "Reassign Manager" : "Assign to Manager"}
                        </button>
                    )}

                    {roleName === "Sales Manager" && lead.managerId === currentUser?.id && (
                        <button
                            onClick={() => {
                                setSelectedRepId(lead.assignedToUserId || "");
                                setIsAssignModalOpen(true);
                            }}
                            className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-semibold shadow-xs transition cursor-pointer"
                        >
                            <UserPlus className="w-3.5 h-3.5" />
                            {lead.assignedToUserId ? "Reassign Representative" : "Assign to Representative"}
                        </button>
                    )}
                </div>
            </div>

            {/* Lead Header Card */}
            <div className="bg-white rounded-2xl border border-gray-200 p-6 shadow-xs space-y-5">
                <div className="flex flex-col md:flex-row md:items-start md:justify-between gap-4">
                    <div>
                        <div className="flex items-center gap-3 flex-wrap">
                            <h1 className="text-2xl font-bold text-gray-900">{lead.title}</h1>
                            <span className={`px-3 py-0.5 rounded-full text-xs font-semibold border ${STAGES.find(s => s.id === lead.leadStageId)?.color || "bg-gray-100 text-gray-700"
                                }`}>
                                {lead.leadStage?.name || "New"}
                            </span>
                        </div>
                        <p className="text-sm text-gray-500 mt-1">
                            Source: <span className="font-semibold text-gray-700">{lead.source}</span>
                            {lead.createdAt && ` • Created on ${new Date(lead.createdAt).toLocaleDateString()}`}
                        </p>
                    </div>

                    <div className="flex items-center gap-6 bg-slate-50 px-4 py-2.5 rounded-xl border border-slate-100">
                        <div>
                            <span className="block text-[11px] font-semibold uppercase tracking-wider text-gray-400">Budget</span>
                            <span className="text-lg font-bold text-gray-900">
                                {lead.estimatedValue ? `Rs. ${lead.estimatedValue.toLocaleString()}` : "N/A"}
                            </span>
                        </div>
                        <div className="border-l border-gray-200 pl-6">
                            <span className="block text-[11px] font-semibold uppercase tracking-wider text-gray-400">Lead Score</span>
                            <span className="text-lg font-bold text-blue-600">
                                {lead.currentScore ?? 0}
                            </span>
                        </div>
                    </div>
                </div>

                {/* Query description */}
                {lead.query && (
                    <div className="p-3.5 rounded-xl bg-slate-50 border border-slate-100 text-sm text-slate-700 leading-relaxed">
                        <span className="font-semibold text-slate-900 block text-xs uppercase tracking-wider mb-1">Inquiry / Requirements:</span>
                        {lead.query}
                    </div>
                )}

                {/* Pipeline Stage Transition Stepper */}
                <div className="pt-2 border-t border-gray-100">
                    <span className="block text-xs font-semibold uppercase tracking-wider text-gray-400 mb-2">Move Lead Stage:</span>
                    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-6 gap-2">
                        {STAGES.map((stg) => {
                            const isCurrent = lead.leadStageId === stg.id;
                            return (
                                <button
                                    key={stg.id}
                                    onClick={() => {
                                        setUpdatingStage(stg.id);
                                        stageMutation.mutate(stg.id);
                                    }}
                                    disabled={isCurrent || stageMutation.isPending}
                                    className={`py-2 px-3 rounded-lg text-xs font-semibold transition border
                                    inline-flex items-center justify-center gap-2
                                    ${isCurrent
                                    ? "bg-slate-900 text-white border-slate-900 shadow-xs ring-2 ring-slate-200"
                                    : "bg-white text-gray-700 border-gray-200 hover:bg-gray-100 hover:border-gray-300 hover:cursor-pointer"
                                }
                                    ${updatingStage === stg.id ? "opacity-70 cursor-wait" : ""}
                                `}
                                >
                                    {stg.name}

                                    {updatingStage === stg.id && (
                                        <LoaderCircle className="h-4 w-4 animate-spin shrink-0" />
                                    )}
                                </button>
                            );
                        })}
                    </div>
                </div>
            </div>

            {/* Information Grid: Contact, Company, Assignment */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
                {/* Contact Card */}
                <div className="bg-white rounded-xl border border-gray-200 p-5 shadow-xs space-y-3">
                    <div className="flex items-center gap-2 text-gray-400 border-b border-gray-100 pb-2">
                        <User className="w-4 h-4 text-blue-600" />
                        <h3 className="text-xs font-bold uppercase tracking-wider text-gray-700">Contact Information</h3>
                    </div>
                    <div className="space-y-2 text-sm">
                        <p className="font-semibold text-gray-900 text-base">{lead.contactName || "Direct Inquiry"}</p>
                        {lead.contactJobTitle && (
                            <p className="text-xs text-gray-500 font-medium flex items-center gap-1.5">
                                <Briefcase className="w-3.5 h-3.5 text-gray-400" /> {lead.contactJobTitle}
                            </p>
                        )}
                        {lead.contactEmail && (
                            <p className="text-xs text-gray-600 flex items-center gap-1.5">
                                <Mail className="w-3.5 h-3.5 text-gray-400" />
                                <a href={`mailto:${lead.contactEmail}`} className="text-blue-600 hover:underline">
                                    {lead.contactEmail}
                                </a>
                            </p>
                        )}
                        {lead.contactPhone && (
                            <p className="text-xs text-gray-600 flex items-center gap-1.5">
                                <Phone className="w-3.5 h-3.5 text-gray-400" />
                                <a href={`tel:${lead.contactPhone}`} className="text-blue-600 hover:underline">
                                    {lead.contactPhone}
                                </a>
                            </p>
                        )}
                    </div>
                </div>

                {/* Company Card */}
                <div className="bg-white rounded-xl border border-gray-200 p-5 shadow-xs space-y-3">
                    <div className="flex items-center gap-2 text-gray-400 border-b border-gray-100 pb-2">
                        <Building2 className="w-4 h-4 text-emerald-600" />
                        <h3 className="text-xs font-bold uppercase tracking-wider text-gray-700">Organization</h3>
                    </div>
                    <div className="space-y-2 text-sm">
                        <p className="font-semibold text-gray-900 text-base">{lead.companyName || "Independent Client"}</p>
                        <p className="text-xs text-gray-500">Corporate client profile</p>
                    </div>
                </div>

                {/* Ownership Card */}
                <div className="bg-white rounded-xl border border-gray-200 p-5 shadow-xs space-y-3">
                    <div className="flex items-center gap-2 text-gray-400 border-b border-gray-100 pb-2">
                        <Briefcase className="w-4 h-4 text-indigo-600" />
                        <h3 className="text-xs font-bold uppercase tracking-wider text-gray-700">Ownership</h3>
                    </div>
                    <div className="space-y-2 text-xs">
                        <div>
                            <span className="text-gray-400 block font-medium">Assigned Representative:</span>
                            <span className="font-semibold text-gray-800 text-sm">
                                {lead.assignedToUserName || "Unassigned"}
                            </span>
                        </div>
                        <div>
                            <span className="text-gray-400 block font-medium">Managing Supervisor:</span>
                            <span className="font-semibold text-gray-800 text-sm">
                                {lead.managerName || "None (Open Pool)"}
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Interaction Tabs: Notes, Follow-ups, Activities */}
            <div className="bg-white rounded-2xl border border-gray-200 shadow-xs overflow-hidden">
                {/* Tab Navigation */}
                <div className="flex border-b border-gray-200 bg-slate-50/50 px-6 pt-2">
                    <button
                        onClick={() => setActiveTab("notes")}
                        className={`flex items-center gap-2 py-3 px-4 text-sm font-semibold border-b-2 transition ${activeTab === "notes"
                                ? "border-blue-600 text-blue-600"
                                : "border-transparent text-gray-500 hover:text-gray-800"
                            }`}
                    >
                        <MessageSquare className="w-4 h-4" />
                        Internal Notes ({notes?.length ?? 0})
                    </button>

                    <button
                        onClick={() => setActiveTab("followups")}
                        className={`flex items-center gap-2 py-3 px-4 text-sm font-semibold border-b-2 transition ${activeTab === "followups"
                                ? "border-blue-600 text-blue-600"
                                : "border-transparent text-gray-500 hover:text-gray-800"
                            }`}
                    >
                        <CheckSquare className="w-4 h-4" />
                        Follow-ups & Tasks ({followUps?.length ?? 0})
                    </button>

                    <button
                        onClick={() => setActiveTab("activities")}
                        className={`flex items-center gap-2 py-3 px-4 text-sm font-semibold border-b-2 transition ${activeTab === "activities"
                                ? "border-blue-600 text-blue-600"
                                : "border-transparent text-gray-500 hover:text-gray-800"
                            }`}
                    >
                        <ActivityIcon className="w-4 h-4" />
                        Activity Log ({activities?.length ?? 0})
                    </button>
                </div>

                <div className="p-6">
                    {/* TAB 1: NOTES */}
                    {activeTab === "notes" && (
                        <div className="space-y-6">
                            {/* Add Note Form */}
                            <form
                                onSubmit={(e) => {
                                    e.preventDefault();
                                    if (noteContent.trim()) addNoteMutation.mutate(noteContent);
                                }}
                                className="space-y-3"
                            >
                                <textarea
                                    value={noteContent}
                                    onChange={(e) => setNoteContent(e.target.value)}
                                    placeholder="Add notes about your conversation with the client, customer needs, or deal progress..."
                                    rows={3}
                                    className="w-full p-3.5 border border-gray-200 rounded-xl text-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 transition resize-none"
                                />
                                <div className="flex justify-end">
                                    <button
                                        type="submit"
                                        disabled={addNoteMutation.isPending || !noteContent.trim()}
                                        className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold transition disabled:opacity-50 cursor-pointer"
                                    >
                                        {addNoteMutation.isPending ? "Posting..." : "Post Note"}
                                    </button>
                                </div>
                            </form>

                            {/* Notes List */}
                            <div className="space-y-3 pt-2">
                                {notes && notes.length > 0 ? (
                                    notes.map((nt) => (
                                        <div key={nt.id} className="p-4 bg-slate-50 rounded-xl border border-slate-100 space-y-1.5">
                                            <div className="flex items-center justify-between text-xs text-gray-400">
                                                <span className="font-semibold text-gray-800">{nt.createdBy}</span>
                                                <span>{new Date(nt.createdAt).toLocaleString()}</span>
                                            </div>
                                            <p className="text-sm text-gray-700 whitespace-pre-wrap">{nt.content}</p>
                                        </div>
                                    ))
                                ) : (
                                    <p className="text-center py-6 text-xs text-gray-400">
                                        No notes recorded for this lead yet.
                                    </p>
                                )}
                            </div>
                        </div>
                    )}

                    {/* TAB 2: FOLLOW-UPS */}
                    {activeTab === "followups" && (
                        <div className="space-y-4">
                            <div className="flex justify-between items-center pb-2">
                                <span className="text-xs text-gray-500 font-medium">Keep up momentum with scheduled tasks</span>
                                <button
                                    onClick={() => setIsFollowUpModalOpen(true)}
                                    className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold shadow-xs transition"
                                >
                                    <Plus className="w-3.5 h-3.5" /> Schedule Follow-up
                                </button>
                            </div>

                            {followUps && followUps.length > 0 ? (
                                <div className="divide-y divide-gray-100">
                                    {followUps.map((fu) => (
                                        <div key={fu.id} className="py-3.5 flex items-center justify-between gap-3">
                                            <div className="flex items-start gap-3">
                                                <button
                                                    onClick={() => toggleFollowUpMutation.mutate({
                                                        id: fu.id,
                                                        status: fu.status === "Completed" ? "Pending" : "Completed"
                                                    })}
                                                    className={`w-5 h-5 mt-0.5 rounded-full border flex items-center justify-center transition cursor-pointer ${fu.status === "Completed"
                                                            ? "bg-emerald-600 border-emerald-600 text-white"
                                                            : "border-gray-300 hover:border-emerald-500 text-transparent"
                                                        }`}
                                                >
                                                    <Check className="w-3.5 h-3.5" />
                                                </button>
                                                <div>
                                                    <p className={`font-semibold text-sm ${fu.status === "Completed" ? "line-through text-gray-400" : "text-gray-900"}`}>
                                                        {fu.title}
                                                    </p>
                                                    <p className="text-xs text-gray-500">
                                                        Due: <span className="font-medium text-gray-700">{new Date(fu.dueDate).toLocaleDateString()}</span>
                                                        {fu.assignedToUserName && ` • Assigned to ${fu.assignedToUserName}`}
                                                    </p>
                                                </div>
                                            </div>

                                            <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full uppercase ${fu.priority === "High" ? "bg-red-50 text-red-700 border border-red-200" :
                                                    fu.priority === "Medium" ? "bg-amber-50 text-amber-700 border border-amber-200" :
                                                        "bg-slate-50 text-slate-700 border border-slate-200"
                                                }`}>
                                                {fu.priority}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-center py-8 text-xs text-gray-400">
                                    No follow-ups currently scheduled for this lead.
                                </p>
                            )}
                        </div>
                    )}

                    {/* TAB 3: ACTIVITIES */}
                    {activeTab === "activities" && (
                        <div className="space-y-4">
                            <div className="flex justify-between items-center pb-2">
                                <span className="text-xs text-gray-500 font-medium">Record meetings, client calls, emails, and demos</span>
                                <button
                                    onClick={() => setIsActivityModalOpen(true)}
                                    className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold shadow-xs transition"
                                >
                                    <Plus className="w-3.5 h-3.5" /> Log Activity
                                </button>
                            </div>

                            {activities && activities.length > 0 ? (
                                <div className="space-y-3">
                                    {activities.map((act) => (
                                        <div key={act.id} className="p-4 bg-slate-50 rounded-xl border border-slate-100 flex items-start gap-3">
                                            <div className="p-2 bg-white rounded-lg border border-gray-200 shrink-0 mt-0.5">
                                                {getActivityIcon(act.activityType)}
                                            </div>
                                            <div className="flex-1 space-y-1">
                                                <div className="flex items-center justify-between text-xs text-gray-400">
                                                    <span className="font-semibold text-gray-800 text-sm">{act.subject}</span>
                                                    <span>{new Date(act.activityDate).toLocaleDateString()}</span>
                                                </div>
                                                <span className="inline-block text-[10px] font-semibold px-2 py-0.2 rounded-full bg-blue-50 text-blue-700 border border-blue-200 uppercase">
                                                    {act.activityType}
                                                </span>
                                                {act.summary && <p className="text-xs text-gray-600 pt-1 leading-relaxed">{act.summary}</p>}
                                                <p className="text-[11px] text-gray-400 pt-1">Logged by: {act.loggedByUserName || "Sales Staff"}</p>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-center py-8 text-xs text-gray-400">
                                    No client interactions logged yet.
                                </p>
                            )}
                        </div>
                    )}
                </div>
            </div>

            {/* ========================================================== */}
            {/* ASSIGN MODAL (MANAGER FOR ADMIN, REP FOR SALES MANAGER) */}
            {/* ========================================================== */}
            {isAssignModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4">
                    <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl border border-gray-100">
                        <div className="flex items-center justify-between pb-4 border-b border-gray-100">
                            <h3 className="text-base font-bold text-gray-900">
                                {roleName === "Admin" ? "Assign Lead to Sales Manager" : "Assign Lead to Representative"}
                            </h3>
                            <button onClick={() => setIsAssignModalOpen(false)} className="text-gray-400 hover:text-gray-600">
                                <X className="w-5 h-5" />
                            </button>
                        </div>

                        <div className="py-4 space-y-4">
                            <p className="text-xs text-gray-600">
                                {roleName === "Admin"
                                    ? "Assigning this lead to a sales manager will allow the manager to review and allocate it to a sales representative on their team."
                                    : "Assigning this lead will allow the representative on your team to talk with the client and log follow-ups, notes, and activities."}
                            </p>

                            <div>
                                <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                    {roleName === "Admin" ? "Select Sales Manager" : "Select Sales Representative"}
                                </label>
                                {roleName === "Admin" ? (
                                    <select
                                        value={selectedRepId}
                                        onChange={(e) => setSelectedRepId(Number(e.target.value))}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                    >
                                        <option value="">-- Choose Sales Manager --</option>
                                        {managers?.map((mgr) => (
                                            <option key={mgr.id} value={mgr.id}>
                                                {mgr.fullName} ({mgr.email})
                                            </option>
                                        ))}
                                    </select>
                                ) : (
                                    <select
                                        value={selectedRepId}
                                        onChange={(e) => setSelectedRepId(Number(e.target.value))}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                    >
                                        <option value="">-- Choose Sales Rep --</option>
                                        {salesReps?.map((rep) => (
                                            <option key={rep.id} value={rep.id}>
                                                {rep.fullName} ({rep.email})
                                            </option>
                                        ))}
                                    </select>
                                )}
                            </div>
                        </div>

                        <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
                            <button
                                onClick={() => setIsAssignModalOpen(false)}
                                className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 cursor-pointer"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={() => selectedRepId && assignMutation.mutate(Number(selectedRepId))}
                                disabled={!selectedRepId || assignMutation.isPending}
                                className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition disabled:opacity-50 cursor-pointer"
                            >
                                {assignMutation.isPending
                                    ? "Assigning..."
                                    : roleName === "Admin"
                                        ? "Assign to Manager"
                                        : "Assign to Representative"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* ========================================================== */}
            {/* SCHEDULE FOLLOW-UP MODAL */}
            {/* ========================================================== */}
            {isFollowUpModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4">
                    <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl border border-gray-100">
                        <div className="flex items-center justify-between pb-4 border-b border-gray-100">
                            <h3 className="text-base font-bold text-gray-900">Schedule Client Follow-up</h3>
                            <button onClick={() => setIsFollowUpModalOpen(false)} className="text-gray-400 hover:text-gray-600">
                                <X className="w-5 h-5" />
                            </button>
                        </div>

                        <form
                            onSubmit={(e) => {
                                e.preventDefault();
                                if (followUpForm.title && followUpForm.dueDate) {
                                    addFollowUpMutation.mutate(followUpForm);
                                }
                            }}
                            className="py-4 space-y-4"
                        >
                            <div>
                                <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                    Action / Title *
                                </label>
                                <input
                                    type="text"
                                    required
                                    placeholder="e.g. Send Revised Proposal or Pricing Deck"
                                    value={followUpForm.title}
                                    onChange={(e) => setFollowUpForm({ ...followUpForm, title: e.target.value })}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-3">
                                <div>
                                    <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Due Date *
                                    </label>
                                    <input
                                        type="date"
                                        required
                                        value={followUpForm.dueDate}
                                        min={getCurrentDate()} // Prevent selecting past dates
                                        onChange={(e) => setFollowUpForm({ ...followUpForm, dueDate: e.target.value })}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                    />
                                </div>
                                <div>
                                    <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Priority
                                    </label>
                                    <select
                                        value={followUpForm.priority}
                                        onChange={(e) => setFollowUpForm({ ...followUpForm, priority: e.target.value })}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                    >
                                        <option value="Low">Low</option>
                                        <option value="Medium">Medium</option>
                                        <option value="High">High</option>
                                    </select>
                                </div>
                            </div>

                            <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
                                <button
                                    type="button"
                                    onClick={() => setIsFollowUpModalOpen(false)}
                                    className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={addFollowUpMutation.isPending}
                                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition disabled:opacity-50"
                                >
                                    {addFollowUpMutation.isPending ? "Scheduling..." : "Save Follow-up"}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* ========================================================== */}
            {/* LOG ACTIVITY MODAL */}
            {/* ========================================================== */}
            {isActivityModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4">
                    <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl border border-gray-100">
                        <div className="flex items-center justify-between pb-4 border-b border-gray-100">
                            <h3 className="text-base font-bold text-gray-900">Log Customer Activity</h3>
                            <button onClick={() => setIsActivityModalOpen(false)} className="text-gray-400 hover:text-gray-600">
                                <X className="w-5 h-5" />
                            </button>
                        </div>

                        <form
                            onSubmit={(e) => {
                                e.preventDefault();
                                if (activityForm.subject) {
                                    addActivityMutation.mutate(activityForm);
                                }
                            }}
                            className="py-4 space-y-4"
                        >
                            <div className="grid grid-cols-2 gap-3">
                                <div>
                                    <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Activity Type
                                    </label>
                                    <select
                                        value={activityForm.activityType}
                                        onChange={(e) => setActivityForm({ ...activityForm, activityType: e.target.value })}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                    >
                                        <option value="Call">Call</option>
                                        <option value="Meeting">Meeting</option>
                                        <option value="Email">Email</option>
                                        <option value="Demo">Demo</option>
                                    </select>
                                </div>
                                <div>
                                    <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Date
                                    </label>
                                    <input
                                        type="datetime-local"
                                        value={activityForm.activityDate}
                                        max={getCurrentDateTime()} // Prevent selecting future dates
                                        onChange={(e) => setActivityForm({ ...activityForm, activityDate: e.target.value })}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-xs outline-none focus:border-blue-500"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                    Subject *
                                </label>
                                <input
                                    type="text"
                                    required
                                    placeholder="e.g. Discovery Call with CTO"
                                    value={activityForm.subject}
                                    onChange={(e) => setActivityForm({ ...activityForm, subject: e.target.value })}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                                />
                            </div>

                            <div>
                                <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                    Summary & Discussion Details
                                </label>
                                <textarea
                                    rows={3}
                                    placeholder="Detailed takeaway from this conversation..."
                                    value={activityForm.summary}
                                    onChange={(e) => setActivityForm({ ...activityForm, summary: e.target.value })}
                                    className="w-full p-2.5 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500 resize-none"
                                />
                            </div>

                            <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
                                <button
                                    type="button"
                                    onClick={() => setIsActivityModalOpen(false)}
                                    className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={addActivityMutation.isPending}
                                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition disabled:opacity-50"
                                >
                                    {addActivityMutation.isPending ? "Logging..." : "Log Activity"}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};