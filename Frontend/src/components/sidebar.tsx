import { useLocation, Link } from "react-router";
import { useUser } from "../store/userStore";
import { 
  LayoutDashboard, 
  Contact, 
  Layers, 
  CalendarCheck, 
  Users, 
  PlusCircle,
  Building2,
  DockIcon
} from "lucide-react";

type NavItem = {
  label: string;
  path: string;
  icon: React.ElementType;
  roles?: string[]; // Allowed roles (if omitted, visible to all)
  badge?: string;
};

const NAV_ITEMS: NavItem[] = [
  { label: "Dashboard", path: "/dashboard", icon: LayoutDashboard },
  { label: "All Leads", path: "/leads", icon: Contact },
  { label: "Open Pool", path: "/pool", icon: Layers, roles: ["Admin", "Sales Manager"] },
  { label: "Follow-ups", path: "/followups", icon: CalendarCheck },
  { label: "User Management", path: "/admin/users", icon: Users, roles: ["Admin"] },
  { label: "Create Lead", path: "/create-lead", icon: PlusCircle },
  { label: "Audit Logs", path: "/audit-logs", icon:  DockIcon, roles: ["Admin"]},
];

export const SideBar = () => {
  const role = useUser((state) => state.user?.role);
  const location = useLocation();

  // Filter links based on current user role
  const filteredNavItems = NAV_ITEMS.filter(
    (item) => !item.roles || (role && item.roles.includes(role.name))
  );

  return (
    <aside className="w-60 min-h-screen bg-slate-900 text-slate-300 flex flex-col border-r border-slate-800 shrink-0">
      {/* App Branding */}
      <div className="h-16 flex items-center gap-3 px-6 border-b border-slate-800 bg-slate-950/40">
        <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold">
          <Building2 className="w-5 h-5" />
        </div>
        <div>
          <h1 className="text-base font-bold text-white tracking-wide leading-none">CRM Leads</h1>
          <span className="text-[10px] text-slate-400 font-medium tracking-wider uppercase">Lead Management</span>
        </div>
      </div>

      {/* Navigation Links */}
      <nav className="flex-1 px-3 py-6 space-y-1.5 overflow-y-auto">
        <div className="px-3 pb-2 text-[11px] font-semibold uppercase tracking-wider text-slate-500">
          Navigation
        </div>
        {filteredNavItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname === item.path || 
            (item.path !== "/dashboard" && item.path !== "/create-lead" && location.pathname.startsWith(item.path));

          return (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center justify-between px-3.5 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                isActive
                  ? "bg-blue-600 text-white shadow-sm font-semibold"
                  : "text-slate-400 hover:bg-slate-800/70 hover:text-slate-100"
              }`}
            >
              <div className="flex items-center gap-3">
                <Icon className={`w-4 h-4 ${isActive ? "text-white" : "text-slate-400"}`} />
                <span>{item.label}</span>
              </div>
              {item.badge && (
                <span className="text-xs bg-slate-800 text-slate-300 px-2 py-0.5 rounded-full font-semibold">
                  {item.badge}
                </span>
              )}
            </Link>
          );
        })}
      </nav>

      {/* Footer info */}
      <div className="p-4 border-t border-slate-800 text-xs text-slate-500 text-center">
        CRM Lead System &copy; 2026
      </div>
    </aside>
  );
};