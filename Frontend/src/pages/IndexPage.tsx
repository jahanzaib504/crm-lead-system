import { Link } from "react-router"


export const IndexPage = ()=>{
    return (
        <div>
            Welcome to crm lead system
            <div>Navigate to <Link to="/login" className="underline">Login page</Link></div>
            <div>Navigate to <Link to="/dashboard" className="underline">Dashboard page</Link></div>
        </div>
    )
}