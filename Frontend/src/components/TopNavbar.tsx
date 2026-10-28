import { useUser } from "../store/userStore";
import { logout } from "../services/authService";
import { LogOut, User as UserIcon, Shield, Briefcase, UserCheck, ExternalLink } from "lucide-react";
import { Link } from "react-router";

export const TopNavbar = () => {
    const user = useUser((state) => state.user);

    const getRoleBadge = (roleName?: string) => {
        switch (roleName) {
            case "Admin":
                return (
                    <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-rose-100 text-rose-800 border border-rose-200">
                        <Shield className="w-3 h-3" /> Admin
                    </span>
                );
            case "Sales Manager":
                return (
                    <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-indigo-100 text-indigo-800 border border-indigo-200">
                        <Briefcase className="w-3 h-3" /> Sales Manager
                    </span>
                );
            default:
                return (
                    <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-emerald-100 text-emerald-800 border border-emerald-200">
                        <UserCheck className="w-3 h-3" /> Sales Rep
                    </span>
                );
        }
    };

    return (
        <header className="h-16 bg-white border-b border-gray-200 px-6 flex items-center justify-between sticky top-0 z-30 shadow-xs">
            <div className="ml-10 flex items-center gap-4 md:ml-0">
                <span className="text-sm text-gray-500 font-medium">
                    Welcome back, <span className="font-semibold text-gray-800">{user?.fullName || "User"}</span>
                </span>
            </div>

            <div className="flex items-center gap-4">
                <Link
                    to="/create-lead"
                    target="_blank"
                    className="hidden sm:inline-flex items-center gap-1.5 text-xs font-medium text-slate-600 hover:text-blue-600 px-3 py-1.5 rounded-lg border border-gray-200 hover:border-blue-200 transition-colors"
                    title="Open public lead submission form"
                >
                    <ExternalLink className="w-3.5 h-3.5" />
                    Public Lead Form
                </Link>

                {/* Role Badge */}
                {getRoleBadge(user?.role?.name)}

                {/* User Info & Avatar */}
                <div className="flex items-center gap-2 pl-2 border-l border-gray-200">
                    <div className="w-8 h-8 rounded-full bg-slate-800 text-white flex items-center justify-center font-semibold text-sm">
                        {user?.fullName?.charAt(0).toUpperCase() || <UserIcon className="w-4 h-4" />}
                    </div>
                    <div className="hidden md:block text-left text-xs">
                        <p className="font-semibold text-gray-800 leading-tight">{user?.fullName}</p>
                        <p className="text-gray-400">{user?.email}</p>
                    </div>
                </div>

                {/* Logout Button */}
                <button
                    onClick={logout}
                    className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-red-600 hover:bg-red-50 rounded-lg transition-colors border border-transparent hover:border-red-200"
                    title="Sign Out"
                >
                    <LogOut className="w-4 h-4" />
                    <span className="hidden sm:inline">Logout</span>
                </button>
            </div>
        </header>
    );
};
