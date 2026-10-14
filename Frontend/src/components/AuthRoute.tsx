
import { useUser } from "../store/userStore";
import { Navigate, Outlet } from "react-router";


export const AuthRoute = () =>{
    const user = useUser((state)=> state.user);
    const loading = useUser((state)=> state.loading);

    if(loading)
        return <div>Loading.....</div>
    
    if(user == null)
        return <Navigate to="/login"/>
    
    return <Outlet/>

}