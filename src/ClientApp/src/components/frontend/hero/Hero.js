import React from "react";
import "./hero.css";
import illustration from "../../../assets/illustrations/1.png";
import { LazyLoadImage } from "react-lazy-load-image-component";
import data from "../../../data/hero";
import { Link } from "react-router-dom";

export const Hero = () => {
  return (
    <div className="hero-content">
      <div className="info">
        <h1 className="landing-header">{data.title}</h1>
        <p className="lander-page-subtitle">{data.subtitle}</p>
        <button className="heroGetStarted">
          <Link to={process.env.REACT_APP_SIGNUPIN_URL}>Get Started</Link>
        </button>
      </div>
      <div className="hero-image-div">
        <LazyLoadImage
          src={illustration}
          className="hero-image"
          alt="Bannerimage"
        />
      </div>
    </div>
  );
};
