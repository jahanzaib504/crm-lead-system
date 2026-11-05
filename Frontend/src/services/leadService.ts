import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
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

export const createLeadWithContact = async (payload: LeadForm): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>("/leads/create", payload)).data;
};

export const createLead = async (payload: any): Promise<ApiResponse<Lead>> => {
    return (await api.post<ApiResponse<Lead>>("/leads", payload)).data;
};

export const getLead = async (id: number): Promise<ApiResponse<Lead>> => {
    return (await api.get<ApiResponse<Lead>>(`/leads/${id}`)).data;
};

export const getLeadsPaged = async (
    pageNumber = 1,
    pageSize = 10,
    search?: string,
    stageId?: number,
    openPool?: boolean
): Promise<ApiResponse<PaginationResponse<Lead>>> => {
    const params: Record<string, any> = { pageNumber, pageSize };
    if (search) params.search = search;
    if (stageId) params.stageId = stageId;
    if (openPool !== undefined) params.openPool = openPool;

    return (await api.get<ApiResponse<PaginationResponse<Lead>>>("/leads", { params })).data;
};

export const getOpenPoolLeads = async (): Promise<ApiResponse<Lead[]>> => {
    return (await api.get<ApiResponse<Lead[]>>("/leads/pool")).data;
};

export const acquireLead = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/leads/${id}/acquire`)).data;
};

export const assignLead = async (id: number, userId: number): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/leads/${id}/assign`, { userId, salesRepId: userId, managerId: userId })).data;
};

export const updateLeadStage = async (id: number, stageId: number): Promise<ApiResponse<boolean>> => {
    return (await api.patch<ApiResponse<boolean>>(`/leads/${id}/stage`, { stageId })).data;
};

export const updateLead = async (id: number, payload: Partial<Lead>): Promise<ApiResponse<boolean>> => {
    return (await api.put<ApiResponse<boolean>>(`/leads/${id}`, payload)).data;
};

export const deleteLead = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.delete<ApiResponse<boolean>>(`/leads/${id}`)).data;
};

export const withdrawLead = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.post<ApiResponse<boolean>>(`/leads/${id}/withdraw`)).data;
}