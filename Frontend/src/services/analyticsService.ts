import api from "../api";
import type { DashboardAnalytics } from "../types/Analytics";

export const getDashboardAnalytics = async (): Promise<DashboardAnalytics> => {
    return (await api.get("/analytics/dashboard")).data;
};
