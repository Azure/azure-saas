import React from "react";
import "./about.css";
import data from "../../../data/about";

export const About = () => {
  return (
    <div className="about">
      <p className="hero-header">{data.title}</p>
      <h1 className="hero-lgheader">{data.subtitle}</h1>
      <p className="about-p">{data.description}</p>
    </div>
  );
};
