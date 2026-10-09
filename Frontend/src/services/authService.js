
import api from "../api"

const register = async({fullname, email, password, role}) =>{
    const request = await api.post("/auth/register", {fullname, email, password, role});
    return request.data;
}

const login = async({email, password}) =>{
    const request = await api.post("/auth/login", {email, password});
    return request.data;
}
const logout = async() =>{

}

module.exports = {register, login, logout};