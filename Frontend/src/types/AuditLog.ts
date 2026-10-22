export interface AuditLog{
    id: number,
    timestamp: string,
    performedByUserId: number,
    entityName: string,
    action: string,
    changesJson: string
}