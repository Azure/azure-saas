import React from "react";

const GridItemContent = ({ data, title }) => {
  return (
    <main className="lg:rounded-md h-screen md:h-full flex flex-col gap-3 lg:shadow-xl p-0 lg:p-5">
      <section>
        <h2 className="font-semibold">{title}</h2>
      </section>
      <hr className="border h-0 border-gray-200" />
      <section className="flex flex-wrap gap-4 justify-between">
        {Object.keys(data).map((key) => (
          <article
            key={key}
            className="w-full md:w-[45%] lg:w-[48%] flex justify-between gap-1 items-center border-b border-gray-300"
          >
            <h3 className=" font-light text-xs">{key}:</h3>
            <h5 className="font-medium text-sm">{data[key]}</h5>
          </article>
        ))}
      </section>
    </main>
  );
};

export default GridItemContent;
