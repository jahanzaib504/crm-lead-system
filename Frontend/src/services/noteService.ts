import api from "../api";
import type { Note, CreateNotePayload } from "../types/Note";

export const getNotesForLead = async (leadId: number): Promise<Note[]> => {
    return (await api.get(`/notes/lead/${leadId}`)).data;
};

export const createNote = async (payload: CreateNotePayload): Promise<Note> => {
    return (await api.post("/notes", payload)).data;
};
