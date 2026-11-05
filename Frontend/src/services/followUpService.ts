import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { FollowUp, CreateFollowUpPayload } from "../types/FollowUp";

export const getFollowUpsByLead = async (leadId: number): Promise<ApiResponse<FollowUp[]>> => {
    return (await api.get<ApiResponse<FollowUp[]>>(`/followups/lead/${leadId}`)).data;
};

export const getMyPendingFollowUps = async (): Promise<ApiResponse<FollowUp[]>> => {
    return (await api.get<ApiResponse<FollowUp[]>>("/followups/my-pending")).data;
};

export const createFollowUp = async (payload: CreateFollowUpPayload): Promise<ApiResponse<FollowUp>> => {
    return (await api.post<ApiResponse<FollowUp>>("/followups", payload)).data;
};

export const updateFollowUpStatus = async (id: number, status: string): Promise<ApiResponse<boolean>> => {
    return (await api.patch<ApiResponse<boolean>>(`/followups/${id}/status`, { status })).data;
};

export const deleteFollowUp = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.delete<ApiResponse<boolean>>(`/followups/${id}`)).data;
};
