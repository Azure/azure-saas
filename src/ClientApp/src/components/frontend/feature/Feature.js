import { LazyLoadImage } from "react-lazy-load-image-component";
import features from "../../../data/products.json";
import { Link } from "react-router-dom";

const Feature = () => {
  return (
    <main className="py-3 px-8 md:py-8 md:px-20 bg-bgSection">
      <section>
        <article className="text-center">
          <h2 className="text-white text-xl md:text-3xl font-semibold">
            {features.header}
          </h2>
          <p className="w-full  m-5 mx-auto text-lg text-white opacity-80">
            {features.content}
          </p>
        </article>
        <article className="flex items-center flex-wrap justify-evenly">
          {features?.items?.map((item) => (
            <div
              key={item.key}
              className="w-full px-5  box-border md:w-1/2 mt-4"
            >
              <div className="flex px-3 py-5 gap-5  bg-white rounded-md shadow-2xl">
                <LazyLoadImage
                  src={item.icon}
                  className="w-12 h-12"
                  alt="Bannerimage"
                />
                <div className="flex flex-col gap-2">
                  <h4 className="font-semibold text-lg">{item?.title}</h4>
                  <p className="text-gray-600">{item?.description}</p>
                </div>
              </div>
            </div>
          ))}
        </article>
        <article className="mt-8 flex items-center justify-center">
          <Link to={process.env.REACT_APP_SIGNUPIN_URL}>
            <div className="w-fit px-3 py-2 cursor-pointer rounded-sm bg-headingBlue text-white hover:bg-buttonHover">
              {features.mainBtn}
            </div>
          </Link>
        </article>
      </section>
    </main>
  );
};

export default Feature;
