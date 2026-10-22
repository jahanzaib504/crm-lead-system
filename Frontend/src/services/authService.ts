import api from "../api";
import type { User } from "../types/User";

export interface LoginResponse {
    token: string;
    refreshToken?: string;
    expiresAt?: string;
    user: User;
}

export interface RegisterResponse {
    token?: string;
    user: User;
}

export interface LoginArgs {
    email: string;
    password: string;
}

export interface RegisterArgs {
    fullName: string;
    email: string;
    password: string;
}

export const register = async ({ fullName, email, password }: RegisterArgs): Promise<RegisterResponse> => {
    const request = await api.post("/auth/register", { fullName, email, password });
    return request.data;
};

export const login = async ({ email, password }: LoginArgs): Promise<LoginResponse> => {
    const request = await api.post("/auth/login", { email, password });
    return request.data;
};

export const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("expiresAt");
    window.location.href = "/login"; // Navigate to /login immediately
};