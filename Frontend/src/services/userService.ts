import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { User } from "../types/User";

export interface ApproveUserPayload {
    roleId: number;
    managerId?: number | null;
    isActive: boolean;
}
// Returns current user data
export const getMe = async (): Promise<ApiResponse<User>> => {
    return (await api.get<ApiResponse<User>>("/users/me")).data;
};
// Returns all the users under this user
export const getUsers = async (): Promise<ApiResponse<User[]>> => {
    return (await api.get<ApiResponse<User[]>>("/users")).data;
};
// Get Pending access user for admin
export const getPendingUsers = async (): Promise<ApiResponse<User[]>> => {
    return (await api.get<ApiResponse<User[]>>("/users/pending")).data;
};
// Get managers for admin
export const getManagers = async (): Promise<ApiResponse<User[]>> => {
    return (await api.get<ApiResponse<User[]>>("/users/managers")).data;
};
// Get sales reps data
export const getSalesReps = async (): Promise<ApiResponse<User[]>> => {
    return (await api.get<ApiResponse<User[]>>("/users/reps")).data;
};

export const approveUser = async (id: number, payload: ApproveUserPayload): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/users/approve/${id}`, payload)).data;
};
// Grants or revoke user access
export const updateUserAccess = async (id: number, payload: ApproveUserPayload): Promise<ApiResponse<boolean>> => {
    return (await api.put<ApiResponse<boolean>>(`/users/${id}/access`, payload)).data;
};
// Grant access to user
export const grantAccess = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/users/grant-access/${id}`)).data;
};
// Revokes user access
export const revokeAccess = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/users/revoke-access/${id}`)).data;
};