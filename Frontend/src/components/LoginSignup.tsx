import { useState } from "react";
import { login, register } from "../services/authService";
import toast from "react-hot-toast";
import { Link, useNavigate } from "react-router";
import { useUser } from "../store/userStore";
import { Building2, Lock, Mail, User as UserIcon, CheckCircle2, AlertCircle } from "lucide-react";

interface LoginSignupProps {
    isLoginPage?: boolean;
}

const LoginSignup = ({ isLoginPage = false }: LoginSignupProps) => {
    const [user, setUser] = useState({ fullName: "", email: "", password: "" });
    const [errors, setErrors] = useState<{ fullName: string | null; email: string | null; password: string | null }>({
        fullName: null,
        email: null,
        password: null
    });
    const [isSubmitting, setSubmitting] = useState(false);
    const [registrationSuccess, setRegistrationSuccess] = useState(false);

    const setUserStore = useUser((state) => state.setUser);
    const navigate = useNavigate();

    const HandleError = (field: "fullName" | "email" | "password", message: string) => {
        setErrors((prev) => ({ ...prev, [field]: message }));
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const fieldId = e.target.id as "fullName" | "email" | "password";
        setErrors((prev) => ({ ...prev, [fieldId]: null }));
        setUser((prev) => ({ ...prev, [fieldId]: e.target.value }));
    };

    const validate = () => {
        let isValid = true;

        if (!isLoginPage && user.fullName.trim() === "") {
            HandleError("fullName", "Full name is required");
            isValid = false;
        }

        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailRegex.test(user.email)) {
            HandleError("email", "Please enter a valid email address");
            isValid = false;
        }

        if (user.password.length < 6) {
            HandleError("password", "Password must be at least 6 characters long");
            isValid = false;
        }

        return isValid;
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!validate()) return;

        const { email, password, fullName } = user;
        setSubmitting(true);

        if (isLoginPage) {
            try {
                const response = await login({ email, password });
                localStorage.setItem("token", response.token);
                if (response.refreshToken) localStorage.setItem("refreshToken", response.refreshToken);
                if (response.expiresAt) localStorage.setItem("expiresAt", response.expiresAt);
                
                toast.success("Logged in successfully!");
                setUserStore(response.user);
                navigate("/dashboard");
            } catch (e: any) {
                const msg = e.response?.data?.detail || e.response?.data?.title || e.message || "Failed to log in";
                toast.error(msg, { duration: 5000 });
            }
        } else {
            try {
                await register({ email, fullName, password });
                setRegistrationSuccess(true);
                toast.success("Account registered! Awaiting admin approval.");
            } catch (e: any) {
                const msg = e.response?.data?.detail || e.response?.data?.title || e.message || "Registration failed";
                toast.error(msg);
            }
        }
        setSubmitting(false);
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-indigo-950 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-md text-center">
                <div className="inline-flex items-center justify-center w-12 h-12 rounded-xl bg-blue-600 text-white shadow-lg mb-3">
                    <Building2 className="w-6 h-6" />
                </div>
                <h2 className=" text-2xl sm:text-3xl font-extrabold text-white tracking-tight">
                    CRM Lead System
                </h2>
                <p className="mt-2 text-xs sm:text-sm text-slate-400">
                    {isLoginPage ? "Sign in to your CRM workspace" : "Register for staff access"}
                </p>
            </div>

            <div className="mt-8 max-w-md w-full mx-auto sm:w-full sm:max-w-lg">
                <div className="bg-white py-8 px-6 shadow-2xl rounded-2xl sm:px-10 border border-gray-100">
                    {registrationSuccess ? (
                        <div className="text-center space-y-4 py-4">
                            <div className="w-12 h-12 rounded-full bg-emerald-100 text-emerald-600 flex items-center justify-center mx-auto">
                                <CheckCircle2 className="w-7 h-7" />
                            </div>
                            <h3 className="text-xl font-bold text-gray-900">Registration Received!</h3>
                            <p className="text-sm text-gray-600 leading-relaxed">
                                Your account for <span className="font-semibold text-gray-800">{user.email}</span> has been created with pending status.
                            </p>
                            <div className="p-3 bg-amber-50 rounded-lg border border-amber-200 text-amber-800 text-xs text-left flex gap-2 items-start">
                                <AlertCircle className="w-4 h-4 shrink-0 mt-0.5 text-amber-600" />
                                <span>An administrator must approve your account and assign your role before you can log in.</span>
                            </div>
                            <div className="pt-4">
                                <Link
                                    to="/login"
                                    onClick={() => setRegistrationSuccess(false)}
                                    className="w-full inline-flex justify-center items-center py-2.5 px-4 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 transition"
                                >
                                    Go to Login Page
                                </Link>
                            </div>
                        </div>
                    ) : (
                        <>
                            {/* Toggle Tabs */}
                            <div className="flex border-b border-gray-200 mb-6">
                                <Link
                                    to="/login"
                                    className={`flex-1 text-center pb-3 text-sm font-semibold transition border-b-2 ${
                                        isLoginPage
                                            ? "border-blue-600 text-blue-600"
                                            : "border-transparent text-gray-400 hover:text-gray-700"
                                    }`}
                                >
                                    Sign In
                                </Link>
                                <Link
                                    to="/register"
                                    className={`flex-1 text-center pb-3 text-sm font-semibold transition border-b-2 ${
                                        !isLoginPage
                                            ? "border-blue-600 text-blue-600"
                                            : "border-transparent text-gray-400 hover:text-gray-700"
                                    }`}
                                >
                                    Create Account
                                </Link>
                            </div>

                            <form onSubmit={handleSubmit} className="space-y-4">
                                {!isLoginPage && (
                                    <div>
                                        <label htmlFor="fullName" className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                            Full Name
                                        </label>
                                        <div className="relative">
                                            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
                                                <UserIcon className="w-4 h-4" />
                                            </div>
                                            <input
                                                type="text"
                                                id="fullName"
                                                onChange={handleChange}
                                                value={user.fullName}
                                                placeholder="e.g. Alex Morgan"
                                                className={`w-full pl-9 pr-3 py-2 border rounded-lg outline-none text-sm transition ${
                                                    errors.fullName ? "border-red-500 bg-red-50/30" : "border-gray-300 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                                }`}
                                            />
                                        </div>
                                        {errors.fullName && <p className="mt-1 text-xs text-red-500">{errors.fullName}</p>}
                                    </div>
                                )}

                                <div>
                                    <label htmlFor="email" className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Email Address
                                    </label>
                                    <div className="relative">
                                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
                                            <Mail className="w-4 h-4" />
                                        </div>
                                        <input
                                            type="email"
                                            id="email"
                                            onChange={handleChange}
                                            value={user.email}
                                            placeholder="you@crm.com"
                                            className={`w-full pl-9 pr-3 py-2 border rounded-lg outline-none text-sm transition ${
                                                errors.email ? "border-red-500 bg-red-50/30" : "border-gray-300 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                            }`}
                                        />
                                    </div>
                                    {errors.email && <p className="mt-1 text-xs text-red-500">{errors.email}</p>}
                                </div>

                                <div>
                                    <label htmlFor="password" className="block text-xs font-semibold text-gray-700 uppercase tracking-wider mb-1">
                                        Password
                                    </label>
                                    <div className="relative">
                                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
                                            <Lock className="w-4 h-4" />
                                        </div>
                                        <input
                                            type="password"
                                            id="password"
                                            onChange={handleChange}
                                            value={user.password}
                                            placeholder="••••••••"
                                            className={`w-full pl-9 pr-3 py-2 border rounded-lg outline-none text-sm transition ${
                                                errors.password ? "border-red-500 bg-red-50/30" : "border-gray-300 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                            }`}
                                        />
                                    </div>
                                    {errors.password && <p className="mt-1 text-xs text-red-500">{errors.password}</p>}
                                </div>

                                {!isLoginPage && (
                                    <div className="p-3 bg-blue-50 rounded-lg border border-blue-100 text-xs text-blue-800">
                                        Roles and managers are assigned by an administrator upon reviewing your registration request.
                                    </div>
                                )}

                                <button
                                    type="submit"
                                    disabled={isSubmitting}
                                    className="w-full mt-2 py-2.5 px-4 bg-blue-600 hover:bg-blue-700 text-white font-semibold text-sm rounded-lg shadow-sm transition-all focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 cursor-pointer"
                                >
                                    {isSubmitting ? "Processing..." : isLoginPage ? "Sign In" : "Submit Registration"}
                                </button>
                            </form>

                            {/* Demo Accounts Helper */}
                            <div className="mt-6 pt-4 border-t border-gray-100 text-xs text-gray-500">
                                <p className="font-semibold text-gray-700 mb-1.5">Demo Credentials:</p>
                                <div className="space-y-1 bg-slate-50 p-2.5 rounded-lg border border-slate-200">
                                    <div className="flex justify-between">
                                        <span className="font-medium text-slate-700">Admin:</span>
                                        <code className="text-slate-600">admin@crm.com / Password123</code>
                                    </div>
                                    <div className="flex justify-between">
                                        <span className="font-medium text-slate-700">Manager:</span>
                                        <code className="text-slate-600">sarah.m@crm.com / Password123</code>
                                    </div>
                                    <div className="flex justify-between">
                                        <span className="font-medium text-slate-700">Sales Rep:</span>
                                        <code className="text-slate-600">john.rep@crm.com / Password123</code>
                                    </div>
                                </div>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
};

export default LoginSignup;
