import api from "../api"
import type { User } from "../types/User";


const getMe = async(): Promise<User>=>{
    return (await api.get("/users/me")).data;
}

export {getMe}