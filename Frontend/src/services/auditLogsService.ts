import api from "../api";
import type { AuditLog } from "../types/AuditLog";
import type { PaginationResponse } from "../types/PaginationResponse";

export const getAuditLogsPaged = async (): Promise<PaginationResponse<AuditLog>> => {
    return (await api.get("/audit-logs")).data;
};