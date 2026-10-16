import type { Company } from "./Company";

export interface Contact {
    firstName: string,
    lastName: string,
    email: string,
    phone: string,
    jobTitle: string,
    companyId: number,

    company: Company | null

    isActive: boolean,

    createdAt: string | null,
    createdBy: string | null,
    updatedAt: string | null,
    updatedBy: string | null,
    isDeleted: boolean | null
}