import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { DashboardAnalytics } from "../types/Analytics";

export const getDashboardAnalytics = async (): Promise<ApiResponse<DashboardAnalytics>> => {
    return (await api.get<ApiResponse<DashboardAnalytics>>("/analytics/dashboard")).data;
};
