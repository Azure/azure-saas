import React, { useState, useEffect } from "react";
import { MdArrowDropDown, MdArrowDropUp } from "react-icons/md";
import { RxDot } from "react-icons/rx";
import { Link } from "react-router-dom";
import { RiSettings5Fill, RiFolder3Fill } from "react-icons/ri";
import { TbReportSearch } from "react-icons/tb";
import { FaTools } from "react-icons/fa";
import { useSelector } from "react-redux";
import axios from "axios";

const SideLinks = () => {
  const [heading, setHeading] = useState("");
  const [moduleMenus, setModuleMenus] = useState([]);

  const partitionKey = useSelector(
    (state) => state.moduleCategory?.partitionKey
  );

  // Method to switch between each menu icons
  const getCategoryIcon = (title) => {
    switch (title) {
      case "Processess":
        return <RiSettings5Fill />;
      case "Inquiries":
        return <RiFolder3Fill />;
      case "Setups":
        return <FaTools />;
      case "Reports":
        return <TbReportSearch />;
      case "Member Services":
        return <RiSettings5Fill />;
      case "Other Services":
        return <RiFolder3Fill />;
      default:
        break;
    }
  };

  useEffect(() => {
    const url = process.env.REACT_APP_SIDEMENUS_URL;

    const getModuleMenus = async () => {
      try {
        const { data } = await axios.get(url);
        setModuleMenus(data);
      } catch (error) {
        console.log(error);
      }
    };
    getModuleMenus();
  }, []);

  const filteredMenus = moduleMenus?.filter((menu) => {
    if (partitionKey === "") {
      return menu?.partitionKey.toLowerCase().includes("procure2pay");
    } else {
      return menu?.partitionKey
        .toLowerCase()
        .includes(partitionKey.toLowerCase());
    }
  });

  return (
    <>
      {filteredMenus &&
        filteredMenus?.map((link) => {
          const icon = getCategoryIcon(link.title);

          return (
            <div key={link.id}>
              <div className="cursor-pointer px-1 overflow-y-auto w-full max-h-80 scrollbar-hide bg-blend-overlay">
                <div
                  className="font-semibold text-xs sticky pl-2 py-0.5 bg-sidebarHeading text-heading top-0 flex justify-between  items-center w-full"
                  onClick={() =>
                    heading !== link.title
                      ? setHeading(link.title)
                      : setHeading("")
                  }
                >
                  <div className="flex items-center gap-1">
                    {icon}
                    {link.title}
                  </div>
                  <div className="flex items-center">
                    {heading !== link.title ? (
                      <MdArrowDropUp fontSize={20} />
                    ) : (
                      <MdArrowDropDown fontSize={20} />
                    )}
                  </div>
                </div>
                {link.subMenu && (
                  <div
                    className={`
              ${heading === link.title && "hidden"}
              `}
                  >
                    <div>
                      <div className="px-2">
                        {link.subLinks.map((mysublinks) => (
                          <div
                            className="flex items-center gap-1 hover:bg-[#f5f5f5]"
                            key={mysublinks.name}
                          >
                            <RxDot />
                            <li className="text-xs font-normal text-sideMenu py-1.5">
                              <Link to={mysublinks.link}>
                                {mysublinks.name}
                              </Link>
                            </li>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          );
        })}
    </>
  );
};

export default SideLinks;
