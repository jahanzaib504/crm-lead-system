import { useState } from "react"

// Role has 
const LoginSignup = ({ isLoginPage = false }) => {

    const [user, setUser] = useState({ fullName: "", email: "", password: "", role: "Sales Rep" });
    const [errors, setErrors] = useState({ fullName: null, email: null, password: null });

    const roles = ["Admin", "Sales Rep", "Sales Manager"];

    const HandleError = (field:string, message:string)=>{
        setErrors((prev)=>({...prev, [field]: message}))
    }
    const handleChange = (e: any) => {
        // Reseting errors
        setErrors((prev)=>({...prev, [e.target.id]: null}));
        setUser((prev) => ({ ...prev, [e.target.id]: e.target.value }));
    }

    const handleSubmit = (e: any) => {
        e.preventDefaults();
        
        if(!validate())
            return;


        if(isLoginPage){

        }else{

        }
    }

    const validate = ()=>{
        if(user.password.length < 6){
             HandleError("password", "Password must be of 6 length");
            return false;
            }
    }
    const handleRoleClick = (role: string)=>{
        return ()=> setUser((prev)=>({...prev, role: role}));
    }

    return (
        <div>
            <div>
                <form onSubmit={handleSubmit}>
                    { !isLoginPage &&
                    <div>
                        <label htmlFor="fullName">Full Name</label>
                        <input type="text" onChange={handleChange} id="fullName" />
                    </div>
                    }


                    <div>
                        <label htmlFor="email">Email</label>
                        <input type="text" onChange={handleChange} id="email" />
                    </div>

                    <div>
                        <label htmlFor="password">Password</label>
                        <input type="text" onChange={handleChange} id="password" />
                    </div>
                    {roles.map((crole)=>(
                        <div className={`p-2 ${crole == user.role? "text-red-500":""}`} onClick={handleRoleClick(crole)}>
                            {crole}
                        </div>
                    ))}
                </form>
            </div>
        </div>
    )
}