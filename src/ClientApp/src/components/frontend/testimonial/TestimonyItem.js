import React, { useState } from "react";
import "./testimonial.css";
import { LazyLoadImage } from "react-lazy-load-image-component";
import testimonies from "../../../data/testimonial";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAngleLeft, faAngleRight } from "@fortawesome/free-solid-svg-icons";

const TestimonyItem = () => {
  const data = testimonies.items;

  const [carousel, setCarousel] = useState(0);

  const handleCarouselRight = () => {
    let slides = (document.getElementsByClassName("t-item").length - 1) * 100;
    if (carousel === -slides) {
      setCarousel(0);
    } else {
      setCarousel(carousel - 100);
    }
  };

  const handleCarouselLeft = () => {
    let slides = (document.getElementsByClassName("t-item").length - 1) * 100;
    if (carousel === 0) {
      setCarousel(-slides);
    } else {
      setCarousel(carousel + 100);
    }
  };

  return (
    <div className="testimony-items">
      <FontAwesomeIcon
        icon={faAngleLeft}
        onClick={handleCarouselLeft}
        className="testimony-prev"
      />
      <div
        className="testimony-item"
        style={{ transform: `translateX(${carousel}%)` }}
      >
        {data.map((item) => (
          <div className="t-item" key={item.description}>
            <LazyLoadImage
              src={item.imgurl}
              className="t-profile"
              alt="profile"
            />
            <div className="t-info">
              <p className="t-item-description">"{item.description}"</p>
              <p className="t-item-name">
                <b>{item.author}</b>
              </p>
              <p className="t-item-role">{item.occupation}</p>
            </div>
          </div>
        ))}
      </div>
      <FontAwesomeIcon
        icon={faAngleRight}
        onClick={handleCarouselRight}
        className="testimony-next"
      />
    </div>
  );
};

export default TestimonyItem;
