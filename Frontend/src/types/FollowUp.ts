export interface FollowUp {
    id: number;
    leadId: number;
    title: string;
    dueDate: string;
    status: "Pending" | "Completed" | "Cancelled";
    priority: "Low" | "Medium" | "High";
    assignedToUserId: number;
    assignedToUserName?: string | null;
    createdAt: string;
    createdBy: string;
}

export interface CreateFollowUpPayload {
    leadId: number;
    title: string;
    dueDate: string;
    priority: string;
}
