import { useState } from "react";
import { login, register } from "../services/authService";
import toast from "react-hot-toast";
import { Link, useNavigate } from "react-router";
import { useUser } from "../store/userStore";

interface LoginSignupProps {
    isLoginPage?: boolean;
}

const LoginSignup = ({ isLoginPage = false }: LoginSignupProps) => {
    const [user, setUser] = useState({ fullName: "", email: "", password: "", role: "Sales Rep" });
    const [errors, setErrors] = useState<{ fullName: string | null; email: string | null; password: string | null }>({
        fullName: null,
        email: null,
        password: null
    });
    const [isSubmitting, setSubmitting] = useState(false);

    const setUserStore = useUser((state)=>state.setUser);
    const navigate = useNavigate();

    const roles = ["Sales Rep", "Sales Manager"];

  

    const HandleError = (field: "fullName" | "email" | "password", message: string) => {
        setErrors((prev) => ({ ...prev, [field]: message }));
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const fieldId = e.target.id as "fullName" | "email" | "password";
        // Reset specific field error
        setErrors((prev) => ({ ...prev, [fieldId]: null }));
        setUser((prev) => ({ ...prev, [fieldId]: e.target.value }));
    };

    const validate = () => {
        let isValid = true;

        // 1. Full Name Validation (Only for Signup Page)
        if (!isLoginPage && user.fullName.trim() === "") {
            HandleError("fullName", "Full name is required");
            isValid = false;
        }

        // 2. Email Validation
        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailRegex.test(user.email)) {
            HandleError("email", "Please enter a valid email address");
            isValid = false;
        }

        // 3. Password Validation
        if (user.password.length < 6) {
            HandleError("password", "Password must be at least 6 characters long");
            isValid = false;
        }

        return isValid;
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!validate()) return;

        const { email, password, fullName, role } = user;
        setSubmitting(true);
        if (isLoginPage) {
            console.log("Logging in with:", { email: user.email, password: user.password });
            try {
                const { token, refreshToken, user, expiresAt } = await login({ email, password });
                localStorage.setItem("token", token);
                localStorage.setItem("refreshToken", refreshToken);
                localStorage.setItem("expiresAt", expiresAt);
                toast.success("User logged in successfully");
                
                // set user

                setUserStore(user);
                // Navigate to dashboard
                navigate("/dashboard");
            } catch (e: any) {
                toast.error(e.response?.data?.title);
            }
        } else {
            console.log("Signing up with:", user);
            try {
                const { token, refreshToken, user, expiresAt } = await register({ email, fullName, password, role });
                localStorage.setItem("token", token);
                localStorage.setItem("refreshToken", refreshToken);
                localStorage.setItem("expiresAt", expiresAt);

                toast.success("User registered successfully");
                setUserStore(user);
                navigate("/dashboard");
            } catch (e: any) {
                toast.error(e.response?.data?.title);
            }
        }
        setSubmitting(false);
    };

    const handleRoleClick = (role: string) => {
        return () => setUser((prev) => ({ ...prev, role: role }));
    };

    return (
        <div className="mx-auto w-full max-w-md p-5">
            <h2 className="mb-6 text-2xl font-bold text-gray-800">
                {isLoginPage ? "Login" : "Sign Up"}
            </h2>

            <form onSubmit={handleSubmit} className="space-y-4">
                {!isLoginPage && (
                    <div>
                        <label
                            htmlFor="fullName"
                            className="mb-1 block text-sm font-medium text-gray-700"
                        >
                            Full Name
                        </label>
                        <input
                            type="text"
                            id="fullName"
                            onChange={handleChange}
                            value={user.fullName}
                            className="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
                        />
                        {errors.fullName && (
                            <p className="mt-1 text-sm text-red-500">
                                {errors.fullName}
                            </p>
                        )}
                    </div>
                )}

                <div>
                    <label
                        htmlFor="email"
                        className="mb-1 block text-sm font-medium text-gray-700"
                    >
                        Email
                    </label>
                    <input
                        type="email"
                        id="email"
                        onChange={handleChange}
                        value={user.email}
                        className="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
                    />
                    {errors.email && (
                        <p className="mt-1 text-sm text-red-500">
                            {errors.email}
                        </p>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="password"
                        className="mb-1 block text-sm font-medium text-gray-700"
                    >
                        Password
                    </label>
                    <input
                        type="password"
                        id="password"
                        onChange={handleChange}
                        value={user.password}
                        className="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-200"
                    />
                    {errors.password && (
                        <p className="mt-1 text-sm text-red-500">
                            {errors.password}
                        </p>
                    )}
                </div>



                {!isLoginPage && <div>
                    <label className="mb-2 block text-sm font-medium text-gray-700">
                        Select Role:
                    </label>

                    <div className="flex flex-wrap gap-3">
                        {roles.map((crole) => (
                            <button
                                key={crole}
                                type="button"
                                onClick={handleRoleClick(crole)}
                                className={`rounded-lg border px-4 py-2 transition ${crole === user.role
                                    ? "border-red-500 bg-red-50 font-semibold text-red-500"
                                    : "border-gray-300 text-gray-700 hover:border-gray-400"
                                    }`}
                            >
                                {crole}
                            </button>
                        ))}
                    </div>
                </div>}

                <button
                    type="submit"
                    className="w-full rounded-lg bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-300 focus:ring-offset-2"
                    disabled={isSubmitting}
                >
                    {isLoginPage ? "Login" : "Sign Up"}
                    {isSubmitting? "...": ""}
                </button>

            </form>
            {isLoginPage && <div className="text-sm text-red-500 text-center mt-2">Dont have an account? <Link to="/register" className="underline">Register</Link></div>}
            {!isLoginPage && <div className="text-sm text-red-500 text-center mt-2">Already have an account? <Link to="/login" className="underline">Login</Link></div>}
        </div>
    );
};

export default LoginSignup;
