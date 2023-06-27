import classNames from "classnames";
import { MdOutlineClose } from "react-icons/md";

const CustomActionModal = ({ isOpen, title, handleClose, children }) => {
  return (
    <main
      className={classNames({
        "flex flex-col bg-white shadow-2xl": true,
        "right-0 overflow-auto top-0 md:top-[40px] py-1.5 md:z-0 z-50 fixed": true,
        "md:h-[calc(98vh_-_40px)] h-full w-[300px] md:w-[410px]": true,
        "transition-transform .3s ease-in-out md:translate-x-0": true,
        "-translate-x-full": !isOpen,
        hidden: !isOpen,
      })}
    >
      <section className="px-5 py-1.5 border-b border-gray-300 flex w-full items-center justify-between sticky inset-x-0 top-0 z-50">
        <h1 className="text-menuHeading  text-sm w-full font-semibold">
          Approve
        </h1>
        <div className="px:2 md:px-5">
          <MdOutlineClose
            onClick={handleClose}
            className="text-lg hover:text-xl text-menuHeading opacity-60 cursor-pointer"
          />
        </div>
      </section>
      <section className="mt-5 h-full overflow-y-auto">{children}</section>
    </main>
  );
};

export default CustomActionModal;
