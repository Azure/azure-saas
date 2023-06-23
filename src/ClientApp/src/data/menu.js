import { MdFreeCancellation } from "react-icons/md";
import { FcAddDatabase } from "react-icons/fc";
import { TbFileExport } from "react-icons/tb";
import { SiMicrosoftexcel } from "react-icons/si";
import { SlPrinter } from "react-icons/sl";
import { GrDocumentPdf } from "react-icons/gr";
import { RiFileEditFill } from "react-icons/ri";

import {
  MdArrowDropDown,
  MdOutlineSearch,
  MdOutlineAdd,
  MdOutlineLocalPrintshop,
  MdOutlineDeleteOutline,
  MdHelpOutline,
} from "react-icons/md";

export const homeMenuSource = [
  {
    id: 1,
    title: "Find",
    icon: <MdOutlineSearch fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 2,
    title: "New",
    icon: <MdOutlineAdd fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 3,
    title: "Export",
    icon: <MdOutlineLocalPrintshop fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 4,
    title: "Delete",
    icon: <MdOutlineDeleteOutline fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 5,
    title: "Close",
    icon: <MdFreeCancellation fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 6,
    title: "Help",
    icon: <MdHelpOutline fontSize={20} />,
    onClick: "handleClick",
  },
];

export const usersMenuSource = [
  {
    id: 1,
    title: "Find",
    icon: <MdOutlineSearch fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 2,
    title: "New",
    icon: <MdOutlineAdd fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 3,
    title: "Delete",
    icon: <MdOutlineDeleteOutline fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 4,
    title: "Close",
    icon: <MdFreeCancellation fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 5,
    title: "Help",
    icon: <MdHelpOutline fontSize={20} />,
    onClick: "handleClick",
  },
];

export const newMenuSource = [
  {
    id: 1,
    title: "Save",
    icon: <FcAddDatabase fontSize={"18px"} />,
    onClick: "handleClick",
  },
  {
    id: 2,
    title: "Close",
    icon: <MdFreeCancellation fontSize={20} />,
    onClick: "handleClick",
  },
];
export const updateMenuSource = [
  {
    id: 1,
    title: "Edit",
    icon: <RiFileEditFill fontSize={20} />,
    onClick: "handleClick",
  },

  {
    id: 2,
    title: "Delete",
    icon: <MdOutlineDeleteOutline fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 3,
    title: "Close",
    icon: <MdFreeCancellation fontSize={20} />,
    onClick: "handleClick",
  },
];
export const customActionsSource = [
  {
    id: 1,
    title: "Approve",
    onClick: "handleClick",
  },

  {
    id: 2,
    title: "Assign",
    onClick: "handleClick",
  },
  {
    id: 3,
    title: "Decline",
    onClick: "handleClick",
  },
  {
    id: 4,
    title: "Reverse",
    onClick: "handleClick",
  },
];
export const purchaseOrderMenu = [
  {
    id: 1,
    title: "Submit Order",
    icon: <SlPrinter fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 2,
    title: "Export",
    icon: <MdOutlineLocalPrintshop fontSize={20} />,
    onClick: "handleClick",
  },
  {
    id: 3,
    title: "Close",
    icon: <MdFreeCancellation fontSize={20} />,
    onClick: "handleClick",
  },
];

export const dropDownMenuSource = [
  {
    id: 1,
    title: "Export",
    icon: <TbFileExport fontSize={"18px"} />,
    dropArrow: (
      <MdArrowDropDown fontSize={26} className=" opacity-70 cursor-pointer" />
    ),
    onClick: "handleClick",
    submenu: true,
    sublinks: [
      {
        Head: "Set Credentials",
        sublink: [
          // { name: "Export selected rows to Excel", icon: <SiMicrosoftexcel /> },
          { name: "Export all data to Excel", icon: <SiMicrosoftexcel /> },
          // { name: "Export selected rows to PDF", icon: <GrDocumentPdf /> },
          { name: "Export all data to PDF", icon: <GrDocumentPdf /> },
        ],
      },
    ],
  },
];

export function handleClick(menu) {
  switch (menu) {
    case "Find":
      console.log("Find was clicked");
      break;
    case "New":
      console.log("New was clicked");
      break;
    case "Print Report":
      console.log("Print Report was clicked");
      break;
    case "Delete":
      console.log("Delete was clicked");
      break;
    case "Close":
      console.log("Close was clicked");
      break;
    case "Help":
      console.log("Help was clicked");
      break;

    default:
      break;
  }
}
