import { ImUndo2 } from "react-icons/im";
import SelectBox from "devextreme-react/select-box";
import DateBox from "devextreme-react/date-box";
import NumberBox from "devextreme-react/number-box";
import Validator, { RequiredRule } from "devextreme-react/validator";
import { Button } from "devextreme-react";
import services from "../../helpers/formDataSource";

const SalesApprovalComponent = ({ handleClose }) => {
  return (
    <main className="h-full">
      <form className="flex flex-col h-full justify-between">
        <article className="flex px-5  flex-wrap  w-full">
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Total Sales
            </label>
            <NumberBox
              placeholder="Type here.."
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            >
              <Validator>
                <RequiredRule message="Organisation name is required" />
              </Validator>
            </NumberBox>
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Paid
            </label>
            <NumberBox
              placeholder="Type here.."
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            >
              <Validator>
                <RequiredRule message="Organisation name is required" />
              </Validator>
            </NumberBox>
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Payment Mode
            </label>
            <SelectBox
              dataSource={salesPaymentModeOptions}
              searchEnabled={true}
              placeholder="Select a Payment Mode"
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            />
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Bank Account
            </label>
            <SelectBox
              dataSource={paymentModeOptions}
              searchEnabled={true}
              placeholder="Select a Payment Mode"
              height={30}
              style={{ fontSize: "12px" }}
              className=" border pl-1 text-center w-full  outline-none"
            />
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>RefNo
            </label>
            <NumberBox
              placeholder="Type here.."
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            >
              <Validator>
                <RequiredRule message="Organisation name is required" />
              </Validator>
            </NumberBox>
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Customer Balance
            </label>
            <NumberBox
              placeholder="Type here.."
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            >
              <Validator>
                <RequiredRule message="Organisation name is required" />
              </Validator>
            </NumberBox>
          </div>
          <div className="box-border  w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Purchase Order
            </label>
            <NumberBox
              placeholder="Type here.."
              height={30}
              style={{ fontSize: "12px" }}
              className="border pl-1 text-center w-full  outline-none"
            >
              <Validator>
                <RequiredRule message="Organisation name is required" />
              </Validator>
            </NumberBox>
          </div>
          <div className="box-border w-full flex flex-col justify-between gap-1 mb-2">
            <label
              className="text-[11px] text-label font-semibold"
              htmlFor="fullName"
            >
              <sup className="text-red-600">*</sup>Payment Date
            </label>
            <DateBox
              id="courseDate"
              height={30}
              style={{ fontSize: "12px" }}
              value={today}
              className="border pl-1 text-center w-full  outline-none"
            />
          </div>
        </article>
        <article className="w-full border-t border-gray-300 py-1.5 bg-white sticky inset-x-0 bottom-0 flex justify-center items-center gap-4">
          <Button id="devBlueButton" useSubmitBehavior={true}>
            <span className="font-semibold text-xs border-none py-1 px-3 rounded-sm w-fit text-white cursor-pointer">
              OK
            </span>
          </Button>
          <button
            type="button"
            onClick={handleClose}
            className="flex gap-1 text-xs  items-center font-semibold bg-menuButton hover:bg-buttonBg border-none py-1 px-3 rounded-sm w-fit text-white cursor-pointer"
          >
            <ImUndo2 fontSize={18} />
            Cancel
          </button>
        </article>
      </form>
    </main>
  );
};

const paymentModeOptions = services.getPaymentMode();
const salesPaymentModeOptions = services.getSalesPaymentMode();
const today = new Date().toISOString().slice(0, 10);

export default SalesApprovalComponent;
