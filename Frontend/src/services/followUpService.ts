import api from "../api";
import type { FollowUp, CreateFollowUpPayload } from "../types/FollowUp";

export const getFollowUpsByLead = async (leadId: number): Promise<FollowUp[]> => {
    return (await api.get(`/followups/lead/${leadId}`)).data;
};

export const getMyPendingFollowUps = async (): Promise<FollowUp[]> => {
    return (await api.get("/followups/my-pending")).data;
};

export const createFollowUp = async (payload: CreateFollowUpPayload): Promise<FollowUp> => {
    return (await api.post("/followups", payload)).data;
};

export const updateFollowUpStatus = async (id: number, status: string): Promise<boolean> => {
    return (await api.patch(`/followups/${id}/status`, { status })).data;
};

export const deleteFollowUp = async (id: number): Promise<boolean> => {
    return (await api.delete(`/followups/${id}`)).data;
};
