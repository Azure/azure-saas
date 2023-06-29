import { dropDownMenuSource } from "../../data/menu";
import { handleExporting } from "../../helpers/datagridFunctions";

const DropDownMenu = () => {
  return (
    <>
      {dropDownMenuSource.map((link) => (
        <main key={link.id} className="flex flex-col">
          <section className="cursor-pointer group">
            <h1 className="flex items-center gap-1">
              {link?.icon} {link?.title} {link?.dropArrow}
            </h1>
            {link.submenu && (
              <section>
                <section className="absolute top-9  z-50 hidden  group-hover:block hover:block">
                  <article className="bg-bgDropDown rounded-md px-5  shadow-xl">
                    {link.sublinks.map((mysublinks) => (
                      <div
                        key={mysublinks.Head}
                        className="flex flex-col mt-2 justify-between"
                      >
                        {mysublinks.sublink.map((slink) => (
                          <li
                            key={slink.name}
                            className="text-xs flex gap-1 items-center text-dropDown py-2.5 hover:bg-bgxxLight"
                            onClick={() => handleExporting(slink.name)}
                          >
                            {slink.icon} {slink.name}
                          </li>
                        ))}
                      </div>
                    ))}
                  </article>
                </section>
              </section>
            )}
          </section>
        </main>
      ))}
    </>
  );
};

export default DropDownMenu;
