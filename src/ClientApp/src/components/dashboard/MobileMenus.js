import { RiMenuUnfoldFill } from "react-icons/ri";
import { useState } from "react";

const MobileMenus = ({ onMenuClick, menus }) => {
  const [isExpanded, setIsExpanded] = useState(false);
  return (
    <main className="md:hidden">
      <div
        onClick={() => setIsExpanded(!isExpanded)}
        className="p-1.5 md:hidden cursor-pointer z-40"
      >
        <RiMenuUnfoldFill
          fontSize={18}
          className=" opacity-50 cursor-pointer"
        />
      </div>
      {isExpanded && (
        <ul className="top-0 left-0 bg-bgxxLight font-medium w-fit  z-20">
          {menus.map((menu) => (
            <li
              key={menu.id}
              className="flex items-center gap-1  text-xs text-menu px-5 py-1.5 cursor-pointer hover:bg-bgLight"
              onClick={() => onMenuClick(menu.title)}
            >
              {menu.icon}
              {menu.title}
            </li>
          ))}
        </ul>
      )}
    </main>
  );
};

export default MobileMenus;
