import React from "react";
import "./company.css";
import illustration from "../../../../assets/ill3.png";
import data from "../../../../data/pages/company";

export const CompanyInfo = () => {
  return (
    <div className="c-info" id="c-info">
      <p className="c-header">{data.company_info.header}</p>
      <div className="c-description">
        <p className="c-description-text">{data.company_info.description}</p>
        <img src={illustration} alt="illustration" className="c-illustration" />
      </div>
    </div>
  );
};
