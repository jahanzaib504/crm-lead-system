import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { AuditLog } from "../types/AuditLog";
import type { PaginationResponse } from "../types/PaginationResponse";

export const getAuditLogsPaged = async (page:number, pageSize:number, search=null): Promise<ApiResponse<PaginationResponse<AuditLog>>> => {
    return (await api.get<ApiResponse<PaginationResponse<AuditLog>>>("/audit-logs", {params: {page, pageSize, search}})).data;
};