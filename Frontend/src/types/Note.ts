export interface Note {
    id: number;
    leadId: number;
    content: string;
    createdAt: string;
    createdBy: string;
}

export interface CreateNotePayload {
    leadId: number;
    content: string;
}
