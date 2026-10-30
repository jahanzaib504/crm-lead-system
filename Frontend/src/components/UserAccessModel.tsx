import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { approveUser, getManagers } from "../services/userService";
import toast from "react-hot-toast";
import { X } from "lucide-react";
import type { User } from "../types/User";

export const UserAccessModal = ({selectedUser, setSelectedUser}: {selectedUser: User, setSelectedUser: any}) => {
    const queryClient = useQueryClient();
    const [roleId, setRoleId] = useState<number>(selectedUser.roleId ?? 3); // Default to Sales Rep if roleId is null
    const [managerId, setManagerId] = useState<number | null>(selectedUser.managerId ?? null);
    const [isActive, setIsActive] = useState<boolean>(selectedUser.isActive ?? true);
            

    const { data: managers } = useQuery({
        queryKey: ["admin-managers-list"],
        queryFn: getManagers,
        enabled: Boolean(selectedUser),
    });

    // Mutations
    const updateAccessMutation = useMutation({
        mutationFn: ({ id, roleId, managerId, isActive }: { id: number; roleId: number; managerId: number | null; isActive: boolean }) =>
            approveUser(id, { roleId, managerId, isActive }),
        onSuccess: () => {
            toast.success("User access updated successfully!");
            queryClient.invalidateQueries({ queryKey: ["admin-all-users"] });
            queryClient.invalidateQueries({ queryKey: ["dashboard-analytics"] });
            queryClient.invalidateQueries({ queryKey: ["admin-pending-users"] });
            setSelectedUser(null);
        },
        onError: (err: any) => {
            toast.error(err.response?.data?.detail || "Failed to update access.");
        }
    });

    const handleFormSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedUser) return;
        updateAccessMutation.mutate({
            id: selectedUser.id,
            roleId: Number(roleId),
            managerId: Number(roleId) === 3 ? (managerId ? Number(managerId) : null) : null, // No direct manager for admin
            isActive: isActive
        });
    };
    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 p-4">
            <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl border border-gray-100">
                <div className="flex items-center justify-between pb-4 border-b border-gray-100">
                    <h3 className="text-base font-bold text-gray-900">
                        {selectedUser.roleId !== null ? "Edit User Permissions" : "Approve User Access"}
                    </h3>
                    <button onClick={() => setSelectedUser(null)} className="text-gray-400 hover:text-gray-600">
                        <X className="w-5 h-5" />
                    </button>
                </div>

                <form onSubmit={handleFormSubmit} className="py-4 space-y-4">
                    <div>
                        <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                            Staff Member
                        </label>
                        <p className="text-sm font-bold text-gray-900">{selectedUser.fullName}</p>
                        <p className="text-xs text-gray-500">{selectedUser.email}</p>
                    </div>

                    {selectedUser.roleId == null && <div>
                        <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                            Role *
                        </label>
                        <select
                            value={roleId}
                            onChange={(e) => setRoleId(Number(e.target.value))}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                        >
                            <option value={3}>Sales Representative</option>
                            <option value={2}>Sales Manager</option>
                        </select>
                    </div>}

                    {Number(roleId) === 3 && (
                        <div>
                            <label className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                Assigned Sales Manager
                            </label>
                            <select
                                value={managerId || ""}
                                onChange={(e) => setManagerId(e.target.value ? Number(e.target.value) : null)}
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:border-blue-500"
                            >
                                <option value="">-- No Direct Manager --</option>
                                {managers?.filter(m => m.id !== selectedUser.id).map((mgr) => (
                                    <option key={mgr.id} value={mgr.id}>
                                        {mgr.fullName} ({mgr.email})
                                    </option>
                                ))}
                            </select>
                            <p className="text-[11px] text-gray-500 mt-1">
                                The manager who assigns pool leads to this sales rep.
                            </p>
                        </div>
                    )}

                    <div>
                        <label className="flex items-center gap-2 cursor-pointer pt-2">
                            <input
                                type="checkbox"
                                checked={isActive}
                                onChange={(e) => setIsActive(e.target.checked)}
                                className="w-4 h-4 rounded text-blue-600 focus:ring-blue-500 border-gray-300"
                            />
                            <span className="text-sm font-medium text-gray-700">Account is Active & Permitted to Login</span>
                        </label>
                    </div>

                    <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
                        <button
                            type="button"
                            onClick={() => setSelectedUser(null)}
                            className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={updateAccessMutation.isPending}
                            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition disabled:opacity-50"
                        >
                            {updateAccessMutation.isPending ? "Saving..." : "Save Settings"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}