import { useState } from "react";
import DateBox from "devextreme-react/date-box";

const today = new Date().toISOString().slice(0, 10);

const FromToDateComponent = () => {
  const [fromDate, setFromDate] = useState(today);
  const [toDate, setToDate] = useState(today);

  return (
    <article className=" md:pr-5 flex gap-4 items-center">
      <div className="flex flex-col gap-2 md:flex-row w-full md:py-2">
        <div className="flex w-full justify-between md:justify-start md:w-1/2 items-center gap-5">
          <label
            className="font-semibold text-xs  text-gray-600"
            htmlFor="fromDate"
          >
            From Date:
          </label>

          <DateBox
            id="fromDate"
            onValueChanged={(e) => setFromDate(e.value)}
            value={fromDate}
            height={26}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-1/2  outline-none"
          />
        </div>
        <div className="flex w-full justify-between md:justify-start md:w-1/2 items-center gap-5">
          <label
            className="font-semibold text-xs  text-gray-600"
            htmlFor="toDate"
          >
            To Date:
          </label>
          <DateBox
            id="toDate"
            onValueChanged={(e) => setToDate(e.value)}
            value={toDate}
            height={26}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-1/2  outline-none"
          />
        </div>
      </div>
    </article>
  );
};

export default FromToDateComponent;
