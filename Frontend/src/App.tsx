
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
import { SideBar } from './components/sidebar'
import { LeadsPage } from './pages/LeadsPage'
import { SingleLeadPage } from './pages/SingleLeadPage'


function App() {
  const setUserStore = useUser((state) => state.setUser);
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
      <Route path='/s' element={<SideBar/>}/>
      <Route element={<AuthRoute />}>
        <Route path='/dashboard' element={<Dashboard/>} />
        <Route path='/leads' element={<LeadsPage/>} />
        <Route path='/leads/:id' element={<SingleLeadPage/>} />
      </Route>
    </Routes>
  )
}

export default App
