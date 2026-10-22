import api from "../api";
import type { Activity, CreateActivityPayload } from "../types/Activity";

export const getActivitiesByLead = async (leadId: number): Promise<Activity[]> => {
    return (await api.get(`/activities/lead/${leadId}`)).data;
};

export const getMyActivities = async (): Promise<Activity[]> => {
    return (await api.get("/activities/my")).data;
};

export const createActivity = async (payload: CreateActivityPayload): Promise<Activity> => {
    return (await api.post("/activities", payload)).data;
};

export const deleteActivity = async (id: number): Promise<boolean> => {
    return (await api.delete(`/activities/${id}`)).data;
};
