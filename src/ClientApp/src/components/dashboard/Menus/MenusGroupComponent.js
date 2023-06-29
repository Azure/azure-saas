import React from "react";
import DesktopMenus from "./DesktopMenus";
import MobileMenus from "./MobileMenus";

const MenusGroupComponent = ({ menus, heading, onMenuClick }) => {
  return (
    <section>
      <DesktopMenus heading={heading} menus={menus} onMenuClick={onMenuClick} />
      <MobileMenus menus={menus} onMenuClick={onMenuClick} />
    </section>
  );
};

export default MenusGroupComponent;
