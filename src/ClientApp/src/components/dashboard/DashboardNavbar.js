import { RiMenuUnfoldFill } from "react-icons/ri";
import { MdOutlineClose } from "react-icons/md";
import { MdOutlineSearch } from "react-icons/md";
import NavMenus from "./NavMenus";
import SearchCategories from "./SearchCategories";
import { useState } from "react";
import Module from "./Module";

const DashboardNavbar = ({ onMenuButtonClick, onMenuClick }) => {
  const handleSubmit = (e) => e.preventDefault();
  const [searchMode, setSearchMode] = useState(false);
  const [searchInput, setSearchInput] = useState("");

  return (
    <main className="w-full p-1 md:p-4 bg-bg flex sticky top-0 z-10 items-center h-[40px] box-border text-navColor">
      <section className=" w-full flex items-center justify-between">
        <article className="hidden md:flex items-center gap-5">
          <RiMenuUnfoldFill
            onClick={onMenuClick}
            className=" opacity-50 cursor-pointer"
            fontSize={20}
          />
          <h1 className="font-semibold text-base cursor-pointer">
            ARBS Customer Portal
          </h1>
        </article>

        <article className="hidden md:flex gap-4  ">
          <form
            onSubmit={handleSubmit}
            className="flex  w-[400px]  px-2 items-center border border-gray-300 overflow-hidden rounded-3xl justify-between "
          >
            <MdOutlineSearch fontSize={20} />
            <input
              type="text"
              className="py-1.5  px-2 w-full outline-none bg-transparent  text-xs  placeholder:text-gray-200 placeholder:text-xs"
              placeholder="Search for..."
              name="search"
              onChange={(e) => setSearchInput(e.target.value)}
            />
            <SearchCategories
              searchInput={searchInput}
              setSearchInput={setSearchInput}
            />
          </form>
          <article className=" flex items-center font-medium">
            <article className="flex items-center gap-1">
              <article className="rounded-full overflow-hidden  w-7 h-7 flex items-center justify-center cursor-pointer">
                <img
                  className="w-full object-cover"
                  src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=687&q=80"
                  alt="profile"
                />
              </article>
              <NavMenus />
            </article>
          </article>
          <article className="self-center">
            <Module />
          </article>
        </article>
        {searchMode ? (
          <form
            onSubmit={handleSubmit}
            className="md:hidden w-full rounded-md border border-gray-300 overflow-hidden"
          >
            <div className="flex w-full items-center  justify-between ">
              <input
                type="text"
                className="py-1.5 px-2 w-full outline-none bg-transparent text-xs  placeholder:text-gray-200 placeholder:text-xs"
                placeholder="Search for..."
                name="search"
                onChange={(e) => setSearchInput(e.target.value)}
              />
              <MdOutlineClose
                onClick={() => setSearchMode(false)}
                fontSize={18}
                className="opacity-60 cursor-pointer"
              />
            </div>

            <SearchCategories
              searchInput={searchInput}
              setSearchInput={setSearchInput}
            />
          </form>
        ) : (
          <article className="md:hidden flex items-center w-full justify-between">
            <article className="items-center gap-2">
              <h1 className="font-semibold text-base cursor-pointer">
                ARBS Customer Portal
              </h1>
            </article>
            <article className="flex items-center md:hidden gap-1 ">
              <MdOutlineSearch
                onClick={() => setSearchMode(true)}
                fontSize={20}
                className="opacity-60 cursor-pointer"
              />
              <article className=" flex items-center font-medium">
                <article className="flex items-center gap-1">
                  <article className="rounded-full overflow-hidden w-6 h-6 md:w-8 md:h-8 flex items-center justify-center cursor-pointer">
                    <img
                      className="w-full object-cover"
                      src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=687&q=80"
                      alt="profile"
                    />
                  </article>
                  <NavMenus />
                </article>
              </article>
              <div
                onClick={onMenuButtonClick}
                className="space-y-1 p-1.5 md:hidden cursor-pointer z-40"
              >
                <RiMenuUnfoldFill
                  className=" opacity-50 cursor-pointer "
                  fontSize={20}
                />
              </div>
              <article className="self-center">
                <Module />
              </article>
            </article>
          </article>
        )}
      </section>
    </main>
  );
};

export default DashboardNavbar;
