
import api from "../api"
import type { User } from "../types/User";

interface LoginResponse{
    token: string,
    refreshToken: string,
    expiresAt: string
    user: User
}
interface RegsiterResponse{
    token: string,
    refreshToken: string,
    expiresAt: string
    user: User
}
interface LoginArgs{
    email: string,
    password: string
}
interface RegisterArgs{
    email: string,
    password: string,
    fullName: string,
    role: string
}

const register = async({fullName, email, password, role}: RegisterArgs): Promise<RegsiterResponse> =>{
    const request = await api.post("/auth/register", {fullName, email, password, role});
    return request.data;
}

const login = async({email, password}: LoginArgs): Promise<LoginResponse> =>{
    const request = await api.post("/auth/login", {email, password});
    return request.data;
}
const logout = async() =>{

}

export {register, login, logout};