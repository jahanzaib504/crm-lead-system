import type { Contact } from "./Contact";
import type { LeadStage } from "./LeadStage";
import type { User } from "./User";

export interface Lead {
    id: number;
    title: string;
    query: string;
    source: string;
    leadStageId: number;
    assignedToUserId: number | null;
    assignedToUserName?: string | null;
    managerId?: number | null;
    managerName?: string | null;
    contactId: number;
    contactName?: string | null;
    contactEmail?: string | null;
    contactPhone?: string | null;
    contactJobTitle?: string | null;
    companyName?: string | null;
    currentScore?: number | null;
    estimatedValue?: number | null;

    User?: User | null;
    leadStage?: LeadStage | null;
    contact?: Contact | null;

    isActive?: boolean;
    createdAt?: string | null;
    createdBy?: string | null;
    updatedAt?: string | null;
    updatedBy?: string | null;
}
