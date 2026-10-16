import type { Contact } from "./Contact";
import type { LeadStage } from "./LeadStage";
import type { User } from "./User";

export interface Lead {
    id: number | null,
    title: string | null,
    query: string | null,
    source: string | null,
    leadStageId: number | null,
    assignedToUserId: number | null,
    contactId: number | null,
    currentScore: number,
    estimatedValue: number | 0,


    User: User | null,
    leadStage: LeadStage
    contact: Contact | null,

    isActive: boolean,

    createdAt: string | null,
    createdBy: string | null,
    updatedAt: string | null,
    updatedBy: string | null,
    isDeleted: boolean | null

}
