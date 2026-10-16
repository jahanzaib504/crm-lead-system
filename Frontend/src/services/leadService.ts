import api from "../api";
import type { Lead } from "../types/Lead";
import type { PaginationResponse } from "../types/PaginationResponse";

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

const getLead = async (id: number): Promise<Lead> =>{
    return (await api.get(`/leads/${id}`)).data;
}

const getLeadsPaged = async (pageNumber: number, pageSize=10): Promise<PaginationResponse<Lead>> => {
    return (await api.get("/leads", {params: {pageNumber, pageSize}})).data;
}


export { createLeadWithContact, createLead, getLead, getLeadsPaged };
