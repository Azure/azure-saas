import React, { useState } from "react";
import Brand from "../UI/Brand";
import { Navbutton } from "../UI/Button";
import "./navbar.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBars,
  faTimes,
  faAngleRight,
  faComments
} from "@fortawesome/free-solid-svg-icons";
import data from "../../../data/navbar";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { logout } from "../../../redux/userSlice";

export const Navbar = () => {
  const [toggleSidebar, setToggleNav] = useState(false);
  const currentUser = useSelector((state) => state.user?.currentUser?.user);
  const dispatch = useDispatch();

  const handleLogOut = () => {
    dispatch(logout());
  };

  const handleToggle = () => {
    if (toggleSidebar === false) {
      setTimeout(() => {
        document.getElementById("nav-mobile").style.left = "0";
      });
      document.getElementById("nav-mobile").style.display = "";

      setToggleNav(true);
    } else {
      document.getElementById("nav-mobile").style.left = "100vw";
      setTimeout(() => {
        document.getElementById("nav-mobile").style.display = "none";
      }, 500);
      setToggleNav(false);
    }
  };

  return (
    <>
      <div className="nav-mobile" id="nav-mobile">
        <div className="nav-mobile-content">
          {data.navmobilecontent.links.map((link) => (
            <Link
              key={link.key}
              className="nav-mobile-route"
              onClick={handleToggle}
              to={link.to}
            >
              <button className="nav-mobile-link">
                {link.value}
                <span className="nav-mobile-icon">
                  <FontAwesomeIcon icon={faAngleRight} />
                </span>
              </button>
            </Link>
          ))}
        </div>
      </div>
      <div className="navbar">
        <div className="brand">
          <Link to="/" className="brand-logo">
            <Brand className="brand-logo" />
          </Link>
          <div className="brand-nav-links">
            {data.navlinks.map((link) => (
              <Link to={link.to} key={link.key} className="brand-nav-link">
                <Navbutton className="brand-left" value={link.value} />
              </Link>
            ))}
          </div>
        </div>
        <div className="brand-links">
          {currentUser ? (
            <>
              <article className="flex items-center font-medium">
                <article className="flex items-center gap-1">
                  <h1 className="text-gray-600 font-medium">
                    Hello, {currentUser?.fullName}
                  </h1>
                </article>
              </article>
              <button onClick={handleLogOut} className="nav-signin-button">
                {data.signouttext}
              </button>
            </>
          ) : (
            <a href={process.env.REACT_APP_SIGNUPIN_URL}>
              <button className="nav-signin-button">{data.signintext}</button>
            </a>
          )}

          <FontAwesomeIcon
            icon={toggleSidebar ? faTimes : faBars}
            id="burger"
            onClick={handleToggle}
          />
        </div>
      </div>
      <div className="contact-section">
        <p className="contact-section-text">{data.chatText}</p>
        <FontAwesomeIcon icon={faComments} className="contact-section-icon"/>
      </div>
    </>
  );
};
