import { MdArrowDropDown } from "react-icons/md";
export const menus = [
  {
    title: (
      <MdArrowDropDown className="opacity-70 cursor-pointer" fontSize={26} />
    ),
    submenu: true,
    sublinks: [
      {
        Head: "Set Credentials",
        sublink: [
          { name: "Update Profile", link: "/dashboard/profile" },
          { name: "Change Password", link: "" },
          { name: "Sign Out", link: "" },
        ],
      },
    ],
  },
];
