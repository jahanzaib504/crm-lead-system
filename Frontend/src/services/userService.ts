import api from "../api";
import type { User } from "../types/User";

export interface ApproveUserPayload {
    roleId: number;
    managerId?: number | null;
    isActive: boolean;
}
// Returns current user data
export const getMe = async (): Promise<User> => {
    return (await api.get("/users/me")).data;
};
// Returns all the users under this user
export const getUsers = async (): Promise<User[]> => {
    return (await api.get("/users")).data;
};
// Get Pending access user for admin
export const getPendingUsers = async (): Promise<User[]> => {
    return (await api.get("/users/pending")).data;
};
// Get managers for admin
export const getManagers = async (): Promise<User[]> => {
    return (await api.get("/users/managers")).data;
};
// Get sales reps data
export const getSalesReps = async (): Promise<User[]> => {
    return (await api.get("/users/reps")).data;
};

export const approveUser = async (id: number, payload: ApproveUserPayload): Promise<boolean> => {
    return (await api.post(`/users/approve/${id}`, payload)).data;
};
// Grants or revoke user access
export const updateUserAccess = async (id: number, payload: ApproveUserPayload): Promise<boolean> => {
    return (await api.put(`/users/${id}/access`, payload)).data;
};
// Grant access to user
export const grantAccess = async (id: number): Promise<boolean> => {
    return (await api.post(`/users/grant-access/${id}`)).data;
};
// Revokes user access
export const revokeAccess = async (id: number): Promise<boolean> => {
    return (await api.post(`/users/revoke-access/${id}`)).data;
};