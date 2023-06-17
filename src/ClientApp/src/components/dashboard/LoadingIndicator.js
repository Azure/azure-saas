import React from "react";
import { ImSpinner } from "react-icons/im";

const LoadingIndicator = () => {
  return (
    <main className="flex gap-1 items-center">
      <ImSpinner className=" animate-spin" />
      <span className="text-xs">Loading...</span>
    </main>
  );
};

export default LoadingIndicator;
