import { MdOutlineClose, MdOutlineDeleteOutline } from "react-icons/md";
import { TbBrandBooking } from "react-icons/tb";
import { ImUndo2 } from "react-icons/im";

const ConfirmationPopupComponent = ({
  handleClose,
  title,
  statusBarText,
  onDelete,
}) => {
  return (
    <main className="bg-white w-full md:w-[600px]  mx-auto h-fit">
      <section className="sticky w-full inset-x-0 top-0 z-50">
        <article className="bg-formHeading flex items-center justify-between">
          <div className="flex items-center py-1 px:2 md:px-5 w-full gap-1 text-formHeadingColor">
            <TbBrandBooking />
            <p className="text-xs opacity-90">{title}</p>
          </div>
          <div className="px:2 md:px-5">
            <MdOutlineClose
              onClick={handleClose}
              className="text-lg hover:text-xl text-formHeadingColor opacity-60 cursor-pointer"
            />
          </div>
        </article>
      </section>
      <section className="w-full p-3 text-xs flex items-center justify-center">
        <p>Are you sure you want to delete this booking?</p>
      </section>
      <section className="sticky border-t border-gray-200 w-full inset-x-0 bottom-0 ">
        <article className="flex bg-white px-2 pb-1 justify-center items-center gap-4">
          <button
            onClick={onDelete}
            className="flex gap-1 border-none  hover:bg-gray-200 py-1 px-4 w-fit bg-white text-menuText items-center font-medium  cursor-pointer text-xs"
          >
            <MdOutlineDeleteOutline fontSize={20} /> Delete
          </button>
          <button
            onClick={handleClose}
            className="flex gap-1 border-none  hover:bg-gray-200 py-1 px-4 w-fit bg-white text-menuText items-center font-medium  cursor-pointer text-xs"
          >
            <ImUndo2 fontSize={18} />
            Cancel
          </button>
        </article>
        <article className="flex bg-formTitle text-formHeadingColor py-1 px:2 md:px-5 w-full">
          <p className="text-xs opacity-90">{statusBarText}</p>
        </article>
      </section>
    </main>
  );
};

export default ConfirmationPopupComponent;
