import React from "react";
import data from "../../../data/copyright";
import copy from "../../../data/footer";
import "./copyright.css";

export const Copyright = () => {
  return (
    <div className="copyright">
      <div className="copyright-left">
        {data.map((link) => (
          <a href={link.to} key={link.value} className="copyright-link">
            {link.value}
          </a>
        ))}
      </div>
      <div className="copyright-right">&copy; {copy.copyright.subtitle}</div>
    </div>
  );
};
