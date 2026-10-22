import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyPendingFollowUps, updateFollowUpStatus } from "../services/followUpService";
import { Link } from "react-router";
import toast from "react-hot-toast";
import { Check, Calendar, ArrowRight, CheckSquare } from "lucide-react";

export const FollowUpsPage = () => {
    const queryClient = useQueryClient();

    const { data: followUps, isPending } = useQuery({
        queryKey: ["my-followups"],
        queryFn: getMyPendingFollowUps,
    });

    const toggleMutation = useMutation({
        mutationFn: ({ id, status }: { id: number; status: string }) => updateFollowUpStatus(id, status),
        onSuccess: () => {
            toast.success("Follow-up updated!");
            queryClient.invalidateQueries({ queryKey: ["my-followups"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        }
    });

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 border-b border-gray-200">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Client Follow-ups & Tasks</h1>
                    <p className="text-sm text-gray-500 mt-1">
                        Manage action items, scheduled calls, and client touchpoints
                    </p>
                </div>
            </div>

            <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
                    <span className="text-xs font-semibold uppercase tracking-wider text-gray-500">
                        Pending Tasks ({followUps?.length ?? 0})
                    </span>
                </div>

                {isPending ? (
                    <div className="p-8 text-center text-sm text-gray-400">Loading follow-ups...</div>
                ) : followUps && followUps.length > 0 ? (
                    <div className="divide-y divide-gray-100">
                        {followUps.map((fu) => (
                            <div key={fu.id} className="p-5 flex items-center justify-between gap-4 hover:bg-gray-50/50">
                                <div className="flex items-start gap-3">
                                    <button
                                        onClick={() => toggleMutation.mutate({
                                            id: fu.id,
                                            status: fu.status === "Completed" ? "Pending" : "Completed"
                                        })}
                                        className={`w-5 h-5 mt-0.5 rounded-full border flex items-center justify-center transition cursor-pointer ${
                                            fu.status === "Completed"
                                                ? "bg-emerald-600 border-emerald-600 text-white"
                                                : "border-gray-300 hover:border-emerald-500 text-transparent"
                                        }`}
                                    >
                                        <Check className="w-3.5 h-3.5" />
                                    </button>
                                    <div className="space-y-0.5">
                                        <p className={`font-semibold text-sm ${fu.status === "Completed" ? "line-through text-gray-400" : "text-gray-900"}`}>
                                            {fu.title}
                                        </p>
                                        <p className="text-xs text-gray-500 flex items-center gap-1.5">
                                            <Calendar className="w-3.5 h-3.5 text-gray-400" />
                                            Due: <span className="font-medium text-gray-700">{new Date(fu.dueDate).toLocaleDateString()}</span>
                                            {fu.assignedToUserName && ` • Assigned to ${fu.assignedToUserName}`}
                                        </p>
                                    </div>
                                </div>

                                <div className="flex items-center gap-3">
                                    <span className={`text-[10px] font-semibold px-2 py-0.5 rounded-full uppercase ${
                                        fu.priority === "High" ? "bg-red-50 text-red-700 border border-red-200" :
                                        fu.priority === "Medium" ? "bg-amber-50 text-amber-700 border border-amber-200" :
                                        "bg-slate-50 text-slate-700 border border-slate-200"
                                    }`}>
                                        {fu.priority}
                                    </span>
                                    <Link
                                        to={`/leads/${fu.leadId}`}
                                        className="text-xs font-semibold text-blue-600 hover:underline flex items-center gap-1"
                                    >
                                        Open Lead <ArrowRight className="w-3 h-3" />
                                    </Link>
                                </div>
                            </div>
                        ))}
                    </div>
                ) : (
                    <div className="p-12 text-center text-gray-400 space-y-2">
                        <CheckSquare className="w-8 h-8 mx-auto text-emerald-500" />
                        <p className="font-semibold text-gray-700">No pending follow-ups!</p>
                        <p className="text-xs text-gray-400">All customer action items have been completed.</p>
                    </div>
                )}
            </div>
        </div>
    );
};
