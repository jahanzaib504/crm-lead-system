import api from "../api";
import type { Lead } from "../types/Lead";
import type { PaginationResponse } from "../types/PaginationResponse";

export interface LeadForm {
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    jobTitle: string;
    title: string;
    query: string;
    source: string;
    estimatedValue: number | string;
}

export interface GetLeadsParams {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    stageId?: number;
    openPool?: boolean;
}

export const createLeadWithContact = async (payload: LeadForm): Promise<boolean> => {
    return (await api.post("/leads/create", payload)).data;
};

export const createLead = async (payload: any): Promise<Lead> => {
    return (await api.post("/leads", payload)).data;
};

export const getLead = async (id: number): Promise<Lead> => {
    return (await api.get(`/leads/${id}`)).data;
};

export const getLeadsPaged = async (
    pageNumber = 1,
    pageSize = 10,
    search?: string,
    stageId?: number,
    openPool?: boolean
): Promise<PaginationResponse<Lead>> => {
    const params: Record<string, any> = { pageNumber, pageSize };
    if (search) params.search = search;
    if (stageId) params.stageId = stageId;
    if (openPool !== undefined) params.openPool = openPool;

    return (await api.get("/leads", { params })).data;
};

export const getOpenPoolLeads = async (): Promise<Lead[]> => {
    return (await api.get("/leads/pool")).data;
};

export const acquireLead = async (id: number): Promise<boolean> => {
    return (await api.post(`/leads/${id}/acquire`)).data;
};

export const assignLead = async (id: number, userId: number): Promise<boolean> => {
    return (await api.post(`/leads/${id}/assign`, { userId, salesRepId: userId, managerId: userId })).data;
};

export const updateLeadStage = async (id: number, stageId: number): Promise<boolean> => {
    return (await api.patch(`/leads/${id}/stage`, { stageId })).data;
};

export const updateLead = async (id: number, payload: Partial<Lead>): Promise<boolean> => {
    return (await api.put(`/leads/${id}`, payload)).data;
};

export const deleteLead = async (id: number): Promise<boolean> => {
    return (await api.delete(`/leads/${id}`)).data;
};

export const withdrawLead = async (id: number): Promise<boolean> => {
    return (await api.post(`/leads/${id}/withdraw`)).data;
}