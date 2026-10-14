import {create} from "zustand";
import type { User } from "../types/User";

interface UserStore{
    user: User | null,
    loading: boolean,
    setLoading: (arg0: boolean)=>void,
    setUser: (user: User)=> void,
    updateUser: (fields: Partial<User>)=>void
}

const useUser = create<UserStore>((set)=>({
    user: null,
    loading: true,
    setUser: (user)=>{
        set({user});
    },
    setLoading: (loading)=> set({loading}),
    updateUser: (fields)=>{
         set((state) => ({
            user: state.user
                ? { ...state.user, ...fields }
                : null,
        }))
    }
}));

export {useUser};