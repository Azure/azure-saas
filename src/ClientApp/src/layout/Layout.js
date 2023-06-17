import { useState } from "react";
import DashboardNavbar from "../components/dashboard/DashboardNavbar";
import Sidebar from "../components/dashboard/Sidebar";

const Layout = ({ children }) => {
  const [showSidebar, setShowSidebar] = useState(false);
  const [openSidebar, setOpenSidebar] = useState(true);

  return (
    <main className="grid h-screen grid-rows-header">
      <DashboardNavbar
        onMenuButtonClick={() => setShowSidebar((prev) => !prev)}
        onMenuClick={() => setOpenSidebar((prev) => !prev)}
      />
      <div className="flex w-full">
        <Sidebar showSidebar={showSidebar} openSidebar={openSidebar} />
        <div className="w-full">{children}</div>
      </div>
    </main>
  );
};

export default Layout;
