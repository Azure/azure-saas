import React from "react";
import "./company.css";
import illustration from "../../../../assets/illustrations/3.png";
import data from "../../../../data/pages/company";

export const Header = () => {
  return (
    <div className="company">
      <div className="company-illustration">
        <img src={illustration} alt="company-illustration" />
      </div>
      <div className="company-info">
        <h4 className="info-header">{data.header_section.header}</h4>
        <p className="info-description">{data.header_section.description}</p>
      </div>
    </div>
  );
};
