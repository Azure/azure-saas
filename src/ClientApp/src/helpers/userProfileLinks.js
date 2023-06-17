import { MdArrowDropDown } from "react-icons/md";
export const userMenus = [
  {
    title: (
      <MdArrowDropDown className="opacity-70 cursor-pointer" fontSize={26} />
    ),
    submenu: true,
    sublinks: [
      {
        Head: "Set Credentials",
        sublink: [
          { name: "Dashbord", link: "" },
          { name: "Sign Out", link: "" },
        ],
      },
    ],
  },
];
