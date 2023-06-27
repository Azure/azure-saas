import React, { useState } from "react";
import { FaToolbox } from "react-icons/fa";
import CustomActionsComponent from "../Menus/CustomActionsComponent";
import CustomActionModal from "../../modals/CustomActionModal";
import SalesApprovalComponent from "../SalesApprovalComponent";

const DetailsRightBar = ({ customAction }) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleClick = (action) => {
    switch (action) {
      case "Approve":
        setIsOpen(true);
        break;

      default:
        break;
    }
  };

  const handleClose = () => {
    setIsOpen(false);
  };

  return (
    <main className="flex  flex-col gap-4 box-border">
      <section className="flex flex-col gap-y-0.5 md:gap-1.5">
        <article className="p-2 flex flex-col gap-2.5">
          <div className="flex items-center gap-x-2 font-semibold text-sm">
            <FaToolbox className="text-menuText text-sm md:text-lg" /> Custom
            Actions
          </div>
        </article>
        <hr className="border h-0 border-gray-200" />
        <article className="flex flex-wrap items-center">
          {customAction.map((action) => (
            <CustomActionsComponent
              key={action.id}
              title={action.title}
              onClick={() => handleClick(action.title)}
            />
          ))}
        </article>
      </section>
      <CustomActionModal isOpen={isOpen} handleClose={handleClose}>
        <SalesApprovalComponent handleClose={handleClose} />
      </CustomActionModal>
    </main>
  );
};

export default DetailsRightBar;
