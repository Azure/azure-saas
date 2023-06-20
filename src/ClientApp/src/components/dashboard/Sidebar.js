import React from "react";
import classNames from "classnames";
import SideLinks from "./SideLinks";

const Sidebar = ({ showSidebar, openSidebar }) => {
  return (
    <main
      className={classNames({
        "flex md:py-2 flex-col bg-sidebarBg  text-sideColor": true,
        // " text-sideMenu": true,
        "left-0 w-[250px] md:sticky overflow-auto md:top-[40px] md:z-0 top-0 z-20 fixed": true,
        "md:h-[calc(100vh_-_40px)] h-full md:w-2/12": true,
        "transition-transform .3s ease-in-out md:translate-x-0": true,
        "-translate-x-full": !showSidebar,
        "md:hidden": !openSidebar,
      })}
    >
      <section className="flex flex-col h-full justify-between">
        <section>
          <article className="w-full font-medium">
            <ul className="flex flex-col gap-2 text-left">
              <SideLinks />
            </ul>
          </article>
        </section>
      </section>
    </main>
  );
};

export default Sidebar;
