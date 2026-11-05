import api from "../api";
import type { ApiResponse } from "../types/ApiResponse";
import type { Note, CreateNotePayload } from "../types/Note";

export const getNotesForLead = async (leadId: number): Promise<ApiResponse<Note[]>> => {
    return (await api.get<ApiResponse<Note[]>>(`/notes/lead/${leadId}`)).data;
};

export const createNote = async (payload: CreateNotePayload): Promise<ApiResponse<Note>> => {
    return (await api.post<ApiResponse<Note>>("/notes", payload)).data;
};
