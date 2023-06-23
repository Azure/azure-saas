import DropDownMenu from "./DropDownMenu";

const CustomButtonComponent = ({ onClick, title, icon }) => {
  if (title === "Export") {
    return (
      <article className="flex gap-1 transition-all duration-100 hover:bg-gray-200 py-0.5 px-4 w-fit bg-menuBg text-menuText items-center font-medium  cursor-pointer text-xs">
        <DropDownMenu />
      </article>
    );
  }

  return (
    <article
      onClick={onClick}
      className="flex gap-1 transition-all duration-100 hover:bg-gray-200 py-0.5 px-4 w-fit bg-menuBg text-menuText items-center font-medium  cursor-pointer text-xs"
    >
      {icon}
      {title}
    </article>
  );
};

export default CustomButtonComponent;
