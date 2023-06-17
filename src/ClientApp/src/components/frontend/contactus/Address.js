import React from "react";
import "./contactus.css";
import data from "../../../data/address";

export const Address = () => {
  return (
    <div className="support-information">
      <div className="support-info-header">
        <h2 className="support-info-header-title">
          {data.supportInfoHeaderTitle}
        </h2>
        <p className="support-info-header-subtitle">
          {data.supportInfoHeaderSubtitle}
        </p>
      </div>
      <div className="support-info-description">
        <p className="support-info-description-text">
          {data.supportInfoDescriptionText}
        </p>
      </div>
    </div>
  );
};
