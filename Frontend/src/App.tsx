
import './App.css'
import { Routes, Route } from 'react-router'
import LoginSignup from './components/LoginSignup'
import { LeadPage } from './pages/LeadPage'
function App() {
 

  return (
    <Routes>
      <Route path='/login' element={<LoginSignup isLoginPage={true}/>}></Route>
      <Route path='/register' element={<LoginSignup isLoginPage={false}/>}></Route>

      <Route path='/create-lead' element={<LeadPage />}></Route>
    </Routes>
  )
}

export default App
