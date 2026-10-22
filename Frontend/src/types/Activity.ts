export interface Activity {
    id: number;
    leadId: number;
    activityType: "Call" | "Meeting" | "Email" | "Demo";
    subject: string;
    summary?: string | null;
    activityDate: string;
    loggedByUserId: number;
    loggedByUserName?: string | null;
    createdAt: string;
}

export interface CreateActivityPayload {
    leadId: number;
    activityType: "Call" | "Meeting" | "Email" | "Demo";
    subject: string;
    summary?: string;
    activityDate?: string;
}
