import React from "react";
import "./testimonial.css";
import data from "../../../data/testimonial";
import TestimonyItem from "./TestimonyItem";

export const Testimonial = () => {
  return (
    <div className="testimonial">
      <div className="test-main">
        <h1 className="testimonial-lgheader">{data.testimony.subtitle}</h1>
        <p className="testimonial-content">{data.testimony.description}</p>
      </div>
      <TestimonyItem />
    </div>
  );
};
