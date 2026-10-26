import { Outlet } from "react-router";
import { SideBar } from "./sidebar";
import { TopNavbar } from "./TopNavbar";

export const AppLayout = () => {
  return (
    <div className="flex min-h-screen bg-slate-50 font-sans text-slate-800">
      <SideBar />
      <div className="flex-1 flex flex-col min-w-0 sm:ml-(--sidebar-margin)">
        <TopNavbar />
        <main className="flex-1 p-6 md:p-8 max-w-7xl w-full mx-auto">
          <Outlet />
        </main>
      </div>
    </div>
  );
};
