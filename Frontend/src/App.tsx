
import './App.css'
import { Routes, Route } from 'react-router'
import LoginSignup from './components/LoginSignup'
import { CreateLeadPage } from './pages/CreateLeadPage'
import { AuthRoute } from './components/AuthRoute'
import { useEffect } from 'react'
import { getMe } from './services/userService'
import { useUser } from './store/userStore'
import { IndexPage } from './pages/IndexPage'
import { Dashboard } from './pages/Dashboard'

function App() {
  const setUserStore = useUser((state) => state.setUser);
  const loading = useUser((state)=> state.loading);
  const setLoading = useUser((state)=> state.setLoading);

  useEffect(() => {
    // Runs at the start of application
    const fetchUser = async () => {
      setLoading(true);

      try {  
        console.log("Fetching current user");
        const user = await getMe();
        setUserStore(user);
        console.log("Current user", user);
      } catch(e:any) {
        console.log(e.response.data);
      }
      finally{
        setLoading(false);
      }
    }
    fetchUser();
  }, [])

  return (
    <Routes>
      <Route path='/login' element={<LoginSignup isLoginPage={true} />}></Route>
      <Route path='/register' element={<LoginSignup isLoginPage={false} />}></Route>

      <Route path='/' element={<IndexPage />}/>

      <Route path='/create-lead' element={<CreateLeadPage />}></Route>
      
      <Route element={<AuthRoute />}>
        <Route path='/dashboard' element={<Dashboard/>} />

      </Route>
    </Routes>
  )
}

export default App
