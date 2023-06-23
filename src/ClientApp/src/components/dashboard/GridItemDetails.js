import React from "react";
import GridItemContent from "./GridItemContent";
import Statusbar from "./Statusbar";
import GridDetailHeader from "./GridDetailHeader";
import RightBarComponent from "./RightBarComponent";

const GridItemDetails = ({
  heading,
  menus,
  title,
  company,
  data,
  onMenuClick,
  customAction,
}) => {
  return (
    <main className="w-full min-h-full">
      <GridDetailHeader
        menus={menus}
        heading={heading}
        onMenuClick={onMenuClick}
      />
      <section className="mt-5 w-full gap-2 md:gap-0 flex flex-col-reverse md:flex-row">
        <article className="w-full px-2 md:w-9/12 lg:px-5 box-border">
          {data && <GridItemContent data={data} title={title} />}
        </article>
        <article className="w-full px-2 md:w-3/12 lg:px-5 ">
          <RightBarComponent customAction={customAction} />
        </article>
      </section>
      <Statusbar heading={heading} company={company} />
    </main>
  );
};

export default GridItemDetails;
