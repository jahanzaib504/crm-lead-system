import api from "../api";
import type { AuditLog } from "../types/AuditLog";
import type { PaginationResponse } from "../types/PaginationResponse";

export const getAuditLogsPaged = async (page:number, pageSize:number, search=null): Promise<PaginationResponse<AuditLog>> => {
    return (await api.get("/audit-logs", {params: {page, pageSize, search}})).data;
};