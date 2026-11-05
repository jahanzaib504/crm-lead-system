import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { Activity, CreateActivityPayload } from "../types/Activity";

export const getActivitiesByLead = async (leadId: number): Promise<ApiResponse<Activity[]>> => {
    return (await api.get<ApiResponse<Activity[]>>(`/activities/lead/${leadId}`)).data;
};

export const getMyActivities = async (): Promise<ApiResponse<Activity[]>> => {
    return (await api.get<ApiResponse<Activity[]>>("/activities/my")).data;
};

export const createActivity = async (payload: CreateActivityPayload): Promise<ApiResponse<Activity>> => {
    return (await api.post<ApiResponse<Activity>>("/activities", payload)).data;
};

export const deleteActivity = async (id: number): Promise<ApiResponse<boolean>> => {
    return (await api.delete<ApiResponse<boolean>>(`/activities/${id}`)).data;
};
