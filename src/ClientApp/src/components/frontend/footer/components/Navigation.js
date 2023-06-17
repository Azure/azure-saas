import React, { useRef, useEffect } from "react";
import data from "../../../../data/footer";
import { Link } from "react-router-dom";
import Brand from "../../UI/Brand";
import { footermenus } from "../../../../helpers/footerMenusSource";

export const Navigation = () => {
  const scrollRef = useRef(null);

  useEffect(() => {
    if (scrollRef.current) {
      window.scrollTo({
        top: 0,
        behavior: "smooth",
      });
    }
  }, [scrollRef]);
  return (
    <div className="navigation">
      <div className="navigation-area">
        {footermenus?.map((menu) => (
          <div key={menu?.id} className="nav-area">
            <h3>{menu?.title}</h3>
            <ul>
              {menu?.items?.map((link) => (
                <li key={link.id}>{link.name}</li>
              ))}
            </ul>
          </div>
        ))}
      </div>
      <div className="nav-copy">
        <Link to="/" className="brand-logo">
          <Brand className="brand-logo" />
        </Link>
        <p className="nav-copy-h">&copy; {data.navlinks.copy}</p>
      </div>
    </div>
  );
};
