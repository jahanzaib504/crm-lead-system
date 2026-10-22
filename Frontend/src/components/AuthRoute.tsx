import { useUser } from "../store/userStore";
import { Navigate, Outlet } from "react-router";

export const AuthRoute = () => {
    const user = useUser((state) => state.user);
    const loading = useUser((state) => state.loading);

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-screen bg-slate-900 text-slate-200">
                <div className="flex flex-col items-center gap-3">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                    <span className="text-xs font-semibold text-slate-400">Authenticating session...</span>
                </div>
            </div>
        );
    }
    
    if (user == null) {
        return <Navigate to="/login" replace />;
    }
    
    return <Outlet />;
};