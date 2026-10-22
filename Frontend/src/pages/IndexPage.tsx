import { Link } from "react-router";
import { Building2, ArrowRight, ShieldCheck, Layers, PhoneCall, Sparkles } from "lucide-react";
import { useUser } from "../store/userStore";

export const IndexPage = () => {
    const user = useUser((state) => state.user);

    return (
        <div className="min-h-screen bg-slate-900 text-slate-100 flex flex-col justify-between">
            {/* Navigation Header */}
            <header className="border-b border-slate-800 bg-slate-950/40 px-6 py-4">
                <div className="max-w-7xl mx-auto flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className="w-9 h-9 rounded-xl bg-blue-600 flex items-center justify-center text-white font-bold shadow-md">
                            <Building2 className="w-5 h-5" />
                        </div>
                        <span className="text-lg font-bold text-white tracking-wide">CRM Lead System</span>
                    </div>

                    <div className="flex items-center gap-3">
                        {user ? (
                            <Link
                                to="/dashboard"
                                className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold shadow-xs transition"
                            >
                                Go to Dashboard &rarr;
                            </Link>
                        ) : (
                            <>
                                <Link
                                    to="/login"
                                    className="px-4 py-2 text-xs font-semibold text-slate-300 hover:text-white transition"
                                >
                                    Sign In
                                </Link>
                                <Link
                                    to="/register"
                                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-semibold shadow-xs transition"
                                >
                                    Register
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </header>

            {/* Hero Section */}
            <main className="max-w-5xl mx-auto px-6 py-16 text-center space-y-8 flex-1 flex flex-col justify-center">
                <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-500/10 border border-blue-500/20 text-blue-400 text-xs font-semibold mx-auto">
                    <Sparkles className="w-3.5 h-3.5" /> Next-Generation Lead & Customer Pipeline
                </div>

                <h1 className="text-4xl sm:text-6xl font-extrabold text-white tracking-tight leading-tight">
                    Convert Inbound Inquiries into <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-indigo-300">Closed Deals</span>
                </h1>

                <p className="max-w-2xl mx-auto text-base sm:text-lg text-slate-400 leading-relaxed">
                    A multi-tiered CRM designed for seamless lead capture, manager pool acquisition, and sales representative follow-ups, notes, and activity tracking.
                </p>

                <div className="flex flex-col sm:flex-row items-center justify-center gap-4 pt-4">
                    <Link
                        to={user ? "/dashboard" : "/login"}
                        className="w-full sm:w-auto inline-flex items-center justify-center gap-2 px-6 py-3.5 bg-blue-600 hover:bg-blue-500 text-white font-semibold rounded-xl shadow-lg shadow-blue-600/30 transition text-sm"
                    >
                        {user ? "Open Dashboard" : "Enter Staff Portal"}
                        <ArrowRight className="w-4 h-4" />
                    </Link>

                    <Link
                        to="/create-lead"
                        className="w-full sm:w-auto inline-flex items-center justify-center gap-2 px-6 py-3.5 bg-slate-800 hover:bg-slate-700 text-slate-200 font-semibold rounded-xl border border-slate-700 transition text-sm"
                    >
                        Submit Lead Inquiry (Public Form)
                    </Link>
                </div>

                {/* Features Grid */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 pt-12 text-left">
                    <div className="p-6 bg-slate-800/40 rounded-2xl border border-slate-800 space-y-3">
                        <div className="w-10 h-10 rounded-xl bg-blue-500/10 text-blue-400 flex items-center justify-center">
                            <Layers className="w-5 h-5" />
                        </div>
                        <h3 className="font-bold text-white text-base">Open Leads Pool</h3>
                        <p className="text-xs text-slate-400 leading-relaxed">
                            Public inquiries enter an unassigned pool. Sales managers review, claim, and allocate leads to the right representatives.
                        </p>
                    </div>

                    <div className="p-6 bg-slate-800/40 rounded-2xl border border-slate-800 space-y-3">
                        <div className="w-10 h-10 rounded-xl bg-purple-500/10 text-purple-400 flex items-center justify-center">
                            <PhoneCall className="w-5 h-5" />
                        </div>
                        <h3 className="font-bold text-white text-base">Representative Workflows</h3>
                        <p className="text-xs text-slate-400 leading-relaxed">
                            Reps track deal progress with internal notes, priority-driven follow-ups, and activity histories for calls, meetings, and demos.
                        </p>
                    </div>

                    <div className="p-6 bg-slate-800/40 rounded-2xl border border-slate-800 space-y-3">
                        <div className="w-10 h-10 rounded-xl bg-emerald-500/10 text-emerald-400 flex items-center justify-center">
                            <ShieldCheck className="w-5 h-5" />
                        </div>
                        <h3 className="font-bold text-white text-base">Admin Access Control</h3>
                        <p className="text-xs text-slate-400 leading-relaxed">
                            New users sign up without pre-selecting a role. Administrators review applicant requests, assign roles, and designate managers.
                        </p>
                    </div>
                </div>
            </main>

            {/* Footer */}
            <footer className="border-t border-slate-800/60 py-6 text-center text-xs text-slate-500">
                CRM Lead Management System &copy; 2026. All rights reserved.
            </footer>
        </div>
    );
};