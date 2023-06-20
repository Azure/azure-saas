import React from "react";
import { ImSpinner } from "react-icons/im";

const LoadingComponent = () => {
  return (
    <main className="w-full h-screen md:w-screen fixed top-0 bg-neutral-900 left-0 bg-opacity-50 flex items-center justify-center z-50 ">
      <section>
        <main className="flex gap-1 items-center">
          <ImSpinner className=" text-4xl animate-spin" />
          <span className="text-xs">Loading...</span>
        </main>
      </section>
    </main>
  );
};

export default LoadingComponent;
