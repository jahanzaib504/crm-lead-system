import api from "../api"
import type { Company } from "../types/Company";

const getCompanies = async (): Promise<Company[]> => {
        return (await api.get("/companies")).data;
}
const getCompanyById = async (id: string): Promise<Company> => {
        return (await api.get(`/companies/${id}`)).data;
}

export {getCompanies, getCompanyById};