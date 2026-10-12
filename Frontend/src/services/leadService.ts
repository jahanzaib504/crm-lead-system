import api from "../api";
import type { Lead } from "../types/Lead";

const createLeadWithContact = async () => {
    return;
}

const createLead = async () =>{
    return;
}

const getLead = async (id: number) =>{
    return;
}

const getLeadsPaged = async (pageNumber: number, pageSize=10): Promise<Lead[]> => {
    return (await api.get<Lead[]>("/leads", {params: {pageNumber, pageSize}})).data;
}

export { createLeadWithContact, createLead, getLead, getLeadsPaged };
