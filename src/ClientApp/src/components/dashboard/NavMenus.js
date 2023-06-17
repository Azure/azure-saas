import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import { menus } from "../../helpers/myNavLinks";
import { logout } from "../../redux/userSlice";

const NavMenus = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const currentUser = useSelector((state) => state.user?.currentUser?.user);

  const handleLogOut = () => {
    dispatch(logout());
    window.location = "/MicrosoftIdentity/Account/SignOut"
  };

  const handleClick = (link) => {
    if (link === "Sign Out") {
      handleLogOut();
    } else if (link === "Update Profile") {
      navigate("/dashboard/profile");
    }
  };

  return (
    <>
      {menus.map((link) => (
        <main key={link.title} className="flex flex-col">
          <section className="cursor-pointer group">
            <h1>{link?.title}</h1>
            {link.submenu && (
              <section>
                <section className="absolute top-6 md:top-8 right-16 md:right-16 z-50 hidden  group-hover:block hover:block">
                  <article className="bg-bg h-16"></article>
                  <article className="rounded-full overflow-hidden  w-20 h-20 flex items-center justify-center cursor-pointer absolute top-6 right-0 left-0 m-auto">
                    <img
                      className="w-full h-full object-cover"
                      src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=687&q=80"
                      alt="profile"
                    />
                  </article>
                  <article className="bg-sidebarHeading p-5  shadow-xl">
                    <article className="flex flex-col mt-6 items-center">
                      <h3 className="font-medium text-heading">
                        {currentUser?.fullName}
                      </h3>
                      <span className="text-[14px]  text-heading font-normal">
                        {currentUser?.email}
                      </span>
                    </article>
                    {link.sublinks.map((mysublinks) => (
                      <div
                        key={mysublinks.Head}
                        className="flex flex-col mt-2 justify-between"
                      >
                        {mysublinks.sublink.map((slink) => (
                          <li
                            key={slink.name}
                            className="text-sm text-heading py-2.5 hover:bg-bgDark"
                            onClick={() => handleClick(slink.name)}
                          >
                            <Link to={slink.link}>{slink.name}</Link>
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

export default NavMenus;
