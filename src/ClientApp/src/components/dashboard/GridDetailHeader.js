import React from "react";
import MenuButtonsGroup from "./MenuButtonsGroup";
import MobileMenus from "./MobileMenus";

const GridDetailHeader = ({ menus, heading, onMenuClick }) => {
  return (
    <section>
      <MenuButtonsGroup
        heading={heading}
        menus={menus}
        onMenuClick={onMenuClick}
      />
      <article className="relative">
        <MobileMenus menus={menus} onMenuClick={onMenuClick} />
      </article>
    </section>
  );
};

export default GridDetailHeader;
