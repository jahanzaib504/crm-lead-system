import api from "../api"
import type { ApiResponse } from "../types/ApiResponse";
import type { Company } from "../types/Company";

const getCompanies = async (): Promise<ApiResponse<Company[]>> => {
        return (await api.get<ApiResponse<Company[]>>("/companies")).data;
}
const getCompanyById = async (id: string): Promise<ApiResponse<Company>> => {
        return (await api.get<ApiResponse<Company>>(`/companies/${id}`)).data;
}

export {getCompanies, getCompanyById};