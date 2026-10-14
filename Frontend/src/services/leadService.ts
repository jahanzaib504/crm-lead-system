import api from "../api";
import type { Lead } from "../types/Lead";

interface LeadForm{
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  jobTitle: string;
  title: string;
  query: string,
  source: string;
  estimatedValue: number | string;
}


const createLeadWithContact = async (payload: LeadForm): Promise<boolean> => {
    return (await api.post("/leads/create", payload)).data;
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
