import './App.css';
import { Routes, Route } from 'react-router';
import LoginSignup from './components/LoginSignup';
import { CreateLeadPage } from './pages/CreateLeadPage';
import { AuthRoute } from './components/AuthRoute';
import { AppLayout } from './components/AppLayout';
import { useEffect } from 'react';
import { getMe } from './services/userService';
import { useUser } from './store/userStore';
import { IndexPage } from './pages/IndexPage';
import { Dashboard } from './pages/Dashboard';
import { LeadsPage } from './pages/LeadsPage';
import { SingleLeadPage } from './pages/SingleLeadPage';
import { FollowUpsPage } from './pages/FollowUpsPage';
import { AdminUsersManagementPage } from './pages/AdminUsersMangementPage';
import { Toaster } from 'react-hot-toast';
import { AuditLogsPage } from './pages/AuditLogsPage';

function App() {
  const setUserStore = useUser((state) => state.setUser);
  const setLoading = useUser((state) => state.setLoading);

  useEffect(() => {
    const fetchUser = async () => {
      const token = localStorage.getItem("token");
      if (!token) {
        setLoading(false);
        return;
      }

      setLoading(true);
      try {
        const user = await getMe();
        setUserStore(user);
      } catch (e: any) {
        console.error("Session expired or invalid:", e);
        localStorage.removeItem("token");
        setUserStore(null);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, [setUserStore, setLoading]);

  return (
    <>
      <Toaster toastOptions={{ duration: 1000 }} />
      <Routes>
        {/* Public Routes */}
        <Route path='/' element={<IndexPage />} />
        <Route path='/login' element={<LoginSignup isLoginPage={true} />} />
        <Route path='/register' element={<LoginSignup isLoginPage={false} />} />
        <Route path='/create-lead' element={<CreateLeadPage />} />

        {/* Authenticated Workspace Routes */}
        <Route element={<AuthRoute />}>
          <Route element={<AppLayout />}>
            <Route path='/dashboard' element={<Dashboard />} />
            <Route path='/leads' element={<LeadsPage key="all-leads"/>} />
            <Route path='/leads/:id' element={<SingleLeadPage />} />
            <Route path='/pool' element={<LeadsPage openPoolOnly={true} key="pool-leads"/>} />
            <Route path='/followups' element={<FollowUpsPage />} />
            <Route path='/admin/users' element={<AdminUsersManagementPage />} />
            <Route path='/audit-logs' element={<AuditLogsPage />} />
          </Route>
        </Route>
      </Routes>
    </>
  );
}

export default App;
