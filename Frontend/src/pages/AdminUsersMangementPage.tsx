import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { getUsers, getPendingUsers, getManagers, approveUser, revokeAccess, grantAccess } from "../services/userService";
import { useUser } from "../store/userStore";
import toast from "react-hot-toast";
import {
    Users,
    Clock,
    Check,
    X,
    Search
} from "lucide-react";
import { UserAccessModal } from "../components/UserAccessModel";

// Admin user page is for user management, It can only be accessed by admin

export const AdminUsersManagementPage = () => {
    const queryClient = useQueryClient();
    const currentUser = useUser((state) => state.user);

    // Two tabs - one for pending user and one for all users
    const [activeTab, setActiveTab] = useState<"pending" | "all">("pending");
    const [searchTerm, setSearchTerm] = useState(""); // Allows searching across records

    const [selectedUser, setSelectedUser] = useState<any>(null);
    const [loadingId, setLoadingId] = useState<number | null>(null);

    // Queries
    const { data: pendingUsers, isPending: pendingLoading } = useQuery({
        queryKey: ["admin-pending-users"],
        queryFn: getPendingUsers,
    });


    const { data: allUsers, isPending: allLoading } = useQuery({
        queryKey: ["admin-all-users"],
        queryFn: getUsers,
    });




    const revokeMutation = useMutation({
        mutationFn: (id: number) => revokeAccess(id),
        onSuccess: () => {
            setLoadingId(null);
            toast.success("User access revoked.");
            queryClient.invalidateQueries({ queryKey: ["admin-all-users"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        }
    });

    const grantMutation = useMutation({ // Revoking user access will change pending users in dashboard
        mutationFn: (id: number) => grantAccess(id),
        onSuccess: () => {
            setLoadingId(null);
            toast.success("User access granted.");
            queryClient.invalidateQueries({ queryKey: ["admin-all-users"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
        }
    });


    const filteredAllUsers = (allUsers || []).filter(u =>
        u.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        u.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
        u.role?.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const getRoleBadge = (roleName?: string) => {
        switch (roleName) {
            case "Admin":
                return <span className="px-2.5 py-0.5 rounded-full text-xs font-semibold bg-rose-100 text-rose-800 border border-rose-200">Admin</span>;
            case "Sales Manager":
                return <span className="px-2.5 py-0.5 rounded-full text-xs font-semibold bg-indigo-100 text-indigo-800 border border-indigo-200">Sales Manager</span>;
            default:
                return <span className="px-2.5 py-0.5 rounded-full text-xs font-semibold bg-emerald-100 text-emerald-800 border border-emerald-200">Sales Rep</span>;
        }
    };

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 border-b border-gray-200">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Staff & Access Control</h1>
                    <p className="text-sm text-gray-500 mt-1">
                        Review pending account requests, assign staff roles, and configure manager hierarchies
                    </p>
                </div>
            </div>

            {/* Tabs */}
            <div className="flex border-b border-gray-200 gap-4">
                <button
                    onClick={() => setActiveTab("pending")}
                    className={`pb-3 text-sm font-semibold transition border-b-2 flex items-center gap-2 ${activeTab === "pending"
                        ? "border-blue-600 text-blue-600"
                        : "border-transparent text-gray-500 hover:text-gray-800"
                        }`}
                >
                    <Clock className="w-4 h-4" />
                    Pending Approvals
                    {pendingUsers && pendingUsers.length > 0 && (
                        <span className="px-2 py-0.2 rounded-full text-xs bg-amber-100 text-amber-800 font-bold">
                            {pendingUsers.length}
                        </span>
                    )}
                </button>

                <button
                    onClick={() => setActiveTab("all")}
                    className={`pb-3 text-sm font-semibold transition border-b-2 flex items-center gap-2 ${activeTab === "all"
                        ? "border-blue-600 text-blue-600"
                        : "border-transparent text-gray-500 hover:text-gray-800"
                        }`}
                >
                    <Users className="w-4 h-4" />
                    All Registered Staff ({allUsers?.length ?? 0})
                </button>
            </div>

            {/* TAB 1: PENDING APPROVALS */}
            {activeTab === "pending" && (
                <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                    <div className="px-6 py-4 border-b border-gray-100 bg-amber-50/40 flex items-center justify-between">
                        <div>
                            <h3 className="font-semibold text-gray-900">Awaiting Administrator Review</h3>
                            <p className="text-xs text-gray-500">Users who registered through the signup portal with access pending</p>
                        </div>
                    </div>

                    {pendingLoading ? (
                        <div className="p-8 text-center text-sm text-gray-400">Loading pending requests...</div>
                    ) : pendingUsers && pendingUsers.length > 0 ? (
                        <div className="divide-y divide-gray-100">
                            {pendingUsers.map((u) => (
                                <div key={u.id} className="p-5 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-gray-50/50">
                                    <div className="space-y-1">
                                        <div className="flex items-center gap-2">
                                            <span className="font-semibold text-gray-900 text-sm">{u.fullName}</span>
                                            <span className="px-2 py-0.5 rounded-full text-[10px] bg-amber-100 text-amber-800 font-semibold uppercase">
                                                Pending Approval
                                            </span>
                                        </div>
                                        <p className="text-xs text-gray-500">
                                            Email: <span className="font-medium text-gray-700">{u.email}</span>
                                            {u.createdAt && ` • Registered on ${new Date(u.createdAt).toLocaleDateString()}`}
                                        </p>
                                    </div>

                                    <button
                                        onClick={() => setSelectedUser(u)}
                                        className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold shadow-xs transition shrink-0"
                                    >
                                        Review & Approve
                                    </button>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className="p-12 text-center text-gray-400 space-y-2">
                            <Check className="w-8 h-8 mx-auto text-emerald-500" />
                            <p className="font-semibold text-gray-700">No pending access requests.</p>
                            <p className="text-xs text-gray-400">All registered users have been reviewed and approved.</p>
                        </div>
                    )}
                </div>
            )}

            {/* TAB 2: ALL STAFF */}
            {activeTab === "all" && (
                <div className="space-y-4">
                    {/* Search */}
                    <div className="relative max-w-sm">
                        <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                        <input
                            type="text"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            placeholder="Search staff by name, email, or role..."
                            className="w-full pl-9 pr-3 py-2 text-sm border border-gray-300 rounded-lg outline-none focus:border-blue-500"
                        />
                    </div>

                    <div className="bg-white rounded-xl border border-gray-200 shadow-xs overflow-hidden">
                        {allLoading ? (
                            <div className="p-8 text-center text-sm text-gray-400">Loading staff directory...</div>
                        ) : filteredAllUsers.length > 0 ? (
                            <table className="min-w-full divide-y divide-gray-200 text-sm">
                                <thead className="bg-gray-50">
                                    <tr>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">User</th>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Role</th>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Assigned Manager</th>
                                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
                                        <th className="px-6 py-3 text-right text-xs font-semibold text-gray-500 uppercase tracking-wider">Actions</th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white divide-y divide-gray-100">
                                    {filteredAllUsers.map((u) => (
                                        <tr key={u.id} className="hover:bg-gray-50">
                                            <td className="px-6 py-4">
                                                <div className="font-semibold text-gray-900">{u.fullName}</div>
                                                <div className="text-xs text-gray-400">{u.email}</div>
                                            </td>
                                            <td className="px-6 py-4">
                                                {getRoleBadge(u.role?.name)}
                                            </td>
                                            <td className="px-6 py-4 text-xs text-gray-700">
                                                {u.managerName ? (
                                                    <span className="font-medium text-gray-800">{u.managerName}</span>
                                                ) : (
                                                    <span className="text-gray-400 italic">None</span>
                                                )}
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${u.isActive ? "bg-emerald-50 text-emerald-700 border border-emerald-200" : "bg-red-50 text-red-700 border border-red-200"
                                                    }`}>
                                                    {u.isActive ? "Active" : "Inactive"}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-right space-x-2">
                                               
                                                {u.id !== currentUser?.id && (
                                                    u.isActive ? (
                                                        <button
                                                            disabled={loadingId === u.id}
                                                            onClick={() => {
                                                                setLoadingId(u.id);
                                                                revokeMutation.mutate(u.id);
                                                            }}
                                                            className="hover:cursor-pointer px-2.5 py-1 text-xs font-semibold rounded-md border border-red-200 text-red-600 hover:bg-red-50 transition"
                                                        >
                                                            {(loadingId === u.id) ? "Deactivating..." : "Deactivate"}
                                                        </button>
                                                    ) : (
                                                        <button
                                                            disabled={loadingId === u.id}
                                                            onClick={() => {
                                                                setLoadingId(u.id);
                                                                grantMutation.mutate(u.id);
                                                            }}
                                                            className="hover:cursor-pointer px-2.5 py-1 text-xs font-semibold rounded-md border border-emerald-200 text-emerald-600 hover:bg-emerald-50 transition"
                                                        >
                                                            {(loadingId === u.id) ? "Activating..." : "Activate"}
                                                        </button>
                                                    )
                                                )}
                                                {u.id!=currentUser?.id && u.managerId == null && 
                                                    <button 
                                                        onClick={() => {
                                                            setSelectedUser(u);
                                                            console.log("Selected user for editing access:", u);
                                                        }}
                                                        className="hover:cursor-pointer px-2.5 py-1 text-xs font-semibold rounded-md border border-blue-200 text-blue-600 hover:bg-blue-50 transition"
                                                    >
                                                    Edit Access
                                                </button>}
                                                
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <div className="p-8 text-center text-sm text-gray-400">No staff found matching search.</div>
                        )}
                    </div>
                </div>
            )}

            {selectedUser && <UserAccessModal selectedUser={selectedUser} setSelectedUser={setSelectedUser}/>}
        </div>
    );
};

