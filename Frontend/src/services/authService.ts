import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
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

export const register = async ({ fullName, email, password }: RegisterArgs): Promise<ApiResponse<RegisterResponse>> => {
    const request = await api.post<ApiResponse<RegisterResponse>>("/auth/register", { fullName, email, password });
    return request.data;
};

export const login = async ({ email, password }: LoginArgs): Promise<ApiResponse<LoginResponse>> => {
    const request = await api.post<ApiResponse<LoginResponse>>("/auth/login", { email, password });
    return request.data;
};

export const logout = async (): Promise<ApiResponse<boolean>> => {
    try {
        const response = await api.post<ApiResponse<boolean>>("/auth/logout");
        return response.data;
    } finally {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("expiresAt");
        window.location.href = "/login";
    }
};