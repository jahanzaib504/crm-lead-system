
import { useLocation, Link } from "react-router";
import { useUser } from "../store/userStore";
import { 
  LayoutDashboard, 
  Users, 
  Contact, 
  Briefcase, 
  CheckSquare, 
  Settings, 
  ShieldAlert,
} from "lucide-react";

type NavItem = {
  label: string;
  path: string;
  icon: React.ElementType;
  roles?: string[]; // Allowed roles (if omitted, visible to all)
};

const NAV_ITEMS: NavItem[] = [
  { label: "Dashboard", path: "/dashboard", icon: LayoutDashboard },
  { label: "Leads", path: "/leads", icon: Contact, roles: ["Admin", "Sales Rep", "Sales Manager"] },
  { label: "Customers", path: "/customers", icon: Users, roles: ["Admin", "Sales Rep", "Sales Manager"] },
  { label: "Deals", path: "/deals", icon: Briefcase, roles: ["Admin", "Sales Rep", "Sales Manager"] },
  { label: "Tasks", path: "/tasks", icon: CheckSquare },
  { label: "Admin Panel", path: "/admin", icon: ShieldAlert, roles: ["Admin"] },
  { label: "Settings", path: "/settings", icon: Settings },
];

export const SideBar = () => {
  const role = useUser((state) => state.user?.role);
  const location = useLocation();

  // Filter links based on current user role
  const filteredNavItems = NAV_ITEMS.filter(
    (item) => !item.roles || (role && item.roles.includes(role.name))
  );

  return (
    <aside className="w-52 h-screen bg-slate-900 text-slate-300 flex flex-col border-r border-slate-800">
      {/* App Branding */}
      <div className="h-16 flex items-center px-6 border-b border-slate-800">
        <h1 className="text-xl font-bold text-white tracking-wide">CRM Pro</h1>
      </div>

      {/* Navigation Links */}
      <nav className="flex-1 px-4 py-6 space-y-1 overflow-y-auto">
        {filteredNavItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname === item.path;

          return (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                isActive
                  ? "bg-indigo-600 text-white"
                  : "text-slate-400 hover:bg-slate-800 hover:text-slate-200"
              }`}
            >
              <Icon className="w-5 h-5" />
              <span>{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
};